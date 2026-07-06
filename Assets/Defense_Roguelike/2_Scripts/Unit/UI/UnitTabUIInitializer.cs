using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;


public class UnitTabUIInitializer : MonoBehaviour
{
    public enum TabType { portrait, info }
    public enum InfoType { equipmentTab, readOnly, Shop }
    [Tab("Core Data")]
    [Header("# Common Var")]
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;
    [SerializeField] TabType tabType;
    [SerializeField] InfoType infoType;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text classTypeText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text levelSliderText;
    [SerializeField] Image attributeIcon;
    [Space(10)]
    public GameObject cardLockImage;
    [SerializeField] Image backGround;
    [SerializeField] Image portrait;
    [SerializeField] Image portraitShadow;
    [SerializeField] Image classImage;
    [SerializeField] Image levelUpSliderFill;
    [Space(10)]
    [SerializeField] Slider levelupSlider;
    [Space(10)]
    [SerializeField] GameObject levelUpArrow;
    [Space(10)]
    [SerializeField] GameObject equippedImage;

    public UnitData data;
    [HideInInspector] public bool isUnlock;
    [HideInInspector] public bool isEquipped = false;

    [Tab("Portrait Data")]
    [Header("# Portrait Var")]
    [SerializeField] Button unitCardBtn;

    [Tab("Info Data")]
    [Header("# Info Var")]
    [SerializeField] GameObject equipLockedImage;
    [SerializeField] Scrollbar descScrollBar;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text upgradeDamageText;
    [SerializeField] TMP_Text skillAmountText;
    [SerializeField] TMP_Text upgradeSkillAmountText;
    [SerializeField] TMP_Text defenseText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text upgradeHealthText;
    [SerializeField] TMP_Text shieldText;
    [SerializeField] TMP_Text upgradeShieldText;
    [SerializeField] TMP_Text criticalPerText;
    [SerializeField] TMP_Text upgradeCriticalPerText;
    [SerializeField] TMP_Text criticalDamageText;
    [SerializeField] TMP_Text upgradeCriticalDamageText;
    [SerializeField] TMP_Text attackSpeedText;
    [SerializeField] TMP_Text rangeText;
    [SerializeField] TMP_Text coolTimeText;
    [SerializeField] TMP_Text abilityText;
    [SerializeField] TMP_Text factionText;
    [SerializeField] TMP_Text pathText;
    [SerializeField] TMP_Text moralityText;
    [SerializeField] TMP_Text upgradeCostText;
    [SerializeField] TMP_Text attributeNameText;
    [Space(10)]
    [SerializeField] Image moralityImage;
    [SerializeField] Image resourceImage;
    [Space(10)]
    [SerializeField] GameObject upgradeBtnLockImage;
    public GameObject unlockCheckText;
    [Space(10)]
    [SerializeField] GameObject unitDescGameobject;
    [SerializeField] GameObject skillDescGameobject;
    [SerializeField] GridLayoutGroup skillDescGrid;
    [SerializeField] ContentSizeFitter skillTogglesFitter;
    [SerializeField] ContentSizeFitter skillDescFitter;
    [SerializeField] UnitSkillDescriptionToggle[] descriptionToggles;
    [SerializeField] RectTransform skillToggleGroup;
    [Space(10)]
    public Button levelUpgradeBtn;
    public Button selectBtn;
    public Toggle switchUnitFormToggle;
    [SerializeField] GridLayoutGroup unitStatGrid;
    [SerializeField] ContentSizeFitter unitDescFitter;
    [SerializeField] SephiraPathInfoListController sephiraPathInfoListController;
    [SerializeField] HorizontalLayoutGroup actionButtonLayoutGroup;


    void Awake()
    {
        foreach (var toggle in descriptionToggles)
        {
            toggle.GetComponent<Toggle>().onValueChanged.AddListener(_ => RefreshSkillDesc());
        }
    }

    void OnEnable()
    {
        if (tabType == TabType.info)
            StartCoroutine(SetScrollbar());
    }

