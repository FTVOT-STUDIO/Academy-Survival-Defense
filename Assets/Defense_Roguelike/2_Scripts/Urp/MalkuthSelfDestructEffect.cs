using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MalkuthBlackholeEffect : MonoBehaviour
{
    [Header("Volume Settings")]
    public Volume volume;
    Bloom bloom;
    Vignette vignette;
    LensDistortion lensDistortion;
    ColorAdjustments colorAdjustments;

    [Header("Settings")]
    public float enterDuration = 1.5f;
    public float holdDuration = 0.5f;
    public float restoreDuration = 1.0f;

    public Vector2 targetCenter = new Vector2(0.3f, 0.3f);
    public float targetScale = 0.2f;
    public float targetIntensity = -1f;
    public float targetXMultiplier = 0.3f;
    public float targetYMultiplier = 0.3f;

    public float targetSaturation = -100f;
    public float targetVignette = 0.7f;
    public float targetBloom = 2f;

    // 초기 상태 저장용
    Vector2 startCenter;
    float startScale;
    float startIntensity;
    float startX;
    float startY;
    float startSat;
    float startVignette;
    float startBloom;

    void Awake()
    {
        if (volume == null)
        {
            Debug.LogError("Volume not assigned.");
            enabled = false;
            return;
        }

        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out lensDistortion);
        volume.profile.TryGet(out colorAdjustments);

        // 초기값 저장
        if (lensDistortion != null)
        {
            startCenter = lensDistortion.center.value;
            startScale = lensDistortion.scale.value;
            startIntensity = lensDistortion.intensity.value;
            startX = lensDistortion.xMultiplier.value;
            startY = lensDistortion.yMultiplier.value;
        }

        if (colorAdjustments != null)
            startSat = colorAdjustments.saturation.value;

        if (vignette != null)
            startVignette = vignette.intensity.value;

        if (bloom != null)
            startBloom = bloom.intensity.value;
    }

    void OnEnable()
    {
        Play();
    }

    void OnDisable()
    {
        StopAllCoroutines();

        if (lensDistortion != null)
        {
            lensDistortion.center.value = startCenter;
            lensDistortion.scale.value = startScale;
            lensDistortion.intensity.value = startIntensity;
            lensDistortion.xMultiplier.value = startX;
            lensDistortion.yMultiplier.value = startY;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = startSat;
        }

        if (vignette != null)
        {
            vignette.intensity.value = startVignette;
        }

        if (bloom != null)
        {
            bloom.intensity.value = startBloom;
        }
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(BlackholeEffectRoutine());
    }

    IEnumerator BlackholeEffectRoutine()
    {
        float elapsed = 0f;

        // 1. 진입: 서서히 왜곡
        while (elapsed < enterDuration)
        {
            float t = elapsed / enterDuration;
            float easeIn = Mathf.Pow(t, 5f);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            lensDistortion.intensity.value = Mathf.Lerp(0f, 1f, easeIn);
            lensDistortion.scale.value = Mathf.Lerp(1f, targetScale, easeIn);
            lensDistortion.center.value = Vector2.Lerp(startCenter, targetCenter, easeIn);
            lensDistortion.xMultiplier.value = Mathf.Lerp(startX, targetXMultiplier, easeIn);
            lensDistortion.yMultiplier.value = Mathf.Lerp(startY, targetYMultiplier, easeIn);

            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, smooth);
            colorAdjustments.saturation.value = Mathf.Lerp(startSat, targetSaturation, smooth);
            bloom.intensity.value = Mathf.Lerp(startBloom, targetBloom, smooth);

            elapsed += Time.deltaTime;
            yield return null;
        }

        lensDistortion.intensity.value = targetIntensity;
        GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, Color.white);
        yield return new WaitForSeconds(0.05f);

        float flashDuration = 0.1f;
        float flashPeak = 1.0f;
        float flashElapsed = 0f;

        while (flashElapsed < flashDuration)
        {
            float t = flashElapsed / flashDuration;
            float flashCurve = Mathf.Sin(t * Mathf.PI);

            vignette.intensity.value = Mathf.Lerp(targetVignette, flashPeak, flashCurve);

            flashElapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, Color.black);
    }
}
