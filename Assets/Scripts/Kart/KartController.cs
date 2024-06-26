using System;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Kart
{
    public class KartController : MonoBehaviour
    {
        public Vector3 LocalVelocity => _localVelocity;
        public Rigidbody Rigidbody => sphere;
        public Transform Normal => kartNormal;
        public KartModel Model => _kartModel;
        public bool Sliding => _isDrifting;
        public bool Grounded => _isGrounded;
        public float CurrentRotate => _currentRotate;
        
        [Header("Dependencies")]
        [SerializeField] private Transform kartNormal;
        [SerializeField] private Rigidbody sphere;
        [SerializeField] private KartModel _kartModel;

        [Header("Parameters")]
        [SerializeField] private LayerMask layerMask;
        
        private Vector3 _localVelocity;

        // Input
        private float _horizontalSteer;
        private bool _accelerateValue;
        private bool _isDrifting;
        private bool _braking;
        
        // might not need to syncvar, only server needs to know
        private float _speed;
        private float _currentSpeed;
        private float _rotate;
        private float _currentRotate;
        private int _driftDirection; // the client might know for visual purposes
        private int _driftMode;
        private float _driftPower;
        private bool _trickQueued;
        private Vector3 _sphereVelocity;
        private float _prevSpeed;

        private float _acceleration;
        private float _steering;
        private float _gravity;
        
        private bool _isGrounded;
        private float _carHopCooldown;
        private float _airTime;
        private bool _prevIsGrounded;
        private int _prevDriftMode;
        private Tweener _tweenLocalRotate;

        /*[SyncVar(OnChange = nameof(on_changeRotation), SendRate = 0.1f, Channel = Channel.Reliable)] 
        private Vector3 _rotationAngles; */

        public event Action OnDriftStart = () => { };
        public event Action OnDriftEnd = () => { };
        public event Action<int> OnDriftUpgrade = (driftLevel) => { };
        public event Action OnBoostStart = () => { };
        public event Action OnGroundEnter = () => { };
        public event Action OnGroundLeave = () => { };

        private void OnEnable()
        {
            OnGroundEnter += LandedDriftEnd;
        }
        
        private void OnDisable()
        {
            OnGroundEnter -= LandedDriftEnd;
        }

        private void LandedDriftEnd()
        {
            if (_isDrifting)
            {
                DriftEnd(false);
            }
            if (!_isDrifting && _trickQueued)
            {
                _trickQueued = false;
                RpcBoost(true);
            }
        }

        private void on_changeRotation(Vector3 prev, Vector3 next, bool asServer)
        {
            transform.eulerAngles = next;
        }

        public void SetKartModel(KartModel newModel)
        {
            _kartModel = newModel;
        }

        public void Accelerate(bool accelerateVal)
        {
            RpcSetAccelerate(accelerateVal);
        }

        public void Steer(float horizontalValue)
        {
            RpcSetSteer(horizontalValue);
        }
        
        private void RpcSetAccelerate(bool val)
        {
            _accelerateValue = val;
        }
        
        private void RpcSetSteer(float val)
        {
            _horizontalSteer = val;
        }
        
        private void RpcSetBraking(bool val)
        {
            _braking = val;
        }
        
        public void Drift(Boolean driftValue)
        {
            if (driftValue)
            {
                if (_carHopCooldown <= 0f)
                {
                    RpcSetDrift(true);
                    RpcDoCarHop();
                    _carHopCooldown = 0.3f;
                }
            }
            else
                RpcSetDrift(false);
        }
        
        public void Brake(Boolean brakeValue)
        {
            RpcSetBraking(brakeValue);
        }
        
        private void RpcSetDrift(bool driftValue)
        {
            if (_isDrifting && !driftValue)
            {
                _isDrifting = false;
            }
            
            if (driftValue && !_isDrifting && _horizontalSteer != 0 && _currentSpeed > 20f && _isGrounded)
            {
                _isDrifting = true;
                RpcSetDriftVisuals(true);
            }

            if (driftValue && !_isDrifting && !_isGrounded && _airTime < 0.3f && !_trickQueued)
            {
                _kartModel.Trick();
                _trickQueued = true;
            }
            
            if (driftValue)
            {
                _driftDirection = _horizontalSteer > 0 ? 1 : -1;
            }
        }
        
        public void DriftEnd(Boolean driftValue)
        {
            if (!driftValue && _isDrifting)
            {
                _isDrifting = false;
                RpcBoost();
            }
        }
        
        private void RpcDoCarHop()
        {
            RpcSendHop();
        }

        private void Update()
        {
            _accelerateValue = Input.GetKeyDown(KeyCode.W);
            
            SetKartComponentPositions();
            
            float delta = Time.deltaTime;
            if (_carHopCooldown > 0f) _carHopCooldown -= delta;

            if (_accelerateValue)
            {
                _speed = _acceleration;
            }
            else if (_braking)
            {
                _speed = _acceleration * -0.75f;
            }
            
            GetSteerInput();
        
            HandleDrift();

            CalculateDrag();

            if (_isGrounded)
                _airTime += delta;
            else
                _airTime = 0f;

            _sphereVelocity = sphere.velocity;

            _currentSpeed = Mathf.SmoothStep(_currentSpeed, _speed, delta * 12f); _speed = 0f;
            _currentRotate = Mathf.Lerp(_currentRotate, _rotate, delta * 2f); _rotate = 0f;
        }

        private void CalculateDrag()
        {
            Rigidbody.drag = 2f;

            if (sphere.velocity.magnitude < 0.2f && Mathf.Abs(_currentSpeed) < 1f)
                Rigidbody.drag = 20f;
            
            if (!_isGrounded)
            {
                Rigidbody.drag = 0.1f;
            }
        }
        
        private void FixedUpdate()
        {
            var target = sphere.transform.position - new Vector3(0, 0.4f, 0);
            float delta = Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target, delta * 15);
            
            HandleNormalRotation();

            SetForwardAcceleration();

            var gravityMultiplier = 0.4f;
            if (!_isGrounded) gravityMultiplier = 1f;
            if (Rigidbody.drag >= 9f)
                gravityMultiplier = 0f;

            sphere.useGravity = gravityMultiplier > 0f;
            sphere.AddForce(Vector3.down * _gravity * gravityMultiplier, ForceMode.Acceleration);

            transform.eulerAngles = 
                Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + _currentRotate, 0), 
                delta * 10f);
            
            _prevSpeed = sphere.velocity.magnitude;
        }



        private void SetKartComponentPositions()
        {
            
            // this works OK as long as isDrifting, horizontalSteer and driftDirection are sync
            if (!_isDrifting)
            {
                _kartModel.PointTowardsTurn(_horizontalSteer);
            }
            else
            {
                float control = _driftDirection == 1
                    ? _horizontalSteer.Remap(-1, 1, .5f, 2)
                    : _horizontalSteer.Remap(-1, 1, 2, .5f);
                var parent = _kartModel.transform.parent;
                parent.localRotation = Quaternion.Euler(0,
                    Mathf.LerpAngle(parent.localEulerAngles.y, (control * 15) * _driftDirection, .05f), 0);
            }

            var sphereVelocity = _sphereVelocity;
            
            var speedDelta = (sphereVelocity.magnitude - _prevSpeed)*0.4f;
            
            speedDelta *= Mathf.Sign(Vector3.Dot(sphereVelocity.normalized, transform.forward));
            
            _kartModel.PointFrontWheels(_horizontalSteer, sphereVelocity.magnitude);
            _kartModel.SpinBackWheels(sphereVelocity.magnitude);
            _kartModel.PointSteeringWheel(_horizontalSteer);
            _kartModel.WeightTransfer(speedDelta, _currentRotate, sphereVelocity.magnitude);
            _kartModel.SetDrifting(_isDrifting);

            _localVelocity = kartNormal.InverseTransformDirection(sphereVelocity);
        }

        
        private void GetSteerInput()
        {
            if (_horizontalSteer != 0)
            {
                int dir = _horizontalSteer > 0 ? 1 : -1;
                float amount = Mathf.Abs(_horizontalSteer);
                ApplyVelocityToSteering(dir, amount);
            }
        }
        private void HandleDrift()
        {
            if (_isDrifting)
            {
                float control = (_driftDirection == 1)
                    ? _horizontalSteer.Remap(-1, 1, 0, 1.5f)
                    : _horizontalSteer.Remap(-1, 1, 1.5f, 0);
                float powerControl = (_driftDirection == 1)
                    ? _horizontalSteer.Remap(-1, 1, .2f, 1)
                    : _horizontalSteer.Remap(-1, 1, 1, .2f);
                _driftPower += powerControl * Time.deltaTime * 100f;
                ApplyVelocityToSteering(_driftDirection, control*0.7f);
                HandleDriftMode();
            }
        }
        private void ApplyVelocityToSteering(int direction, float amount)
        {
            _rotate = (_steering * direction) * amount;
            _rotate *= Mathf.Clamp(sphere.velocity.magnitude*0.1f, 0f, 1f);
            _rotate *= Mathf.Sign(Vector3.Dot(sphere.velocity.normalized, transform.forward)); // :)
        }

        private void HandleDriftMode()
        {
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
                RpcSendBoostUpgrade(_driftMode);
            }
            
            _prevDriftMode = _driftMode;
        }

        private void HandleNormalRotation()
        {
            // not synchronized, this is done locally
            RaycastHit hitOn;
            RaycastHit hitNear;

            gameObject.scene.GetPhysicsScene()
                .Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
            gameObject.scene.GetPhysicsScene()
                .Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, layerMask);

            kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
            kartNormal.Rotate(0, transform.eulerAngles.y, 0);
            
            if (hitOn.collider != null)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }

            if (_isGrounded && !_prevIsGrounded) OnGroundEnter.Invoke();
            if (!_isGrounded && _prevIsGrounded) OnGroundLeave.Invoke();
            
            _prevIsGrounded = _isGrounded;
        }

        private void SetForwardAcceleration()
        {
            if (!_isGrounded) return;
            if (!_isDrifting)
            {
                sphere.AddForce(-_kartModel.transform.right * _currentSpeed, ForceMode.Acceleration);
            }
            else
            {
                sphere.AddForce(transform.forward * _currentSpeed, ForceMode.Acceleration);
                sphere.AddForce(transform.right * (-_driftDirection * _currentSpeed * 0.15f), ForceMode.Acceleration);
            }
        }
        
        private void RpcBoost(bool forceBoost = false)
        {
            _isDrifting = false;

            var boostLevel = forceBoost ? 1 : _driftMode;

            if (boostLevel > 0)
            {
                DOVirtual.Float(_currentSpeed * 3, _currentSpeed, .3f * boostLevel, x => _currentSpeed = x);
            }
            RpcSendVisuals(boostLevel>0);
            RpcSetDriftVisuals(false);

            _driftMode = 0;
            _driftPower = 0;
            _driftDirection = 0;
        }

        [UsedImplicitly]
        private void RpcSendHop()
        {
            _kartModel.Hop();
        }
        
        private void RpcSendBoostUpgrade(int driftMode)
        {
            OnDriftUpgrade.Invoke(driftMode);
        }
        
        private void RpcSetDriftVisuals(bool driftActive)
        {
            
            if(driftActive)
                OnDriftStart.Invoke();
            else
                OnDriftEnd.Invoke();

        }
        
        private void RpcSendVisuals(bool showsBoostParticle)
        {
            if(showsBoostParticle)
                OnBoostStart.Invoke();
            
            _tweenLocalRotate.Complete();
            _tweenLocalRotate = _kartModel.transform.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);
        }
    }
}
