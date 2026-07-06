using DG.Tweening;
using TMPro;
using UnityEngine;

public class SayaDiceRollingUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro valueText;
    private Tween rollingTween;

    void Awake()
    {
        PlayRandomTextAnimation(90);
    }

    public void PlayRandomTextAnimation(int finalValue)
    {
        rollingTween?.Kill();

        float duration = 3.4f;
        float changeInterval = 0.05f;

        int dummyValue = 0;
        float nextChangeTime = 0f;

        rollingTween = DOTween.To(() => 0f, x =>
        {
            if (x < nextChangeTime)
                return;

            nextChangeTime += changeInterval;

            dummyValue = Random.Range(0, 101);
            valueText.text = dummyValue.ToString();
        }, duration, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            valueText.text = finalValue.ToString();
        });
    }
}
