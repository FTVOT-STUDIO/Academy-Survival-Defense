using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class FieldTabUIInitializer : MonoBehaviour
{
    public enum TabType { portrait, info }
    public enum InfoType { equipmentTab, readOnly }
    public TabType tabType;
    [SerializeField] InfoType infoType;

    [Space(20)]
    [Tab("Core Data")]
    [Header("# Common Var")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [Space(10)]
    [SerializeField] GameObject lockImage;
    [SerializeField] Image portrait;
    [Space(10)]
    [SerializeField] GameObject equippedImage;
    [HideInInspector] public FieldData data;
    [HideInInspector] public bool isUnlock;
    [HideInInspector] public bool isEquipped = false;

    [Tab("Portrait Data")]
    [Header("# Portrait Var")]
    [SerializeField] Button fieldCardBtn;

    [Tab("Info Data")]
    [Header("# Info Var")]
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text upgradeCostText;

    [Space(10)]
    [SerializeField] GameObject upgradeBtnLockImage;
    [SerializeField] GameObject unlockBtn;
    [SerializeField] GameObject unlockBlockImage;
    [SerializeField] Image unlockResourceImage;
    [Space(10)]
    [SerializeField] Button levelUpgradeBtn;
    [SerializeField] Image levelUpgradeResourceImage;
    [SerializeField] Button selectBtn;

    public void Init(FieldData fieldData, TabType tabType, InfoType infoType)
    {
        data = fieldData;
        this.tabType = tabType;
        this.infoType = infoType;

        nameText.text = data.fieldName;
        levelText.text = "Lv " + data.fieldLevel;
        portrait.sprite = data.fieldPortrait;

        lockImage.SetActive(!data.isUnlock);
        equippedImage.SetActive(data.isEquipped);

        switch (tabType)
        {
            case TabType.info:

                descText.text = string.Format(data.desc, (data.value * 100).ToString("F1"));

                unlockBtn.SetActive(!data.isUnlock);
                unlockResourceImage.sprite = data.unlockResourceTypeInfo.icon;

                if (!data.isUnlock)
                {
                    unlockBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                    unlockBtn.GetComponent<Button>().onClick.AddListener(() => UnlockFieldButton());
                    unlockBtn.GetComponentInChildren<TextMeshProUGUI>().text = data.unlockCost.ToString();

                    switch (data.unlockResourceTypeInfo.resourcesType)
                    {
                        case ResourcesData.ResourcesType.gold:
                            unlockBtn.GetComponent<Button>().interactable = PlayerManager.Inst.gold >= data.unlockCost;
                            unlockBtn.GetComponentInChildren<TextMeshProUGUI>().color = PlayerManager.Inst.gold >= data.unlockCost ? Color.white : Color.red;
                            unlockBlockImage.SetActive(!(PlayerManager.Inst.gold >= data.unlockCost));
                            break;
                        case ResourcesData.ResourcesType.gem:
                            unlockBtn.GetComponent<Button>().interactable = PlayerManager.Inst.gem >= data.unlockCost;
                            unlockBtn.GetComponentInChildren<TextMeshProUGUI>().color = PlayerManager.Inst.gem >= data.unlockCost ? Color.white : Color.red;
                            unlockBlockImage.SetActive(!(PlayerManager.Inst.gem >= data.unlockCost));
                            break;
                    }
                }

                levelUpgradeBtn.gameObject.SetActive(data.isUnlock);
                levelUpgradeResourceImage.sprite = data.upgradeResourceTypeInfo.icon;
                selectBtn.gameObject.SetActive(data.isUnlock && infoType == InfoType.equipmentTab);

                levelUpgradeBtn.interactable = !(data.fieldLevel >= 10 || PlayerManager.Inst.gold <= data.upgradeCostTable[data.fieldLevel]);
                upgradeBtnLockImage.SetActive(data.fieldLevel >= 10 || PlayerManager.Inst.gold <= data.upgradeCostTable[data.fieldLevel]);

                upgradeCostText.text = data.fieldLevel < 10 ? data.upgradeCostTable[data.fieldLevel].ToString() : "MAX";
                upgradeCostText.color = PlayerManager.Inst.gold >= data.upgradeCostTable[data.fieldLevel - 1] ? Color.white : Color.red;



                levelUpgradeBtn.onClick.RemoveAllListeners();
                levelUpgradeBtn.onClick.AddListener(() => LevelUpgradeField(fieldData));
                break;
        }
    }

    public void SetEquippedStatus()
    {
        isEquipped = data.isEquipped;

        equippedImage.SetActive(isEquipped);

    }

    public void UnlockFieldButton()
    {
        bool canUnlock = false;

        switch (data.unlockResourceTypeInfo.resourcesType)
        {
            case ResourcesData.ResourcesType.gold:
                canUnlock = PlayerManager.Inst.gold >= data.unlockCost;
                break;
            case ResourcesData.ResourcesType.gem:
                canUnlock = PlayerManager.Inst.gem >= data.unlockCost;
                break;
        }

        if (!canUnlock)
            return;

        switch (data.unlockResourceTypeInfo.resourcesType)
        {
            case ResourcesData.ResourcesType.gold:
                PlayerManager.Inst.ModifyGold(-data.unlockCost, false);
                break;
            case ResourcesData.ResourcesType.gem:
                PlayerManager.Inst.ModifyGem(-data.unlockCost, false);
                break;
        }

        PlayerManager.Inst.UnlockField(data);
        GameUIManager.Inst.ShowFleshImage(LobbyManager.Inst.lobbyUiCanvas.transform, Color.white);
        GetComponent<PopUpUI>().OnHideShrink(gameObject);

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.BuyItem);
        DataManager.Inst.SaveData();
    }

    public void LevelUpgradeField(FieldData field)
    {
        if (field.fieldLevel >= 10)
        {
            Debug.Log("max Level");
            return;
        }

        bool canUpgrade = false;
        int cost = field.upgradeCostTable[field.fieldLevel];

        switch (field.upgradeResourceTypeInfo.resourcesType)
        {
            case ResourcesData.ResourcesType.gold:
                canUpgrade = PlayerManager.Inst.gold >= cost;
                break;
            case ResourcesData.ResourcesType.gem:
                canUpgrade = PlayerManager.Inst.gem >= cost;
                break;
        }

        if (!canUpgrade)
            return;

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.lux);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.select4);
        Debug.Log("Level Up!");

        switch (field.upgradeResourceTypeInfo.resourcesType)
        {
            case ResourcesData.ResourcesType.gold:
                PlayerManager.Inst.ModifyGold(-cost, false);
                break;
            case ResourcesData.ResourcesType.gem:
                PlayerManager.Inst.ModifyGem(-cost, false);
                break;
        }

        field.fieldLevel++;

        field.InitFieldState();

        Init(field, TabType.info, infoType);

        DeckManager.Inst.InitializeAllDeckLists();
        if (PlayerManager.Inst.isJointOperationUnlocked)
            DeckManager.Inst.jointOperationsDeckManager.InitializeJointOperationsAllList();

        GameUIManager.Inst.ShowFleshImage(LobbyManager.Inst.lobbyUiCanvas.transform, Color.white);

        DataManager.Inst.SaveData();
    }

    public void ForcingLevelUpgradeField(FieldData field)
    {
        if (field.fieldLevel >= 10)
        {
            Debug.Log("max Level");
            return;
        }

        field.fieldLevel++;

        field.InitFieldState();

        Init(field, TabType.portrait, infoType);

        DeckManager.Inst.InitializeAllDeckLists();
        if (PlayerManager.Inst.isJointOperationUnlocked)
            DeckManager.Inst.jointOperationsDeckManager.InitializeJointOperationsAllList();

        DataManager.Inst.SaveData();
    }

    public void SelectField()
    {
        DeckManager.Inst.curDeckFieldData.isEquipped = false;

        DeckManager.Inst.curDeckFieldData = data;
        DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].fieldData = data;

        data.isEquipped = true;

        DeckManager.Inst.InitializeAllDeckLists();

        EquipmentManager.Inst.UpdateAllEquippedStatus();

        // EquipmentManager.Inst.RefreshAllUnitAndFieldLists();
        //   EquipmentManager.Inst.InitAllUnitAndFieldLists();

        // DeckManager.Inst.GetFactionBonusInfo(DeckManager.Inst.factionBonusText);

        GetComponent<PopUpUI>().OnHideShrink(gameObject);

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.select5);
    }
}
