using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    public GameObject infoWindow;
    public Button saleBtn;
    public Button exitBtn;
    public Button skillBtn;
    [Space]
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;
    [SerializeField] Image unitPortrait;
    [SerializeField] Image unitShadow;
    [SerializeField] Image unitClassTypeBackGround;
    [SerializeField] Image unitBackGround;
    [SerializeField] Image attributeIcon;
    [Space]
    [SerializeField] TMP_Text unitNameText;
    [SerializeField] TMP_Text unitLevelText;
    [SerializeField] TMP_Text unitDamageText;
    [SerializeField] TMP_Text unitSkillValueText;
    [SerializeField] TMP_Text unitHealthText;
    [SerializeField] TMP_Text unitShieldText;
    [SerializeField] TMP_Text unitAbilityText;
    [SerializeField] TMP_Text unitDefneceText;
    [SerializeField] TMP_Text unitAttackSpeedText;
    [SerializeField] TMP_Text unitCriticalPerText;
    [SerializeField] TMP_Text unitCriticalDamageText;
    [SerializeField] TMP_Text unitCoolTimeText;
    [SerializeField] TMP_Text unitRangeText;
    [SerializeField] TMP_Text unitFactionText;
    [SerializeField] TMP_Text unitClassTypeText;
    [SerializeField] TMP_Text unitDescText;
    [SerializeField] TMP_Text attributeNameText;
    [Space]
    [SerializeField] Scrollbar descScrollBar;
    [SerializeField] Toggle unitStateTabToggles;
    [SerializeField] TabManager tabManager;
    [SerializeField] GridLayoutGroup unitStatGrid;
    [SerializeField] ContentSizeFitter unitDescFitter;
    [Space]
    [Space(10)]
    [SerializeField] GameObject unitDescGameobject;
    [SerializeField] GameObject skillDescGameobject;
    [SerializeField] GridLayoutGroup skillDescGrid;
    [SerializeField] ContentSizeFitter skillTogglesFitter;
    [SerializeField] ContentSizeFitter skillDescFitter;
    [SerializeField] RectTransform skillToggleGroup;
    [SerializeField] UnitSkillDescriptionToggle[] descriptionToggles;
    [SerializeField] Toggle unitDescToggle;
    [Space]
    public GameObject currentUnit;
    public GameObject lastUnit;

    Coroutine updateRoutine;

    void Awake()
    {
        foreach (var toggle in descriptionToggles)
        {
            toggle.GetComponent<Toggle>().onValueChanged.AddListener(_ => RefreshSkillUI());
        }
        unitDescToggle.onValueChanged.AddListener(_ => RefreshSkillUI());
    }

    public void OnHideInfoUI()
    {
        unitDescText.text = "";
        for (int i = 0; i < descriptionToggles.Length; i++)
        {
            if (descriptionToggles[i].GetComponent<Toggle>().isOn)
            {
                descriptionToggles[i].GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                break;
            }
        }
        unitDescGameobject.SetActive(true);
        skillDescGameobject.SetActive(false);
        UnitInfo.UnitType unitType = currentUnit.GetComponentInChildren<UnitInfo>().unitType;
        switch (unitType)
        {
            case UnitInfo.UnitType.Unit:
                Unit selectUnit = currentUnit.GetComponent<Unit>();

                selectUnit.unitDrag.isClicked = false;
                selectUnit.unitInfo.ToggleUnitUI(selectUnit.unitDrag.isClicked, selectUnit.gameObject);

                for (int i = 0; i < FieldManager.Inst.fieldUnits.Length; i++)
                {
                    if (FieldManager.Inst.fieldUnits[i] != null)
                    {
                        if (FieldManager.Inst.fieldUnits[i] == selectUnit.gameObject)
                        {
                            FieldManager.Inst.fieldUnits[i].GetComponent<Unit>().sp.sortingOrder = i + 20;
                            break;
                        }
                    }
                }
                break;
            case UnitInfo.UnitType.SephiraUnit:
                SephiraUnit selectSephiraUnit = currentUnit.GetComponent<SephiraUnit>();

                selectSephiraUnit.isClicked = false;
                selectSephiraUnit.unitInfo.ToggleUnitUI(selectSephiraUnit.isClicked, selectSephiraUnit.gameObject);
                break;
        }

    }

    public void OnSaleUnit()
    {
        Unit selectUnit = FieldManager.Inst.selectUnit.GetComponent<Unit>();

        ApplySaleReward(selectUnit);
        selectUnit.UnitDeActive();
    }

    public void ApplySaleReward(Unit unit)
    {
        int reward = unit.unitLevel.GetSaleRewardByLevel();
        var resourceType = unit.unitLevel.GetSaleResourceByLevel();

        switch (resourceType)
        {
            case ResourcesData.ResourcesType.coin:
                GameManager.Inst.ModifyCoin(reward);
                InGameUIManager.Inst.ShowResourceGainEffect(unit.transform, reward, ResourcesData.ResourcesType.coin);
                break;
            case ResourcesData.ResourcesType.darkMatteCoin:
                GameManager.Inst.ModifyDarkMatteCoin(reward);
                InGameUIManager.Inst.ShowResourceGainEffect(unit.transform, reward, ResourcesData.ResourcesType.darkMatteCoin);
                break;
            default:
                Debug.LogError("Null Resource Type!");
                break;
        }

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.CoinSell);
    }


    public void OnActivateSkill()
    {
        Unit selectUnit = FieldManager.Inst.selectUnit.GetComponent<Unit>();

        if (selectUnit.unitSkill is IButtonAttckPassive buttonAttckPassive)
        {
            buttonAttckPassive.ApplyButtonAttckPassive(selectUnit);
        }

        if (selectUnit.unitSkill is IButtonAttckSkill buttonAttckSkill)
        {
            buttonAttckSkill.ApplyButtonAttckSkill(selectUnit);
        }

        if (selectUnit.unitSkill is IButtonPassive buttonPassive)
        {
            buttonPassive.ApplyButtonPassive(selectUnit);
        }

        if (selectUnit.unitSkill is IButtonInfoinitialize buttonInfoinitialize)
        {
            buttonInfoinitialize.InitializeSkillButtonUI(selectUnit);
        }

    }

    public void InitInfo(Unit unit, SephiraUnit sephiraUnit, UnitInfo.UnitType unitType)
    {
        StartCoroutine(SetScrollbar());

        tabManager.toggles[0].GetComponent<Toggle>().isOn = true;

        if (updateRoutine != null)
            StopCoroutine(updateRoutine);

        updateRoutine = StartCoroutine(UpdateUnitInfoRoutine(unit, sephiraUnit, unitType));
    }

    private IEnumerator UpdateUnitInfoRoutine(Unit unit, SephiraUnit sephiraUnit, UnitInfo.UnitType unitType)
    {
        UnitData data = unitType == UnitInfo.UnitType.Unit ? unit.unitData : sephiraUnit.unitData;

        RectTransform portraitRect = unitPortrait.GetComponent<RectTransform>();

        portraitRect.sizeDelta = new Vector2(portraitWidth * data.portraitScaleFactor, portraitHeight * data.portraitScaleFactor);
        portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * data.portraitPosXFactor, (1 + portraitPosY) * data.portraitPosYFactor);

        unitPortrait.sprite = data.portrait;
        unitShadow.color = data.classColorType[data.unitClass];
        unitClassTypeBackGround.color = data.classColorType[data.unitClass];
        unitBackGround.color = data.classColorType[data.unitClass];

        unitNameText.text = data.unitName;
        unitLevelText.text = "Lv." + data.cardLevel;
        unitFactionText.text = data.factionToKorean[data.faction];
        unitClassTypeText.text = data.classToKorean[data.unitClass] + "/" + data.TypeToKorean(data.type);
        unitDescText.text = data.unitDesc;

        unitCoolTimeText.transform.parent.gameObject.SetActive(unitType == UnitInfo.UnitType.Unit);
        unitAbilityText.transform.parent.gameObject.SetActive(unitType == UnitInfo.UnitType.SephiraUnit);

        attributeNameText.text = data.attribute.attributeName;
        attributeIcon.sprite = data.attribute.icon;

        for (int i = 0; i < descriptionToggles.Length; i++)
        {
            descriptionToggles[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < data.skillActivationInfo.Length; i++)
        {
            descriptionToggles[i].gameObject.SetActive(true);
            descriptionToggles[i].Init(data, i);
        }

        int targetX = data.skillActivationInfo.Length > 5 ? 20 : 100;
        skillToggleGroup.anchoredPosition = new Vector2(skillToggleGroup.anchoredPosition.x, targetX);

        var sorted = descriptionToggles
      .OrderBy(t => t.GetComponent<UnitSkillDescriptionToggle>().activationType)
      .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].transform.SetSiblingIndex(i);
        }

        descriptionToggles = sorted.ToArray();

        GameUIManager.Inst.RefreshLayoutOnce(unitStatGrid);
        //GameStatManager.Inst.MarkStatsDirty();
        if (unitType == UnitInfo.UnitType.Unit)
        {
            unit.RecalculateStats();
        }
        else
        {
            sephiraUnit.RecalculateStats();
        }
        while (gameObject.activeInHierarchy)
        {
            if (unitType == UnitInfo.UnitType.Unit)
            {
                unitDamageText.text = unit.damageAmout.ToString("F1");
                unitSkillValueText.text = unit.skillAmount.ToString("F2");
                unitHealthText.text = Mathf.Round(unit.maxHealth).ToString();
                unitShieldText.text = Mathf.Round(unit.maxShield).ToString();
                unitDefneceText.text = unit.defenseAmount.ToString("0.0");
                unitAttackSpeedText.text = unit.attackSpeed.ToString("0.00");
                unitCriticalPerText.text = Mathf.Round(unit.criticalPer).ToString() + "%";
                unitCriticalDamageText.text = Mathf.Round(unit.criticalDamage).ToString() + "%";
                unitCoolTimeText.text = unit.coolTime.ToString("0.0");
                unitRangeText.text = unit.range.ToString("0.0");
            }
            else
            {
                unitDamageText.text = sephiraUnit.damageAmout.ToString("F1");
                unitSkillValueText.text = sephiraUnit.skillAmount.ToString("F2");
                unitHealthText.text = Mathf.Round(sephiraUnit.maxHealth).ToString();
                unitShieldText.text = Mathf.Round(sephiraUnit.maxShield).ToString();
                unitAbilityText.text = sephiraUnit.maxAbllityCount.ToString("0.0");
                unitDefneceText.text = sephiraUnit.defenseAmount.ToString("0.0");
                unitAttackSpeedText.text = sephiraUnit.attackSpeed.ToString("0.00");
                unitCriticalPerText.text = Mathf.Round(sephiraUnit.criticalPer).ToString() + "%";
                unitCriticalDamageText.text = Mathf.Round(sephiraUnit.criticalDamage).ToString() + "%";
                unitCoolTimeText.text = sephiraUnit.maxAbllityCount.ToString("0.0");
                unitRangeText.text = sephiraUnit.range.ToString("0.0");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SetScrollbar()
    {
        yield return new WaitForEndOfFrame();
        descScrollBar.value = 1f;
    }

    public void RefreshSkillUI()
    {
        GameUIManager.Inst.RefreshLayoutOnce(skillDescGrid);
        GameUIManager.Inst.RefreshFitterOnce(skillTogglesFitter);
        GameUIManager.Inst.RefreshFitterOnce(skillDescFitter);
    }

}
