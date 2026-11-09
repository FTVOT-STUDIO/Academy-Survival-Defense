using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SayaDiceRollingUI : MonoBehaviour
{
    public TextMeshProUGUI valueText { get; private set; }
    public TextMeshProUGUI resultText { get; private set; }
    Tween rollingTween;

    public void Initialize(Sprite panelSprite, TMP_FontAsset font)
    {
        var background = new GameObject("Dice Panel", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(transform, false);
        var image = background.GetComponent<Image>();
        image.sprite = panelSprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        var rect = background.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(132, 81);
        rect.anchoredPosition = new Vector2(24, 12);
        valueText = CreateText("Value", font, 32, new Vector2(24, 12), new Vector2(96, 46));
        resultText = CreateText("Result", font, 16, new Vector2(0, -38), new Vector2(192, 30));
    }

    TextMeshProUGUI CreateText(string label, TMP_FontAsset font, float size, Vector2 position, Vector2 bounds)
    {
        var obj = new GameObject(label, typeof(RectTransform));
        obj.transform.SetParent(transform, false);
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        text.enableAutoSizing = true;
        text.fontSizeMin = size * 0.75f;
        text.fontSizeMax = size;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.rectTransform.sizeDelta = bounds;
        text.rectTransform.anchoredPosition = position;
        text.outlineColor = new Color(0.025f, 0.025f, 0.08f, 1);
        text.outlineWidth = 0.15f;
        return text;
    }

    public void PlayRandomTextAnimation(int finalValue, float duration = 3.4f)
    {
        rollingTween?.Kill();
        valueText.color = Color.white;
        resultText.text = "행동 선언";
        float elapsed = 0, nextChange = 0;
        rollingTween = DOTween.To(() => elapsed, value =>
        {
            elapsed = value;
            if (elapsed < nextChange) return;
            nextChange = elapsed + 0.055f;
            valueText.text = Random.Range(0, 101).ToString();
        }, duration, duration).SetEase(Ease.Linear).OnComplete(() => valueText.text = finalValue.ToString());
    }

    public void ShowResult(int value, string description)
    {
        rollingTween?.Kill();
        valueText.text = value.ToString();
        valueText.color = value <= 10 ? new Color(1, 0.45f, 0.5f) : value >= 91 ? new Color(1, 0.86f, 0.45f) : Color.white;
        resultText.text = description;
    }

    void OnDisable() { rollingTween?.Kill(); }
}
