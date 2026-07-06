using UnityEngine;
using UnityEngine.UI;

public class TabScrollReset : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    public void ChangeTab()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();

        scrollRect.StopMovement();
        scrollRect.velocity = Vector2.zero;

        scrollRect.verticalNormalizedPosition = 1f;

        Canvas.ForceUpdateCanvases();
    }
}