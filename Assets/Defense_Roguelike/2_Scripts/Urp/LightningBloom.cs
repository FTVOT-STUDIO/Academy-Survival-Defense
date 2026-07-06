using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightningBloom : MonoBehaviour
{
    public Volume postProcessVolume;
    private Bloom bloom;

    void Start()
    {
        postProcessVolume.profile.TryGet(out bloom);
        StartCoroutine(LightningRoutine());
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            yield return StartCoroutine(FlashLightning());
        }
    }

    IEnumerator FlashLightning()
    {
        int flashCount = Random.Range(1, 3);
        for (int i = 0; i < flashCount; i++)
        {
            yield return StartCoroutine(AnimateBloom(Random.Range(50, 100), 2.5f, 0.05f));
            yield return StartCoroutine(AnimateBloom(Random.Range(100, 50), 2.5f, 0.1f));
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
    }

    IEnumerator AnimateBloom(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            bloom.intensity.value = Mathf.Lerp(from, to, time / duration);
            bloom.threshold.value = Mathf.Lerp(0.5f, 1, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = to;
        bloom.threshold.value = 1;
    }
}
