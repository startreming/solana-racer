using System;
using DG.Tweening;
using Domain;
using Effects;
using UnityEngine;

namespace Car
{
    public class CarController : MonoBehaviour
    {
        public static GameObject PlayerGameObject;
        public Vector3 LocalVelocity => _localVelocity;
        public Rigidbody Rigidbody => rigidbody;
        public CarModel Model => kartModel;
        public Transform Normal => normal;
        public bool Sliding => _isDrifting;
        public bool Grounded => _isGrounded;
        public float CurrentRotate => _currentRotate;
        public float CurrentSpeed => _currentSpeed;
        public bool IsDrifting => _isDrifting;
        public VehicleStats Stats => stats;

        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Transform kart;
        [SerializeField] private Transform normal;
        [SerializeField] private Transform rotator;
        [SerializeField] private InputCreator inputCreator;
        [SerializeField] private bool isPlayer;

        [Header("Parameters")]
        [SerializeField] private float maxForwardSpeed = 65f;
        [SerializeField] private float maxSteeringSpeed = 15f;
        [SerializeField] private float gravity = 25f;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private CarModel kartModel;
        [SerializeField] private VehicleStats stats;
        [SerializeField] private float vehicleY;

        private bool _canMove = true;
        private Vector3 _localVelocity;
        private float _airTime = 0f;
        private float _currentSpeed;
        private float _currentRotate;
        private int _driftDirection; // the client might know for visual purposes
        private int _driftMode;
        private float _driftPower;
        private float _nitroCollectionRate = 150f;
        private float _engineTime;
        private bool _isDrifting;
        private bool _isGrounded;
        private bool _isOffroad = false;
        private float _offroadMultiplier = 0.8f;
        private bool _trickQueued;
        private float _prevSpeed;
        private bool _prevIsGrounded;
        private GroundSurfacePreset _lastGroundSurfacePreset;

        public event Action OnDriftStart = () => { };
        public event Action OnDriftEnd = () => { };
        public event Action<int> OnDriftUpgrade = (driftLevel) => { };
        public event Action OnBoostStart = () => { };
        public event Action OnGroundEnter = () => { };
        public event Action OnGroundLeave = () => { };
        public event Action<GroundSurfacePreset> OnGroundTypeChange = (groundSurfacePreset) => { };

        private void Awake()
        {
            Application.targetFrameRate = 60;
            if (isPlayer)
            {
                PlayerGameObject = transform.parent.gameObject;
            }
        }

        public void OnEnable()
        {
            inputCreator.Initialize(this);
        }

        public void OnDisable()
        {
            inputCreator.UnInitialize();
        }

        public float _interpolation = 1.0f;

        private void Update()
        {
            Move();
            
            SyncPosition();
            SyncRotation();
            _localVelocity = normal.InverseTransformDirection(rigidbody.velocity);
        }

        private void SyncPosition()
        {
            float time = Time.deltaTime * _interpolation;
            float deltaTime = Time.deltaTime;

            Transform kartVisuals = kart.transform;
            Rigidbody rb = rigidbody;
            var targetPosition = rb.transform.position - new Vector3(0, vehicleY, 0);

            float distance = Vector3.Distance(kartVisuals.position, targetPosition);

            kartVisuals.transform.position = targetPosition;
            //Vector3.MoveTowards(kartVisuals.position, targetPosition, (distance / time) * deltaTime);
        }

        private void SyncRotation()
        {
            float time = Time.deltaTime * _interpolation;
            float deltaTime = Time.deltaTime;

            Transform kartVisuals = kart.transform;
            Transform turningAid = rotator;
            
            var distance = Quaternion.Angle(kartVisuals.rotation, rotator.rotation);

            kartVisuals.rotation = turningAid.rotation;
            //Quaternion.RotateTowards(kartVisuals.rotation, turningAid.rotation, speedSync * deltaTime);
        }

        public void SetCanMove(bool state)
        {
            _canMove = state;
        }

        public void SetKartRotation(Quaternion rotation)
        {
            rotator.rotation = rotation;
        }
        
