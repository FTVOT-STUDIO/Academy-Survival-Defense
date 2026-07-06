using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    Image image;
    [SerializeField] float delay = 0;

    void OnEnable()
    {
        image = GetComponent<Image>();

        var color = image.color;
        color.a = 1f;
        image.color = color;

        image.DOFade(0, 0.4f)
             .SetDelay(delay)
             .SetEase(Ease.InQuint)
             .SetUpdate(true)
             .OnComplete(() => gameObject.SetActive(false));
    }

    void OnDestroy()
    {
        DOTween.KillAll();
    }
}
