using UnityEngine;
using System.Collections;

public class ParticleColorChanger : MonoBehaviour
{
    public ParticleSystem particle;
    public float colorChangeDuration = 1f;

    Coroutine changeColorOverTime;
    Coroutine changeChildColors;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        var mainModule = particle.main;
        mainModule.startColor = Color.white;
    }

    void OnDisable()
    {
        var mainModule = particle.main;
        mainModule.startColor = Color.white;

        if (transform.childCount != 0)
            foreach (Transform child in transform)
            {
                ParticleSystem childParticleSystem = child.GetComponent<ParticleSystem>();
                if (childParticleSystem != null)
                {
                    var childMainModule = childParticleSystem.main;
                    childMainModule.startColor = Color.white;
                }
            }

        if (changeColorOverTime != null)
            StopCoroutine(changeColorOverTime);

        if (changeChildColors != null)
            StopCoroutine(changeChildColors);

    }

    public void ChangeParticleColor(Color newColor)
    {
        changeColorOverTime = StartCoroutine(ChangeColorOverTime(newColor));

        ChangeChildColors(transform, newColor);
    }

    private IEnumerator ChangeColorOverTime(Color targetColor)
    {
        var mainModule = particle.main;

        Color initialColor = mainModule.startColor.color;

        float elapsedTime = 0f;

        while (elapsedTime < colorChangeDuration)
        {
            mainModule.startColor = Color.Lerp(initialColor, targetColor, elapsedTime / colorChangeDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        mainModule.startColor = targetColor;
    }

    private void ChangeChildColors(Transform parent, Color newColor)
    {
        if (parent.childCount == 0) return;

        foreach (Transform child in parent)
        {
            ParticleSystem childParticleSystem = child.GetComponent<ParticleSystem>();
            if (childParticleSystem != null)
            {
                changeChildColors = StartCoroutine(ChangeChildColorOverTime(childParticleSystem, newColor));
            }

            ChangeChildColors(child, newColor);
        }
    }

    private IEnumerator ChangeChildColorOverTime(ParticleSystem childParticleSystem, Color targetColor)
    {
        var mainModule = childParticleSystem.main;
        Color initialColor = mainModule.startColor.color;

        float elapsedTime = 0f;

        while (elapsedTime < colorChangeDuration)
        {
            mainModule.startColor = Color.Lerp(initialColor, targetColor, elapsedTime / colorChangeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainModule.startColor = targetColor;
    }
}
