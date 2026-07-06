using DG.Tweening;
using UnityEngine;

public class GlimmerFX : MonoBehaviour
{
    RectTransform rectTransform;
    public float startPos;
    public float stopPos;
    public float startDelay = 0;
    private Tween tween;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        rectTransform.anchoredPosition = new Vector2(startPos, rectTransform.anchoredPosition.y);
        Invoke(nameof(StartTween), startDelay);
    }

    void OnDisable()
    {
        ClearTween();
        CancelInvoke();
    }

    void OnDestroy()
    {
        ClearTween();
    }

    void ClearTween()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill();
            tween = null;
        }
    }

    void StartTween()
    {
        if (this == null || rectTransform == null || !gameObject.activeInHierarchy)
            return;

        ClearTween();

        tween = rectTransform.DOLocalMoveX(stopPos, 1f)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (this == null || rectTransform == null || !gameObject.activeInHierarchy)
                    return;

                DOVirtual.DelayedCall(2f, () =>
                {
                    if (this == null || rectTransform == null || !gameObject.activeInHierarchy)
                        return;

                    rectTransform.anchoredPosition = new Vector2(startPos, rectTransform.anchoredPosition.y);
                    StartTween();
                }).SetTarget(this);
            })
            .SetTarget(this);
    }
}