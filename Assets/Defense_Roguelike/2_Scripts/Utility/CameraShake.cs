using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Inst;
    private CinemachineCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        Inst = this;
        vCam = GetComponent<CinemachineCamera>();
        noise = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(amplitude, frequency, duration));
    }

    private IEnumerator DoShake(float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
}