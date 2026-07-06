using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnitSkillDescriptionManager : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private GameObject unitDescriptionPanel;
    [SerializeField] private Scrollbar descScrollBar;
    private Coroutine setScrollbarCoroutine;
    private Toggle[] toggles;

    void Awake()
    {
        toggles = toggleGroup.GetComponentsInChildren<Toggle>();

        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(_ => OnToggleChanged());
            //toggle.onValueChanged.AddListener(_ => SetScrollbar());
        }

        UpdateUnitDescriptionState();
    }

    private void OnToggleChanged()
    {
        UpdateUnitDescriptionState();
    }

    private void UpdateUnitDescriptionState()
    {
        bool anyToggleOn = false;

        foreach (var toggle in toggles)
        {
            if (toggle.isOn)
            {
                anyToggleOn = true;
                break;
            }
        }
        unitDescriptionPanel.SetActive(!anyToggleOn);
    }
    public void SetScrollbar()
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (setScrollbarCoroutine != null)
            StopCoroutine(setScrollbarCoroutine);

        setScrollbarCoroutine = StartCoroutine(SetScrollbarCoroutine());
    }

    private IEnumerator SetScrollbarCoroutine()
    {
        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
        descScrollBar.value = 1f;
        setScrollbarCoroutine = null;
    }
}