    void OnDisable()
    {
        if (tabType == TabType.info)
        {
            descText.text = "";
            switchUnitFormToggle.isOn = false;
            for (int i = 0; i < descriptionToggles.Length; i++)
            {
                if (descriptionToggles[i].GetComponent<Toggle>().isOn)
                {
                    descriptionToggles[i].GetComponent<Toggle>().isOn = false;
                    break;
                }
            }

            unitDescGameobject.SetActive(true);
            skillDescGameobject.SetActive(false);
        }
    }

    public void Init(UnitData unit, TabType tabType, InfoType infoType)
    {
        data = unit;
        data.InitUnitState();
        this.tabType = tabType;
        this.infoType = infoType;
        nameText.text = data.unitName;
        classTypeText.text = data.TypeToKorean(data.type);
        levelText.text = "Lv." + data.cardLevel.ToString();

        data.maxLevelExp = data.expTable[data.unitClass][data.cardLevel];
        levelSliderText.text = data.cardLevel < 15 ? data.curLevelExp + "/" + data.maxLevelExp : "MAX";
        levelUpSliderFill.color = data.cardLevel < 15 ? new Color(0.3f, 1, 0.5f) : Color.red;

        portrait.sprite = data.portrait;
        RectTransform portraitRect = portrait.GetComponent<RectTransform>();
        portraitRect.sizeDelta = new Vector2(portraitWidth * data.portraitScaleFactor, portraitHeight * data.portraitScaleFactor);
        portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * data.portraitPosXFactor, (1 + portraitPosY) * data.portraitPosYFactor);

        portraitShadow.color = data.classColorType[data.unitClass];
        classImage.color = data.classColorType[data.unitClass];
        backGround.color = data.classColorType[data.unitClass];
        levelupSlider.value = data.cardLevel < 15 ? (float)data.curLevelExp / data.maxLevelExp : 1;
        levelUpArrow.SetActive(data.maxLevelExp <= data.curLevelExp && data.cardLevel < unit.expTable[unit.unitClass].Length - 1);
        cardLockImage.SetActive(!data.isUnlock && infoType != InfoType.Shop);
        equippedImage.SetActive(data.isEquipped);
        attributeIcon.sprite = data.attribute.icon;

        SetEquippedStatus();

