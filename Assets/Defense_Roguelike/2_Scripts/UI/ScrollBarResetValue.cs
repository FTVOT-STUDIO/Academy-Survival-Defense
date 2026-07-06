using UnityEngine;
using UnityEngine.UI;

public class ScrollBarResetValue : MonoBehaviour
{
    private Scrollbar scrollbar;

    void Awake()
    {
        scrollbar = GetComponentInChildren<Scrollbar>();
    }

    void OnEnable()
    {
        scrollbar.interactable = true;
    }

    void OnDisable()
    {
        scrollbar.interactable = false;
    }
}
