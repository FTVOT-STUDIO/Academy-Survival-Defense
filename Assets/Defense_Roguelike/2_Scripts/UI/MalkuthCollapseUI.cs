using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MalkuthCollapseUI : MonoBehaviour
{
    public const float ImpactTime = 13f / 30;
    public const float Duration = 1.45f;
    public PixelFxClip clip;
    public TMP_FontAsset font;
    RawImage picture;
    CanvasGroup group;
    TMP_Text title;
    float elapsed;

    void Awake()
    {
        var root = new GameObject("Reverse Genesis", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
        root.transform.SetParent(transform, false);
        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 180;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        group = root.GetComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        var shade = new GameObject("Letterbox", typeof(RectTransform), typeof(Image));
        shade.transform.SetParent(root.transform, false);
        Stretch(shade.GetComponent<RectTransform>());
        var background = shade.GetComponent<Image>();
        background.color = new Color(0.015f, 0.005f, 0.025f, 0.08f);
        background.raycastTarget = false;
        var scene = new GameObject("Cosmic Collapse", typeof(RectTransform), typeof(RawImage), typeof(AspectRatioFitter));
        scene.transform.SetParent(root.transform, false);
        picture = scene.GetComponent<RawImage>();
        picture.raycastTarget = false;
        var aspect = scene.GetComponent<AspectRatioFitter>();
        aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        aspect.aspectRatio = 16f / 9;
        title = Text(root.transform, "여명", 40, new Vector2(0, 105), new Vector2(700, 64));
        Text(root.transform, "말쿠트", 17, new Vector2(0, 158), new Vector2(500, 30));
    }

    TMP_Text Text(Transform parent, string content, float size, Vector2 location, Vector2 bounds)
    {
        var obj = new GameObject(content, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = content;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1, 0.84f, 0.7f);
        text.raycastTarget = false;
        text.rectTransform.anchorMin = text.rectTransform.anchorMax = new Vector2(0.5f, 0);
        text.rectTransform.anchoredPosition = location;
        text.rectTransform.sizeDelta = bounds;
        return text;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    public void Play()
    {
        if (picture == null) Awake();
        elapsed = 0;
        picture.texture = clip.atlas;
        Sample(0);
    }

    // 검증 도구도 같은 시점을 렌더링하여 폭발과 즉사 판정의 동기를 확인합니다.
    public void Sample(float time)
    {
        if (picture == null || clip == null) return;
        picture.texture = clip.atlas;
        int index = Mathf.Clamp(Mathf.FloorToInt(time * clip.fps), 0, clip.frameCount - 1);
        picture.uvRect = clip.FrameUV(index);
        group.alpha = Mathf.SmoothStep(0, 1, Mathf.Clamp01(time / 0.045f)) * (1 - Mathf.SmoothStep(0, 1, Mathf.Clamp01((time - 1.18f) / 0.27f)));
        title.characterSpacing = Mathf.Lerp(10, 3, Mathf.Clamp01(time / ImpactTime));
    }

    void Update()
    {
        if (GameManager.Inst != null)
        {
            if (!GameManager.Inst.isGameStart) { gameObject.SetActive(false); return; }
            if (GameManager.Inst.isPause) return;
        }
        elapsed += Time.deltaTime;
        Sample(elapsed);
        if (elapsed >= Duration) gameObject.SetActive(false);
    }

    void OnDisable()
    {
        elapsed = 0;
        if (group != null) group.alpha = 0;
    }
}
