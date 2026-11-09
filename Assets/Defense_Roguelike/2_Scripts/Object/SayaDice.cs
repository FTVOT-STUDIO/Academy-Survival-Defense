using DG.Tweening;
using UnityEngine;

public class SayaDice : MonoBehaviour
{
    private Tween rotatingTween;

    void OnEnable()
    {
        // FBX의 내장 카메라가 주사위와 함께 회전하며 게임 화면을 덮지 않도록 차단합니다.
        foreach (Camera camera in GetComponentsInChildren<Camera>(true)) camera.enabled = false;
        foreach (AudioListener listener in GetComponentsInChildren<AudioListener>(true)) listener.enabled = false;
        Play(3.4f);
    }

    public void Play(float duration)
    {
        rotatingTween?.Kill();
        rotatingTween = transform.DOLocalRotate(new Vector3(-360, 1440, 360), duration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);
    }

    void OnDisable()
    {
        rotatingTween?.Kill();
    }
}