        switch (tabType)
        {
            case TabType.portrait:
                break;
            case TabType.info:
                descText.text = data.unitDesc;
                damageText.text = data.damageAmount.ToString();
                skillAmountText.text = data.skillAmount.ToString("0.###");
                defenseText.text = data.defenseAmount.ToString() + "%";
                healthText.text = data.maxHealth.ToString();

                shieldText.transform.parent.gameObject.SetActive(data.unitClass != UnitClass.Sephira);
                shieldText.text = data.maxShield.ToString();
                abilityText.transform.parent.gameObject.SetActive(data.unitClass == UnitClass.Sephira);
                abilityText.text = data.coolTime.ToString();

                criticalPerText.text = data.criticalPer.ToString() + "%";
                criticalDamageText.text = data.criticalDamage.ToString() + "%";
                attackSpeedText.text = data.attackSpeed.ToString();
                rangeText.text = data.range.ToString();

                coolTimeText.transform.parent.gameObject.SetActive(data.unitClass != UnitClass.Sephira);
                coolTimeText.text = data.coolTime.ToString();

                factionText.text = data.factionToKorean[data.faction];
                pathText.text = data.sephiraPathData.sephiraName;
                moralityText.text = data.moralityToKorean[data.morality];
                moralityImage.color = data.morality == Morality.Good ? Color.white : new Color(0.5f, 0.5f, 0.5f);

                levelUpgradeBtn.gameObject.SetActive(data.isUnlock && infoType != InfoType.Shop);
                resourceImage.sprite = data.upgradeResourcesData.icon;

                attributeNameText.text = data.attribute.attributeName;

                equipLockedImage.SetActive(data.isEquipLocked);

                selectBtn.gameObject.SetActive(data.isUnlock && infoType == InfoType.equipmentTab);
                unlockCheckText.SetActive(!data.isUnlock && infoType != InfoType.Shop);
                if (GameUIManager.Inst)
                    GameUIManager.Inst.RefreshLayoutOnce(actionButtonLayoutGroup);

                switch (data.upgradeResourcesData.resourcesType)
                {
                    case ResourcesData.ResourcesType.gold:
                        levelUpgradeBtn.interactable = !(data.cardLevel == 15 || data.curLevelExp < data.maxLevelExp || PlayerManager.Inst.gold < unit.upgradeCostTable[unit.unitClass][unit.cardLevel]);
                        upgradeBtnLockImage.SetActive(data.cardLevel == 15 || data.curLevelExp < data.maxLevelExp || PlayerManager.Inst.gold < unit.upgradeCostTable[unit.unitClass][unit.cardLevel]);

                        upgradeCostText.color = PlayerManager.Inst.gold >= unit.upgradeCostTable[unit.unitClass][unit.cardLevel] ? Color.white : Color.red;
                        break;
                    case ResourcesData.ResourcesType.gem:
                        levelUpgradeBtn.interactable = !(data.cardLevel == 15 || data.curLevelExp < data.maxLevelExp || PlayerManager.Inst.gem < unit.upgradeCostTable[unit.unitClass][unit.cardLevel]);
                        upgradeBtnLockImage.SetActive(data.cardLevel == 15 || data.curLevelExp < data.maxLevelExp || PlayerManager.Inst.gem < unit.upgradeCostTable[unit.unitClass][unit.cardLevel]);

                        upgradeCostText.color = PlayerManager.Inst.gem >= unit.upgradeCostTable[unit.unitClass][unit.cardLevel] ? Color.white : Color.red;
                        break;
                }

                switchUnitFormToggle.gameObject.SetActive(data.isTrans);

                upgradeCostText.text = data.cardLevel < 15 ? unit.upgradeCostTable[unit.unitClass][unit.cardLevel].ToString() : "MAX";

                bool isMaxLevel = unit.curLevelExp >= unit.maxLevelExp && unit.cardLevel < 15;

                upgradeDamageText.gameObject.SetActive(isMaxLevel);
                upgradeSkillAmountText.gameObject.SetActive(isMaxLevel);
                upgradeHealthText.gameObject.SetActive(isMaxLevel);
                upgradeShieldText.gameObject.SetActive(isMaxLevel);
                upgradeCriticalPerText.gameObject.SetActive(isMaxLevel);
                upgradeCriticalDamageText.gameObject.SetActive(isMaxLevel);

                levelUpgradeBtn.onClick.RemoveAllListeners();

                if (isMaxLevel)
                {
                    levelUpgradeBtn.onClick.AddListener(() => LevelUpgradeUnit(data));

                    upgradeDamageText.text = "+" + (Math.Round(unit.baseDamage * Mathf.Pow(1.28f, unit.cardLevel)) - unit.damageAmount);
                    upgradeSkillAmountText.text = "+" + unit.skillAmountIncrement;
                    if (data.unitClass == UnitClass.Sephira)
                    {
                        float nextHealth = unit.baseHealth + 50 * unit.cardLevel;
                        upgradeHealthText.text = "+" + (nextHealth - unit.maxHealth);
                    }
                    else
                    {
                        float nextHealth = Mathf.Round(unit.baseHealth * Mathf.Pow(1.15f, unit.cardLevel));
                        upgradeHealthText.text = "+" + (nextHealth - unit.maxHealth);
                    }

                    upgradeShieldText.text = "+" + (Mathf.Round(unit.baseShield * Mathf.Pow(1.1f, unit.cardLevel)) - unit.maxShield);
                    upgradeCriticalPerText.text = "+" + (Mathf.Round(unit.baseCriticalPer * Mathf.Pow(1.1f, unit.cardLevel)) - unit.criticalPer);
                    upgradeCriticalDamageText.text = "+" + (Mathf.Round(unit.baseCriticalDamage * Mathf.Pow(1.1f, unit.cardLevel)) - unit.criticalDamage);
                }

                for (int i = 0; i < descriptionToggles.Length; i++)
                {
                    descriptionToggles[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < data.skillActivationInfo.Length; i++)
                {
                    descriptionToggles[i].gameObject.SetActive(true);
                    descriptionToggles[i].Init(data, i);
                }

                int targetX = data.skillActivationInfo.Length > 5 ? 250 : 300;
                skillToggleGroup.anchoredPosition = new Vector2(skillToggleGroup.anchoredPosition.x, targetX);

                var sorted = descriptionToggles
              .OrderBy(t => t.GetComponent<UnitSkillDescriptionToggle>().activationType)
              .ToList();

                for (int i = 0; i < sorted.Count; i++)
                {
                    sorted[i].transform.SetSiblingIndex(i);
                }

                descriptionToggles = sorted.ToArray();

                GameUIManager.Inst.RefreshLayoutOnce(skillDescGrid);
                GameUIManager.Inst.RefreshFitterOnce(skillDescFitter);
                GameUIManager.Inst.RefreshFitterOnce(skillTogglesFitter);
                GameUIManager.Inst.RefreshLayoutOnce(unitStatGrid);
                GameUIManager.Inst.RefreshFitterOnce(unitDescFitter);

                if (sephiraPathInfoListController != null)
                {
                    sephiraPathInfoListController.Init(data);
                }

                for (int i = 0; i < DataManager.Inst.unitDatas.Count; i++)
                {
                    UnitTabUIInitializer[] tabDataArray = EquipmentManager.Inst.unlockUnitCardList.GetComponentsInChildren<UnitTabUIInitializer>();

                    foreach (var tabData in tabDataArray)
                    {
                        if (tabData.data == data)
                        {
                            tabData.Init(data, TabType.portrait, InfoType.equipmentTab);
                            return;
                        }
                    }
                }

                for (int i = 0; i < DataManager.Inst.sephiraUnitDatas.Count; i++)
                {
                    UnitTabUIInitializer[] tabDataArray = EquipmentManager.Inst.unlockSephiraCardList.GetComponentsInChildren<UnitTabUIInitializer>();

                    foreach (var tabData in tabDataArray)
                    {
                        if (tabData.data == data)
                        {
                            tabData.Init(data, TabType.portrait, InfoType.equipmentTab);
                            return;
                        }
                    }
                }
                break;
        }
    }

