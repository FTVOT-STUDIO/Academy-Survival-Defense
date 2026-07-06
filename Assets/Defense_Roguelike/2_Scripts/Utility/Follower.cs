using System.Linq;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [HideInInspector] private Transform followPos;
    private Vector3 targetVecPos;
    RectTransform rectTransform;

    [SerializeField] private Camera worldCamera;
    [SerializeField] private Camera uiCamera;

    public enum RenderTyep { None, Overlay, Camera }
    RenderTyep renderTyep;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        worldCamera = Camera.main;
        uiCamera = InGameUIManager.Inst.uiCamera;
    }

    void FixedUpdate()
    {
        switch (renderTyep)
        {
            case RenderTyep.None:
                Debug.Log("Not Init Render Type");
                break;
            case RenderTyep.Overlay:
                FollowOverlay();
                break;
            case RenderTyep.Camera:
                FollowCamera();
                break;
        }
    }

    public void InitRenderType(RenderTyep renderTyep, Transform pos, Vector3 vecPos)
    {
        this.renderTyep = renderTyep;
        followPos = pos;
        targetVecPos = vecPos;
    }

    private void FollowOverlay()
    {
        Vector3 worldPos = followPos.position + targetVecPos;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        rectTransform.position = screenPos;
    }


    private void FollowCamera()
    {
        Vector3 worldPos = followPos.position + targetVecPos;
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            uiCamera,
            out Vector2 localPoint
        );

        rectTransform.localPosition = localPoint;
    }
}
