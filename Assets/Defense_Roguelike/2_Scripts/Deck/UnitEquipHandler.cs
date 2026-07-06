using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitEquipHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform unitItem;
    [SerializeField] UnitList[] unitLists;
    [SerializeField] UnitList sephiraList;
    private RectTransform parentRectTransform;
    private Canvas rootCanvas;
    private GraphicRaycaster graphicRaycaster;


    private void Awake()
    {
        parentRectTransform = unitItem.parent as RectTransform;
        rootCanvas = GetComponentInParent<Canvas>();
        graphicRaycaster = rootCanvas.GetComponent<GraphicRaycaster>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        graphicRaycaster.ignoreReversedGraphics = false;
        Camera eventCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, eventCamera, out Vector2 localPosition))
        {
            unitItem.anchoredPosition = localPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UnitList targetSlot = null;

        if (EquipmentManager.Inst.unitTab.activeInHierarchy)
        {
            targetSlot = FindBestUnitSlot(eventData);
        }
        else if (EquipmentManager.Inst.sephiraTab.activeInHierarchy)
        {
            targetSlot = FindBestSephiraSlot(eventData);
        }

        if (targetSlot != null)
        {
            targetSlot.SelectDataInit();
        }
        EquipmentManager.Inst.OnHideSelectPanel();
        unitItem.anchoredPosition = Vector3.zero;
    }

    private UnitList FindBestUnitSlot(PointerEventData eventData)
    {
        Camera eventCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;

        Vector2 pointerScreenPosition = eventData.position;

        UnitList nearestSlot = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < unitLists.Length; i++)
        {
            UnitList currentSlot = unitLists[i];
            if (currentSlot == null)
            {
                continue;
            }

            RectTransform slotRectTransform = currentSlot.transform as RectTransform;
            if (slotRectTransform == null)
            {
                continue;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, pointerScreenPosition, eventCamera))
            {
                return currentSlot;
            }

            Vector2 slotScreenPosition = RectTransformUtility.WorldToScreenPoint(eventCamera, slotRectTransform.position);
            float distance = Vector2.Distance(pointerScreenPosition, slotScreenPosition);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSlot = currentSlot;
            }
        }

        const float maxAllowedDistance = 120f;
        return nearestDistance <= maxAllowedDistance ? nearestSlot : null;
    }

    private UnitList FindBestSephiraSlot(PointerEventData eventData)
    {
        if (sephiraList == null)
        {
            return null;
        }

        Camera eventCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;

        Vector2 pointerScreenPosition = eventData.position;

        RectTransform sephiraRectTransform = sephiraList.transform as RectTransform;
        if (sephiraRectTransform == null)
        {
            return null;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(sephiraRectTransform, pointerScreenPosition, eventCamera))
        {
            return sephiraList;
        }

        Vector2 sephiraScreenPosition = RectTransformUtility.WorldToScreenPoint(eventCamera, sephiraRectTransform.position);
        float distance = Vector2.Distance(pointerScreenPosition, sephiraScreenPosition);

        const float maxAllowedDistance = 120f;
        return distance <= maxAllowedDistance ? sephiraList : null;
    }
}
