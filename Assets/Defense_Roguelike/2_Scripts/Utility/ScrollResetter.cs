using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollResetter
{
    public enum ScrollType { Vertical, Horizontal }

    public static void ResetScroll(MonoBehaviour mono, ScrollRect scrollRect, ScrollType direction)
    {
        mono.StartCoroutine(ResetScrollRoutine(scrollRect, direction));
    }

    private static IEnumerator ResetScrollRoutine(ScrollRect scrollRect, ScrollType direction)
    {
        yield return null;

        switch (direction)
        {
            case ScrollType.Vertical:
                scrollRect.verticalNormalizedPosition = 1f;
                break;
            case ScrollType.Horizontal:
                scrollRect.horizontalNormalizedPosition = 0f;
                break;
        }
    }

    public static void ResetVertical(MonoBehaviour mono, ScrollRect scrollRect)
        => ResetScroll(mono, scrollRect, ScrollType.Vertical);

    public static void ResetHorizontal(MonoBehaviour mono, ScrollRect scrollRect)
        => ResetScroll(mono, scrollRect, ScrollType.Horizontal);
}
