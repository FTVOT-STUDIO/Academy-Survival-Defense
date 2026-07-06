using UnityEngine;
using DG.Tweening;

public class SayaDice : MonoBehaviour
{
    Tween rotatingTween;
    Tween popTween;

    void OnEnable()
    {
        TryRolling();
    }

    private void TryRolling()
    {
        rotatingTween.Kill();
        popTween.Kill();

        rotatingTween = transform.DORotate(new Vector3(-360, 1440, 360), 0.8f, RotateMode.FastBeyond360).
        SetLoops(4, LoopType.Restart).
        SetEase(Ease.Linear);

        //popTween = transform.DOScale
    }
}
