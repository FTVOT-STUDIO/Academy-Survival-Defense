using DG.Tweening;
using NUnit.Framework;
using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    private bool isPlayed;
    [Header("Toggle Option")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] float duration = 0.3f;

    public void OnShowShrink(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(Vector3.one, duration).
        SetEase(Ease.OutBack).
        SetUpdate(true).
        OnPlay(()
         =>
        { gameObject.SetActive(true); });
    }

    public void OnHideShrink(GameObject gameObject)
    {
        if (isPlayed)
            return;

        isPlayed = true;

        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.DOScale(Vector3.zero, duration).
        SetEase(Ease.InBack).
        SetUpdate(true).
        OnComplete(()
         =>
        {
            gameObject.SetActive(false);
            isPlayed = false;
        });
    }

    public void OnShowFold(GameObject gameObject)
    {
        gameObject.transform.localScale = new Vector3(1, 0, 1);
        gameObject.transform.DOScale(Vector3.one, duration).
        SetEase(Ease.OutBack).
        SetUpdate(true).
        OnPlay(()
         =>
        { gameObject.SetActive(true); });
    }

    public void OnHideFold(GameObject gameObject)
    {
        if (isPlayed)
            return;

        isPlayed = true;

        gameObject.transform.DOScale(new Vector3(1, 0, 1), duration).
        SetEase(Ease.InBack).
        SetUpdate(true).
        OnComplete(()
         =>
        {
            gameObject.SetActive(false);
            isPlayed = false;
        });
    }

    public void OnFoldToggle(bool isOn)
    {
        if (isOn)
        {
            OnShowFold(targetObject);
        }
        else
        {
            OnHideFold(targetObject);
        }
    }

    public void OnShrinkToggle(bool isOn)
    {
        if (isOn)
        {
            OnShowShrink(targetObject);
        }
        else
        {
            OnHideShrink(targetObject);
        }
    }

    public void PopUpText(GameObject gameObject)
    {
        if (isPlayed)
            return;

        isPlayed = true;
        gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        gameObject.transform.DOScale(Vector3.one, duration).
        SetEase(Ease.OutBack).
        OnComplete(()
        =>
        {
            isPlayed = false;
        });
    }
}
