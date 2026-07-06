using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Inst;

    void Awake()
    {
        Inst = this;
    }

    public void ShowFleshImage(Transform canvas, Color color, float alpha = 1f)
    {
        GameObject fleshImage = PoolManager.Inst.Get(PoolManager.PrefabType.FleshImage, transform.position);
        fleshImage.transform.SetParent(canvas, false);

        RectTransform rectTransform = fleshImage.GetComponent<RectTransform>();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Canvas.ForceUpdateCanvases();

        Image image = fleshImage.GetComponent<Image>();

        color.a = alpha;
        image.color = color;

        image.DOFade(0f, 0.3f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnPlay(() => { fleshImage.SetActive(true); })
            .OnComplete(() =>
            {
                fleshImage.transform.SetParent(PoolManager.Inst.transform, false);
                fleshImage.SetActive(false);
            });
    }

    public void RefreshLayoutOnce(LayoutGroup layoutGroup)
    {
        StartCoroutine(RefreshLayoutOnceCoroutine(layoutGroup));
    }

    IEnumerator RefreshLayoutOnceCoroutine(LayoutGroup layoutGroup)
    {
        layoutGroup.enabled = true;
        yield return new WaitForEndOfFrame();
        layoutGroup.enabled = false;
    }

    public void RefreshFitterOnce(ContentSizeFitter contentSizeFitter)
    {
        StartCoroutine(RefreshFitterOnceCoroutine(contentSizeFitter));
    }

    IEnumerator RefreshFitterOnceCoroutine(ContentSizeFitter contentSizeFitter)
    {
        contentSizeFitter.enabled = true;
        yield return new WaitForEndOfFrame();
        contentSizeFitter.enabled = false;
    }


    public void RefreshElementOnce(LayoutElement layoutElement)
    {
        StartCoroutine(RefreshElementOnceCoroutine(layoutElement));
    }

    IEnumerator RefreshElementOnceCoroutine(LayoutElement layoutElement)
    {
        layoutElement.enabled = true;
        yield return new WaitForEndOfFrame();
        layoutElement.enabled = false;
    }
}
