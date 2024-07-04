using Car;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlayFollowCamera : MonoBehaviour
{
    [SerializeField] private RawImage profilePictureImage;
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private Transform followObject;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CarController controller;
        
    private bool _canFollow;
    private Camera _mainCamera;
    private Transform _objectFollow;
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        TryGetComponent(out _canvasGroup);
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        FollowCamera(followObject, mainCamera);

        if (controller.IsPlayer)
        {
            var nftManager = FindObjectOfType<NftManager>();
            if (nftManager == null)
            {
                _canFollow = false;
            }
            else
            {
                profilePictureImage.texture = nftManager.NftTexture;
            }
        }
    }

    private void Update()
    {
        if (!_canFollow) return;
        if (_objectFollow == null)
        {
            _canFollow = false;
            return;
        }
        Vector3 targPos = _objectFollow.transform.position + offset;
        Vector3 camForward = _mainCamera.transform.forward;
        Vector3 camPos = _mainCamera.transform.position + camForward;
        float distInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);
        if (distInFrontOfCamera < 0f)
        {
            targPos -= camForward * distInFrontOfCamera;
        }

        if (!controller.CanMove)
        {
            _canvasGroup.alpha = 1;
        }

        /*if (controller == CarController.PlayerController && controller.CanMove)
        {
            _canvasGroup.alpha = 0;
            return;
        }*/
        
        
        var distance = Vector3.Distance(controller.Model.transform.position,
            CarController.PlayerController.Model.transform.position);
        float normalizedDistance = Mathf.InverseLerp(4f, 6f, distance);
        _canvasGroup.alpha = 1 - normalizedDistance;

        var pos = RectTransformUtility.WorldToScreenPoint (_mainCamera, targPos);
        uiElement.DOMove(pos, 0.5f);
    }
        
    public void FollowCamera(Transform targetObject, Camera targetCamera)
    {
        _canFollow = true;
        _objectFollow = targetObject;
        _mainCamera = targetCamera;
    }
}