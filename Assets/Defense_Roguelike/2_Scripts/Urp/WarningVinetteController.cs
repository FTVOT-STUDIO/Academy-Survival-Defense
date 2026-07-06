using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WarningVinetteController : MonoBehaviour
{
    [SerializeField] Volume volume;
    [SerializeField] float blinkSpeed;
    [SerializeField] float minIntensity;
    [SerializeField] float maxIntensity;

    bool isBlinking = false;
    [SerializeField] float timer = 0;
    bool fadingIn = true;

    Vignette vignette;

    void Awake()
    {
        if (volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0;
        }

        volume.enabled = false;
    }

    void Update()
    {
        if (!isBlinking)
            return;

        timer += Time.deltaTime * blinkSpeed;

        if (fadingIn)
        {
            vignette.intensity.value = Mathf.Lerp(minIntensity, maxIntensity, timer);
            if (timer >= 1f)
            {
                fadingIn = false;
                timer = 0f;
            }
        }
        else
        {
            vignette.intensity.value = Mathf.Lerp(maxIntensity, minIntensity, timer);
            if (timer >= 1f)
            {
                fadingIn = true;
                timer = 0f;
            }
        }

    }

    public void StartBlink()
    {
        if (isBlinking)
            return;

        volume.enabled = true;
        isBlinking = true;
        timer = 0f;
        fadingIn = true;
    }

    public void StopBlink()
    {
        if (!isBlinking)
            return;

        isBlinking = false;
        vignette.intensity.value = 0f;
        volume.enabled = false;
    }


}