    public void SetEquippedStatus()
    {
        for (int i = 0; i < DeckManager.Inst.curDeckUnitDatas.Count; i++)
        {
            isEquipped = data.isEquipped;
        }

        equippedImage.SetActive(isEquipped);
    }

    public void LevelUpgradeUnit(UnitData unit)
    {
        if (unit.cardLevel >= unit.expTable[unit.unitClass].Length - 1)
        {
            Debug.Log("max Level");
            return;
        }

        bool canUpgrade = false;
        int cost = unit.upgradeCostTable[unit.unitClass][unit.cardLevel];

        if (unit.maxLevelExp <= unit.curLevelExp)
        {
            switch (data.upgradeResourcesData.resourcesType)
            {
                case ResourcesData.ResourcesType.gold:
                    canUpgrade = PlayerManager.Inst.gold >= cost;
                    break;
                case ResourcesData.ResourcesType.gem:
                    canUpgrade = PlayerManager.Inst.gem >= cost;
                    break;
            }
        }

        if (canUpgrade)
        {
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.lux);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.select4);
            Debug.Log("Level Up!");

            switch (data.upgradeResourcesData.resourcesType)
            {
                case ResourcesData.ResourcesType.gold:
                    PlayerManager.Inst.ModifyGold(-unit.upgradeCostTable[unit.unitClass][unit.cardLevel], false);
                    break;
                case ResourcesData.ResourcesType.gem:
                    PlayerManager.Inst.ModifyGem(-unit.upgradeCostTable[unit.unitClass][unit.cardLevel], false);
                    break;
            }


            unit.cardLevel++;
            unit.curLevelExp -= unit.maxLevelExp;
            unit.maxLevelExp = unit.expTable[unit.unitClass][unit.cardLevel];

            unit.InitUnitState();

            Init(unit, TabType.info, infoType);

            GameUIManager.Inst.ShowFleshImage(LobbyManager.Inst.lobbyUiCanvas.transform, Color.white);

