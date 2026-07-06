using UnityEngine;

public class ParticleParentDisabler : MonoBehaviour
{
    ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        particle.Play();
    }

    void OnParticleSystemStopped()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
