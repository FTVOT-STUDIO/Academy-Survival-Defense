using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollScript : ScrollRect
{
    bool forParent;
    ScrollManager scrollManager;
    ScrollRect parentScrollRect;

    protected override void Start()
    {
        scrollManager = GameObject.FindWithTag("ScrollManager")?.GetComponent<ScrollManager>();
        parentScrollRect = GameObject.FindWithTag("ScrollManager")?.GetComponent<ScrollRect>();

        if (scrollManager == null)
        {
            Debug.LogError("ScrollManager is missing in the scene.");
        }

        if (parentScrollRect == null)
        {
            Debug.LogError("Parent ScrollRect is missing in the scene.");
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {

        forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if (forParent)
        {
            scrollManager.OnBeginDrag(eventData);
            parentScrollRect.OnBeginDrag(eventData);
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {


        if (forParent)
        {
            scrollManager.OnDrag(eventData);
            parentScrollRect.OnDrag(eventData);
        }
        else
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {


        if (forParent)
        {
            scrollManager.OnEndDrag(eventData);
            parentScrollRect.OnEndDrag(eventData);
        }
        else
        {
            base.OnEndDrag(eventData);
        }
    }
}