            DeckManager.Inst.InitializeAllDeckLists();
            if (PlayerManager.Inst.isJointOperationUnlocked)
                DeckManager.Inst.jointOperationsDeckManager.InitializeJointOperationsAllList();

            DataManager.Inst.SaveData();
        }
    }

    public void ForcingLevelUpgradeUnit(UnitData unit)
    {
        if (unit.cardLevel >= unit.expTable[unit.unitClass].Length - 1)
        {
            Debug.Log("max Level");
            return;
        }

        unit.cardLevel++;
        unit.maxLevelExp = unit.expTable[unit.unitClass][unit.cardLevel];

        unit.InitUnitState();

        Init(unit, TabType.portrait, infoType);

        DeckManager.Inst.InitializeAllDeckLists();
        if (PlayerManager.Inst.isJointOperationUnlocked)
            DeckManager.Inst.jointOperationsDeckManager.InitializeJointOperationsAllList();

        DataManager.Inst.SaveData();
    }

    public void OnSelect()
    {
        UnitData curData = data;

        while (true)
        {
            if (!curData.isTrans)
            {
                data = curData;
                break;
            }


            if (curData.isTrans && curData.isBaseForm)
            {
                data = curData;
                break;
            }

            if (curData.isTrans && curData.transData.isBaseForm)
            {
                data = curData.transData;
                break;
            }

            curData = curData.transData;
        }
        GameObject deckList = null;

        if (EquipmentManager.Inst.unitTab.activeInHierarchy)
        {
            deckList = DeckManager.Inst.equipmentUnitDeckList;
            for (int i = 0; i < DeckManager.Inst.equipmentUnitLists.Length; i++)
            {
                DeckManager.Inst.equipmentUnitLists[i].equipArrowImage.SetActive(true);
            }
        }
        if (EquipmentManager.Inst.sephiraTab.activeInHierarchy)
        {
            deckList = DeckManager.Inst.equipmentSephiraDeckList;
            DeckManager.Inst.equipmentSephiraLists.equipArrowImage.SetActive(true);
        }

        UnitTabUIInitializer unitTabUI = DeckManager.Inst.selectUnitParent.GetComponentInChildren<UnitTabUIInitializer>();
        UnitData selectData = data;

        unitTabUI.Init(data, TabType.portrait, InfoType.equipmentTab);
        DeckManager.Inst.isSelected = true;
        DeckManager.Inst.selectData = selectData;

        DeckManager.Inst.selectUI.SetActive(true);

        EquipmentManager.Inst.VerticalScrollBar.value = 1;

        deckList.transform.SetParent(DeckManager.Inst.selectDeckListGroup.transform, true);
        DeckManager.Inst.equipmentdeckToggle.transform.SetParent(DeckManager.Inst.selectDeckListGroup.transform, true);


        EquipmentManager.Inst.unitInfoWindow.SetActive(false);

    }

    public void ToggleUnitFormInfo()
    {
        if (switchUnitFormToggle.isOn)
            Init(data.transData, tabType, infoType);
        else
            Init(data.transData, tabType, infoType);

        for (int i = 0; i < descriptionToggles.Length; i++)
        {
            if (descriptionToggles[i].GetComponent<Toggle>().isOn)
            {
                descriptionToggles[i].GetComponent<Toggle>().isOn = false;
                break;
            }
        }

        unitDescGameobject.SetActive(true);
        skillDescGameobject.SetActive(false);

        //selectBtn.gameObject.SetActive(!switchUnitFormToggle.isOn && infoType == InfoType.equipmentTab && data.isUnlock);
    }

    IEnumerator SetScrollbar()
    {
        Canvas.ForceUpdateCanvases();
        yield return new WaitForEndOfFrame();
        descScrollBar.value = 1f;
        Canvas.ForceUpdateCanvases();
    }

    public void RefreshSkillDesc()
    {
        if (GameUIManager.Inst == null)
            return;
        GameUIManager.Inst.RefreshLayoutOnce(skillDescGrid);
        GameUIManager.Inst.RefreshFitterOnce(skillDescFitter);
    }
}
