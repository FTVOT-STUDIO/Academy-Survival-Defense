using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    float moveDistance = 1;
    float duration = 1.5f;

    TMP_Text text;
    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }
    void OnEnable()
    {
        transform.DOScale(Vector3.one, 0.3f).
                     SetEase(Ease.OutBack).
                    SetUpdate(true).
                    From(new Vector3(1.6f, 1.6f, 1.6f));

        Vector3 startPos = transform.position;

        transform.DOMoveY(startPos.y + moveDistance, duration).SetEase(Ease.OutQuad);

        text.DOFade(0, duration).OnComplete(() => DeActive());
    }

    public void InitializeText(string text, float size, Color color)
    {
        this.text.text = text;
        this.text.fontSize = size;
        this.text.color = color;
    }

    public void DeActive()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        text.color = Color.white;
    }
}
