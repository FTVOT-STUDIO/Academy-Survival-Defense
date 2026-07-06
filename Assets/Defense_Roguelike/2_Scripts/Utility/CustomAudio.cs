using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using VInspector;

[System.Serializable]
public class SfxWithDelay
{
    public AudioManager.Sfx sfx;
    public float delay;

}
public class CustomAudio : MonoBehaviour
{
    [Tab("# SFX")]
    [SerializeField] SfxWithDelay[] customSfxWithDelays;
    [SerializeField] bool playOnlyOnUserInput = false;
    [Tab("# BGM")]
    [SerializeField] AudioManager.Bgm bgm;
    [SerializeField] bool isBgm = false;
    [Tab("# ENV")]
    [SerializeField] AudioManager.Env env;
    [SerializeField] bool isEnv = false;
    [Tab("# Pref")]
    [SerializeField] bool isEvent = false;

    void OnEnable()
    {
        if (!isEvent)
            PlayCustomSfx();

        if (isBgm)
            PlayCustomBgm(0);

        if (isEnv)
            PlayCustomEvn(0);
    }
    public void PlayCustomSfx()
    {
        if (playOnlyOnUserInput)
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject)
                return;
        }
        if (gameObject.activeInHierarchy)
            StartCoroutine(PlaySfxWithDelay());
    }

    public void PlayCustomBgm(float delay)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(PlayBgmWithDelay(delay));
    }

    public void PlayCustomEvn(float delay)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(PlayEnvWithDelay(delay));
    }

    IEnumerator PlaySfxWithDelay()
    {
        Array.Sort(customSfxWithDelays, (a, b) => a.delay.CompareTo(b.delay));

        foreach (var item in customSfxWithDelays)
        {
            yield return new WaitForSeconds(item.delay);
            AudioManager.Inst.PlaySfx(item.sfx);
        }
    }

    IEnumerator PlayBgmWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Inst.PlayBgm(bgm);
    }

    IEnumerator PlayEnvWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Inst.PlayEnv(env);
    }

}