        private void Move()
        {
            //_wasUserCreated = (state == ReplicateState.UserCreated || state == ReplicateState.ReplayedUserCreated);

            var forward = inputCreator.Forward;
            var horizontal = inputCreator.Horizontal;
            var drifting = inputCreator.IsDrifting;
            
            float delta = Time.deltaTime;

            if (kartModel == null)
                return;

            var gravityMultiplier = 0.2f;
            if (!_isGrounded)
            {
                gravityMultiplier = 1f;
                _airTime += Time.deltaTime;
            }
            
            float lerpTime = ApplyInputToEngineTime(forward, delta);
            float targetSpeed = ConvertEngineTimeToSpeed();

            if (!_canMove)
            {
                targetSpeed = 0f;
                _engineTime = 0f;
            }
            _currentSpeed = Mathf.SmoothStep(_currentSpeed, targetSpeed, delta * lerpTime);

            HandleDriftInput(horizontal, drifting);
            CalculateDriftStages();
            HandleSteering(horizontal);

            SyncVisuals(horizontal);
            HandleNormalRotation();
            CalculateDrag();

            ApplyMovementForces();
            rigidbody.AddForce(Vector3.down * gravity * gravityMultiplier, ForceMode.Acceleration);
            _prevSpeed = rigidbody.velocity.magnitude;
        }

        private float ConvertEngineTimeToSpeed()
        {
            float targetSpeed;
            if (_engineTime >= 0)
            {
                targetSpeed = stats.GetCurrentSpeed(_engineTime);
            }
            else
            {
                targetSpeed = stats.GetReverseSpeed(-_engineTime);
            }

            return targetSpeed;
        }

        private float ApplyInputToEngineTime(float forward, float delta)
        {
            var lerpTime = 12f;
            if (forward > 0)
            {
                _engineTime += delta;
                if (_currentSpeed < 0)
                {
                    lerpTime = 18f;
                    _engineTime += delta * 2f;
                }
            }
            else if (forward < 0)
            {
                _engineTime -= delta * 15f;
                lerpTime = 18f;
            }
            else
            {
                lerpTime = 4f;
                if (_currentSpeed < 0)
                    lerpTime = 16f;
                _engineTime = Mathf.SmoothStep(_engineTime, 0f, delta*lerpTime);
            }

            _engineTime = Mathf.Clamp(_engineTime, -5f, 5f);
            return lerpTime;
        }

        private void CalculateDriftStages()
        {
            var _prevDriftMode = _driftMode;

            if (_driftPower > 50f && _driftPower < 100f)
            {
                _driftMode = 1;
            }

            if (_driftPower > 100f && _driftPower < 180f)
            {
                _driftMode = 2;
            }

            if (_driftPower > 180f)
            {
                _driftMode = 3;
            }

            if (_driftMode > _prevDriftMode)
            {
                OnDriftUpgrade(_driftMode);
            }
        }

        private void HandleDriftInput(float horizontal, bool drifting)
        {
            bool driftValue = drifting;
            float _horizontalSteer = horizontal;

            if (_isDrifting && !driftValue)
            {
                _isDrifting = false;
                FinishDriftAndBoost();
            }

            if (driftValue && !_isDrifting && _horizontalSteer != 0 && _currentSpeed > 20f && _isGrounded)
            {
                _isDrifting = true;
                _driftDirection = _horizontalSteer > 0 ? 1 : -1;

                OnDriftStart.Invoke();
            }
        }

        private void FinishDriftAndBoost(bool forceBoost = false)
        {
            _isDrifting = false;

            var boostLevel = forceBoost ? 1 : _driftMode;

            if (boostLevel > 0)
            {
                DOVirtual.Float(_currentSpeed * 3, _currentSpeed, .3f * boostLevel, 
                    x => _currentSpeed = x);
                OnBoostStart.Invoke();
            }

            _driftMode = 0;
            _driftPower = 0;
            _driftDirection = 0;
            OnDriftEnd.Invoke();
        }

        private void SyncVisuals(float steering)
        {
            var parentAngleTarget = 0f;

            if (!_isDrifting)
            {
                kartModel.PointTowardsTurn(steering*0.8f);
            }
            else
            {
                float control = _driftDirection == 1
                    ? steering.Remap(-1, 1, .5f, 2)
                    : steering.Remap(-1, 1, 2, .5f);
                parentAngleTarget = control * 15f * _driftDirection;
            }

            var parent = kartModel.transform.parent;
            parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(parent.localEulerAngles.y, parentAngleTarget, .05f), 0);

            var speedDelta = (rigidbody.velocity.magnitude - _prevSpeed)*0.4f;

            speedDelta *= Mathf.Sign(Vector3.Dot(rigidbody.velocity.normalized, transform.forward));

            kartModel.PointFrontWheels(steering, rigidbody.velocity.magnitude);
            kartModel.SpinBackWheels(rigidbody.velocity.magnitude);
            kartModel.PointSteeringWheel(steering);
            kartModel.WeightTransfer(speedDelta, _currentRotate, rigidbody.velocity.magnitude);
            kartModel.SetDrifting(_isDrifting);

