using System.Linq;
using Kart;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

namespace Car
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CarCamera : MonoBehaviour
    {
        [SerializeField] private CarController targetKart;
        
        [Header("Main properties")]
        [SerializeField] private float distance = 2.0f;
        [SerializeField] private float initialDist = 6.0f;
        [SerializeField] private float height = 0.0f;
        [SerializeField] private float initialHeight = 2.0f;
        [SerializeField] private float maxVelDist = 1.0f;
        [SerializeField] private float exaggerateFov = 0.85f;

        [Header("Advanced properties")]
        [SerializeField] private LayerMask castMask = 1;
        [SerializeField] private float smoothRate = 10f;
        [SerializeField] private bool rollWithKart = true;
        [SerializeField] private float rollSmoothRate = 2.0f;
        [SerializeField] private float inputDeadZone = 0.1f;
        [SerializeField] private float xAngle = 10f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem speedLines;
        
        private UnityEngine.Camera _cam;
        private Rigidbody _targetBody;
        private float _upDirBlend = 1.0f;
        private float _castDist = 0.0f;
        private Transform _tempRot;
        private Transform _smoothObj;
        private Vector3 _localDir = Vector3.back;
        private Vector3 _highPoint = Vector3.zero;
        private Vector3 _smoothVel = Vector3.zero;
        private Vector2 _rotateInput = Vector2.zero;
        private bool _lookBack;
        private bool _spectatorMode = false;
        private bool _previouslyOwned = false;
        
        private Vector3 _targetPos = Vector3.zero;
        private Quaternion _targetRot = Quaternion.identity;

        private InputActions _input = null;

        private void Awake() {
            _cam = GetComponent<UnityEngine.Camera>();

            if (GetComponent<AudioListener>() != null) {
                // Change velocity update mode because the camera moves in FixedUpdate
                GetComponent<AudioListener>().velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
            }
            
            _input = new InputActions();

            Initialize(targetKart);
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        public void Initialize(CarController kart) {
            targetKart = kart;
            if (targetKart != null) {
                if (_smoothObj == null) {
                    _smoothObj = new GameObject("Kart Camera Smoother").transform;
                }
                _smoothObj.position = targetKart.Normal.position;
                _smoothObj.rotation = targetKart.Normal.rotation;

                if (_tempRot == null) {
                    _tempRot = new GameObject("Kart Camera Rotator").transform;
                }
                _tempRot.parent = _smoothObj;
                _tempRot.localPosition = Vector3.zero;
                _tempRot.localRotation = Quaternion.identity;
                _targetBody = targetKart.Rigidbody;

                _previouslyOwned = true;
            }
        }
        
        public void OnRotate(CallbackContext context) {
            _rotateInput = context.ReadValue<Vector2>();
        }
        
        public void OnLookBack(CallbackContext context) {
            ButtonControl button = context.control as ButtonControl;
            if (button != null) {
                _lookBack = button.isPressed;
            }
            else {
                _lookBack = false;
            }
        }
        
        public void OnRestartPress(CallbackContext context) 
        {
            ButtonControl button = context.control as ButtonControl;
            if (button != null && button.wasPressedThisFrame) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        private void LateUpdate() 
        {
            
            _lookBack = _input.Kart.LookBack.IsPressed();
            var changeSpectate = _input.System.ChangePlayer.WasPressedThisFrame();

            if (changeSpectate && _spectatorMode)
            {
                FindNewKart();
            }
            
            if (_cam == null || targetKart == null || _targetBody == null)
            {
                if (changeSpectate && !_spectatorMode && _previouslyOwned)
                {
                    FindNewKart();
                }
                
                return;
            }

            // Movement calculations
            var localVel = targetKart.LocalVelocity;
            localVel.y = 0f;
            _smoothVel = Vector3.Lerp(_smoothVel, localVel, smoothRate * Time.deltaTime);
            
            if (speedLines != null) speedLines.Emit( Mathf.FloorToInt(_smoothVel.magnitude*0.2f));

            distance = initialDist + Mathf.Min(maxVelDist, new Vector3(_smoothVel.x, 0.0f, _smoothVel.z).magnitude * 0.1f);
            
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, 45 + targetKart.LocalVelocity.magnitude * exaggerateFov, 0.05f);
            
            distance -= _cam.fieldOfView * 0.03f;
            
            height = initialHeight - Mathf.Clamp(_smoothVel.y * 0.1f, -maxVelDist, maxVelDist);

            _smoothObj.position = targetKart.Normal.position - targetKart.Normal.right * Mathf.Clamp(_smoothVel.x * 0.06f, -maxVelDist, maxVelDist);
            _smoothObj.rotation = Quaternion.Lerp(_smoothObj.rotation, targetKart.Normal.rotation, smoothRate * Time.deltaTime);
            
            Vector2 rotateInputNormalized = _rotateInput.normalized;

            // Apply input to movement
            float targetAngle = _rotateInput.magnitude < inputDeadZone ? 0.0f : Mathf.Atan2(rotateInputNormalized.x, rotateInputNormalized.y);
            _localDir = _lookBack ? Vector3.back : Vector3.Slerp(_localDir, new Vector3(-Mathf.Sin(targetAngle), 0.0f, -Mathf.Cos(targetAngle)), 0.1f);
            _tempRot.localPosition = new Vector3(_localDir.x, 0.0f, _localDir.z * (_lookBack ? -1.0f : 1.0f)) * distance + Vector3.up * height;

            // Calculate target rotation
            if (rollWithKart) {
                _upDirBlend = Mathf.Lerp(_upDirBlend, Mathf.Clamp01(Vector3.Dot(targetKart.Normal.up, Vector3.up)), rollSmoothRate * Time.deltaTime);
            }
            else {
                _upDirBlend = 1.0f;
            }
            _tempRot.rotation = Quaternion.LookRotation(_smoothObj.TransformDirection(-_localDir), Vector3.Lerp(targetKart.Normal.up, Vector3.up, _upDirBlend));

            // Raycast upward to determine how high the camera should be placed relative to the kart
            Vector3 targetHighPoint = _smoothObj.TransformPoint(Vector3.up * height);
            RaycastHit highHit = new RaycastHit();
            if (Physics.Linecast(_smoothObj.position, targetHighPoint, out highHit, castMask, QueryTriggerInteraction.Ignore)) {
                //_highPoint = highHit.point + (_smoothObj.position - targetHighPoint).normalized * _cam.nearClipPlane;
                //TODO: Fix this ground clipping bug
            }
            else {
                _highPoint = targetHighPoint;
            }

            // Raycast from the high point to determine how far away the camera should be from the kart
            RaycastHit lineHit = new RaycastHit();
            if (Physics.Linecast(_highPoint, _tempRot.position, out lineHit, castMask, QueryTriggerInteraction.Ignore)) {
                _castDist = 1.0f - lineHit.distance / Mathf.Max(Vector3.Distance(_highPoint, _tempRot.position), 0.001f);
            }
            else {
                _castDist = Mathf.Lerp(_castDist, 0.0f, smoothRate * Time.fixedDeltaTime);
            }

            // Set final position and rotation of camera
            _targetPos = _tempRot.position + (_highPoint - _tempRot.position) * _castDist;
            _targetRot = Quaternion.LookRotation(_tempRot.rotation * Vector3.forward * (_lookBack ? -1.0f : 1.0f), _tempRot.rotation * Vector3.up);

            transform.position = _targetPos;
            transform.rotation = _targetRot;
            transform.Rotate(xAngle, 0, 0);
        }

        private void FindNewKart()
        {
            //TODO: Expensive method invocation
            _spectatorMode = true;
            
            var kartControllers = FindObjectsOfType<CarController>().ToList();

            if (kartControllers.Count < 1)
                return;

            if (targetKart != null && kartControllers.Contains(targetKart))
            {
                kartControllers.Remove(targetKart);
            }

            var select = UnityEngine.Random.Range(0, kartControllers.Count-1);
            if (kartControllers[select] != null)
            {
                Initialize(kartControllers[select]);
            }
            
        }


        private void OnDestroy() {
            if (_tempRot != null) {
                Destroy(_tempRot.gameObject);
            }

            if (_smoothObj != null) {
                Destroy(_smoothObj.gameObject);
            }
        }
    }
}