            _localVelocity = normal.InverseTransformDirection(rigidbody.velocity);
        }

        private void ApplyMovementForces()
        {
            if (!_isGrounded)
                return;

            var maxSpeed = _currentSpeed;
            if (_isOffroad) maxSpeed *= _offroadMultiplier;

            maxSpeed += Mathf.Abs(_currentRotate * 0.08f);
            
            if (!_isDrifting)
            {
                var direction = rotator.transform.forward;
                direction += kart.transform.right * _currentRotate * 0.05f;
                
                rigidbody.AddForce(direction.normalized * maxSpeed, ForceMode.Acceleration);
            }
            else
            {
                var direction = kart.transform.forward;
                direction += kart.transform.right * -_driftDirection * 0.25f;
                
                rigidbody.AddForce(direction.normalized * maxSpeed, ForceMode.Acceleration);
            }
        }

        private void CalculateDrag()
        {
            rigidbody.drag = 2f;
            if (!_isGrounded)
            {
                rigidbody.drag = 0.1f;
            }
        }

        private void HandleSteering(float horizontalSteer)
        {
            float targetRotation = 0f;

            if (_isDrifting)
            {
                float control = (_driftDirection == 1)
                    ? horizontalSteer.Remap(-1, 1, 0.15f, 1.5f)
                    : horizontalSteer.Remap(-1, 1, 1.5f, 0.15f);
                float powerControl = (_driftDirection == 1)
                    ? horizontalSteer.Remap(-1, 1, .2f, 1)
                    : horizontalSteer.Remap(-1, 1, 1, .2f);
                
                _driftPower += powerControl * Time.deltaTime * _nitroCollectionRate;
                
                targetRotation = ApplyVelocityToSteering(_driftDirection, control * 0.7f);
            }
            else
            {
                if (horizontalSteer != 0)
                {
                    int direction = horizontalSteer > 0 ? 1 : -1;
                    float amount = Mathf.Abs(horizontalSteer);

                    targetRotation = ApplyVelocityToSteering(direction, amount);
                }
            }

            _currentRotate = Mathf.Lerp(_currentRotate, targetRotation, 0.1f);
            
            rotator.transform.Rotate(new Vector3(0f, _currentRotate * 0.15f, 0f));
        }

        private float ApplyVelocityToSteering(int direction, float amount)
        {
            var targetRotation = maxSteeringSpeed * direction * amount;
            targetRotation *= Mathf.Clamp(rigidbody.velocity.magnitude * 0.1f, 0f, 1f);
            targetRotation *= Mathf.Sign(_currentSpeed);//Vector3.Dot(rigidbody.velocity.normalized, kart.transform.forward));
            return targetRotation;
        }

        private void HandleNormalRotation()
        {
            RaycastHit hitOn;
            RaycastHit hitNear;

            Physics.Raycast(kart.transform.position + (kart.transform.up * .1f), Vector3.down, out hitOn, 1.1f,
                layerMask);
            Physics.Raycast(kart.transform.position + (kart.transform.up * .1f), Vector3.down, out hitNear, 2.0f,
                layerMask);

            normal.up = Vector3.Lerp(normal.up, hitNear.normal, Time.deltaTime * 8.0f);
            normal.Rotate(0, transform.eulerAngles.y, 0);

            if (hitOn.collider != null)
            {
                _isGrounded = true;

                if (_trickQueued)
                {
                    FinishDriftAndBoost(true);
                    _trickQueued = false;
                }
                
                _airTime = 0f;
                _isOffroad = false;

                var gs = hitOn.transform.GetComponent<GroundSurface>();
                
                GroundSurfacePreset gsp = null;
                if (gs != null)
                {
                    gsp = gs.GetProps();
                    _isOffroad = gsp.offRoad;
                }

                if (gsp != _lastGroundSurfacePreset)
                {
                    OnGroundTypeChange.Invoke(gsp);
                }

                _lastGroundSurfacePreset = gsp;
            }
            else
            {
                if (_isGrounded)
                    OnGroundTypeChange.Invoke(null);

                _isGrounded = false;
                _lastGroundSurfacePreset = null;
                _isOffroad = false;
            }

            if (_isGrounded && !_prevIsGrounded) OnGroundEnter.Invoke();
            if (!_isGrounded && _prevIsGrounded) OnGroundLeave.Invoke();

            _prevIsGrounded = _isGrounded;
        }
    }
}