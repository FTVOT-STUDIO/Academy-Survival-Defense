using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Inst;
    [Space]
    [Tab("Tab")]
    [Header("# Tab Group")]
    [SerializeField] GameObject equimentTab;
    public GameObject equipmentScrollView;
    public GameObject unitTab;
    public GameObject sephiraTab;
    public GameObject fieldTab;
    public GameObject tabToggleGroup;
    public RectTransform tabContentRect;
    [Tab("Card List")]
    [Header("# Card List Groups")]
    public GameObject unlockUnitCardList;
    public GameObject lockUnitCardList;
    [Space]
    public GameObject unlockSephiraCardList;
    public GameObject lockSephiraCardList;
    [Space]
    public GameObject unlockFieldCardList;
    public GameObject lockFieldCardList;
    [Space]
    [SerializeField] VerticalLayoutGroup unitLayoutGroup;
    [SerializeField] VerticalLayoutGroup sephiraLayoutGroup;
    [SerializeField] VerticalLayoutGroup fieldLayoutGroup;
    [Space]
    [SerializeField] GridLayoutGroup unitUnlockGrid;
    [SerializeField] GridLayoutGroup unitLockGrid;
    [Space]
    [SerializeField] GridLayoutGroup sephiraUnlockGrid;
    [SerializeField] GridLayoutGroup sephiraLockGrid;
    [Space]
    [SerializeField] GridLayoutGroup fieldUnLockGrid;
    [SerializeField] GridLayoutGroup fieldLockGrid;
    [Space]
    [SerializeField] LayoutElement unitTabElement;
    [SerializeField] LayoutElement sephiraTabElement;
    [SerializeField] LayoutElement fieldTabElement;

    [Tab("Tab List")]
    [Header("# Tab List Groups")]
    public List<GameObject> unlockUnitTabItems = new List<GameObject>();
    public List<GameObject> unlockSephiraTabItems = new List<GameObject>();
    public List<GameObject> unlockFieldTabItems = new List<GameObject>();
    [Space]
    public List<GameObject> lockUnitTabItems = new List<GameObject>();
    public List<GameObject> lockSephiraTabItems = new List<GameObject>();
    public List<GameObject> lockFieldTabItems = new List<GameObject>();
    [Tab("ETC")]
    [Header("# ETC")]
    public Scrollbar VerticalScrollBar;
    public GameObject unitInfoWindow;
    public GameObject fieldInfoWindow;

    void Awake()
    {
        Inst = this;
    }

    public void OnClickResizeContent(int index)
    {
        switch (index)
        {
            case 0:
                RefreshUnitContentSize(tabContentRect, DataManager.Inst.unitDatas);
                break;
            case 1:
                RefreshUnitContentSize(tabContentRect, DataManager.Inst.sephiraUnitDatas);
                break;
            case 2:
                RefreshFieldContentSize(tabContentRect, DataManager.Inst.fieldDatas);
                break;
        }

    }

    private IEnumerator RefreshUnitContentSizeCoroutine(RectTransform tabRect, List<UnitData> dataList)
    {
        yield return new WaitForEndOfFrame();
        tabRect.sizeDelta = new Vector2(tabRect.sizeDelta.x, 1400);

        List<UnitData> unlockDataList = dataList.Where(unit => unit.isUnlock).ToList();
        List<UnitData> lockDataList = dataList.Where(unit => !unit.isUnlock).ToList();

        int unlockedUnitCount = unlockDataList.Count;
        int lockedUnitCount = lockDataList.Count;

        int unitsPerRow = 4;

        float unitHeight = 350;

        int unlockedRows = Mathf.CeilToInt((float)unlockedUnitCount / unitsPerRow);

        int lockedRows = Mathf.CeilToInt((float)lockedUnitCount / unitsPerRow);

        float totalHeight = (unlockedRows + lockedRows) * unitHeight;

        tabRect.sizeDelta = new Vector2(tabRect.sizeDelta.x, tabRect.sizeDelta.y + totalHeight);

        RefreshAllUnitAndSephiraLayouts();
    }

    private IEnumerator RefreshFieldContentSizeCoroutine(RectTransform tabRect, List<FieldData> dataList)
    {
        yield return new WaitForEndOfFrame();
        tabRect.sizeDelta = new Vector2(tabRect.sizeDelta.x, 1400);

        List<FieldData> unlockFieldList = dataList.Where(f => f.isUnlock).ToList();
        List<FieldData> lockFieldList = dataList.Where(f => !f.isUnlock).ToList();

        int unlockedFieldCount = unlockFieldList.Count;
        int lockedFieldCount = lockFieldList.Count;

        int unitsPerRow = 2;

        float unitHeight = 450;

        int unlockedRows = Mathf.CeilToInt((float)unlockedFieldCount / unitsPerRow);

        int lockedRows = Mathf.CeilToInt((float)lockedFieldCount / unitsPerRow);

        float totalHeight = (unlockedRows + lockedRows) * unitHeight;

        tabRect.sizeDelta = new Vector2(tabRect.sizeDelta.x, tabRect.sizeDelta.y + totalHeight);

        RefreshAllFieldLayouts();
    }

    public void RefreshUnitContentSize(RectTransform tabRect, List<UnitData> dataList)
    {
        StartCoroutine(RefreshUnitContentSizeCoroutine(tabRect, dataList));
    }

    public void RefreshFieldContentSize(RectTransform tabRect, List<FieldData> dataList)
    {
        StartCoroutine(RefreshFieldContentSizeCoroutine(tabRect, dataList));
    }

    void GenerateUnitList(List<UnitData> unitDatas, List<GameObject> unlockItems, List<GameObject> lockItems, Transform unlockCardList, Transform lockCardList)
    {
        List<UnitData> unlockedUnits = unitDatas.Where(u => u.isUnlock).OrderBy(u => u.unitClass).ToList();
        List<UnitData> lockedUnits = unitDatas.Where(u => !u.isUnlock).OrderBy(u => u.unitClass).ToList();

        if (unlockItems != null)
        {
            for (int i = 0; i < unlockItems.Count; i++)
            {
                unlockItems[i].transform.SetParent(PoolManager.Inst.transform, false);
                unlockItems[i].gameObject.SetActive(false);
            }
        }

        if (lockItems != null)
        {
            for (int i = 0; i < lockItems.Count; i++)
            {
                lockItems[i].transform.SetParent(PoolManager.Inst.transform, false);
                lockItems[i].gameObject.SetActive(false);
            }
        }

        unlockItems.Clear();
        lockItems.Clear();

        foreach (var unit in unlockedUnits)
        {
            GameObject unitTabItem = PoolManager.Inst.Get(PoolManager.PrefabType.unitTabPrefab, transform.position);
            Button button = unitTabItem.GetComponent<Button>();

            unitTabItem.transform.SetParent(unlockCardList.transform, false);
            unitTabItem.GetComponent<UnitTabUIInitializer>().Init(unit, UnitTabUIInitializer.TabType.portrait, UnitTabUIInitializer.InfoType.equipmentTab);
            unitTabItem.GetComponent<UnitTabUIInitializer>().SetEquippedStatus();

            AddUnitButtonClickListener(button, unit, UnitTabUIInitializer.InfoType.equipmentTab);

            unlockItems.Add(unitTabItem);
        }

        foreach (var unit in lockedUnits)
        {
            GameObject unitTabItem = PoolManager.Inst.Get(PoolManager.PrefabType.unitTabPrefab, transform.position);
            Button button = unitTabItem.GetComponent<Button>();

            unitTabItem.transform.SetParent(lockCardList.transform, false);
            unitTabItem.GetComponent<UnitTabUIInitializer>().Init(unit, UnitTabUIInitializer.TabType.portrait, UnitTabUIInitializer.InfoType.readOnly);

            AddUnitButtonClickListener(button, unit, UnitTabUIInitializer.InfoType.equipmentTab);

            lockItems.Add(unitTabItem);
        }

        RefreshAllUnitAndSephiraLayouts();
    }

    void GenerateFieldList()
    {
        List<FieldData> unlockedFields = DataManager.Inst.fieldDatas.Where(f => f.isUnlock).ToList();
        List<FieldData> lockedFields = DataManager.Inst.fieldDatas.Where(f => !f.isUnlock).ToList();

        if (unlockFieldTabItems != null)
        {
            for (int i = 0; i < unlockFieldTabItems.Count; i++)
            {
                unlockFieldTabItems[i].transform.SetParent(PoolManager.Inst.transform, false);
                unlockFieldTabItems[i].gameObject.SetActive(false);
            }
        }

        if (lockFieldTabItems != null)
        {
            for (int i = 0; i < lockFieldTabItems.Count; i++)
            {
                lockFieldTabItems[i].transform.SetParent(PoolManager.Inst.transform, false);
                lockFieldTabItems[i].gameObject.SetActive(false);
            }
        }

        unlockFieldTabItems.Clear();
        lockFieldTabItems.Clear();

        foreach (var field in unlockedFields)
        {
            GameObject fieldTabItem = PoolManager.Inst.Get(PoolManager.PrefabType.fieldTabPrefab, transform.position);
            Button button = fieldTabItem.GetComponent<Button>();

            fieldTabItem.transform.SetParent(unlockFieldCardList.transform, false);
            fieldTabItem.GetComponent<FieldTabUIInitializer>().Init(field, FieldTabUIInitializer.TabType.portrait, FieldTabUIInitializer.InfoType.equipmentTab);
            fieldTabItem.GetComponent<FieldTabUIInitializer>().SetEquippedStatus();

            AddFieldButtonClickListener(button, field, FieldTabUIInitializer.InfoType.equipmentTab);

            unlockFieldTabItems.Add(fieldTabItem);
        }

        foreach (var field in lockedFields)
        {
            GameObject fieldTabItem = PoolManager.Inst.Get(PoolManager.PrefabType.fieldTabPrefab, transform.position);
            Button button = fieldTabItem.GetComponent<Button>();

            fieldTabItem.transform.SetParent(lockFieldCardList.transform, false);
            fieldTabItem.GetComponent<FieldTabUIInitializer>().Init(field, FieldTabUIInitializer.TabType.portrait, FieldTabUIInitializer.InfoType.readOnly);

            AddFieldButtonClickListener(button, field, FieldTabUIInitializer.InfoType.readOnly);

            lockFieldTabItems.Add(fieldTabItem);
        }

        RefreshAllFieldLayouts();
    }

    public void GenerateAllUnitAndFieldLists()
    {
        GenerateUnitList(DataManager.Inst.unitDatas, unlockUnitTabItems, lockUnitTabItems, unlockUnitCardList.transform, lockUnitCardList.transform);
        GenerateUnitList(DataManager.Inst.sephiraUnitDatas, unlockSephiraTabItems, lockSephiraTabItems, unlockSephiraCardList.transform, lockSephiraCardList.transform);
        GenerateFieldList();
    }

    private void UnlockUnitOrSephiraUpdateUI(UnitData unitData, List<GameObject> unlockItems, List<GameObject> lockItems, Transform unlockCardList)
    {
        int index = lockItems.FindIndex(item =>
            item.GetComponent<UnitTabUIInitializer>().data == unitData);

        if (index >= 0)
        {
            GameObject lockedItem = lockItems[index];
            lockItems.RemoveAt(index);

            lockedItem.transform.SetParent(unlockCardList, false);
            lockedItem.GetComponent<UnitTabUIInitializer>().Init(unitData, UnitTabUIInitializer.TabType.portrait, UnitTabUIInitializer.InfoType.equipmentTab);
            lockedItem.GetComponent<UnitTabUIInitializer>().SetEquippedStatus();

            Button button = lockedItem.GetComponent<Button>();
            AddUnitButtonClickListener(button, unitData, UnitTabUIInitializer.InfoType.equipmentTab);

            unlockItems.Add(lockedItem);

            lockItems = lockItems.OrderBy(u => u.GetComponent<UnitTabUIInitializer>().data.unitClass).ThenBy(u => u.GetComponent<UnitTabUIInitializer>().data.unitID).ToList();

            unlockItems = unlockItems.OrderBy(u => u.GetComponent<UnitTabUIInitializer>().data.unitClass).ThenBy(u => u.GetComponent<UnitTabUIInitializer>().data.unitID).ToList();

            for (int i = 0; i < lockItems.Count; i++)
            {
                lockItems[i].transform.SetSiblingIndex(i);
            }

            for (int i = 0; i < unlockItems.Count; i++)
            {
                unlockItems[i].transform.SetSiblingIndex(i);
            }

            RefreshAllUnitAndSephiraLayouts();
            GameModeManager.Inst.jointOperationsManager.InitContentUnlockConditionCount();
        }
    }

    public void UnlockFieldAndUpdateUI(FieldData fieldData)
    {
        int index = lockFieldTabItems.FindIndex(item =>
            item.GetComponent<FieldTabUIInitializer>().data == fieldData);

        if (index >= 0)
        {
            GameObject lockedItem = lockFieldTabItems[index];
            lockFieldTabItems.RemoveAt(index);

            lockedItem.transform.SetParent(unlockFieldCardList.transform, false);
            lockedItem.GetComponent<FieldTabUIInitializer>().Init(fieldData, FieldTabUIInitializer.TabType.portrait, FieldTabUIInitializer.InfoType.equipmentTab);
            lockedItem.GetComponent<FieldTabUIInitializer>().SetEquippedStatus();

            Button button = lockedItem.GetComponent<Button>();
            AddFieldButtonClickListener(button, fieldData, FieldTabUIInitializer.InfoType.equipmentTab);

            unlockFieldTabItems.Add(lockedItem);

            GameUIManager.Inst.RefreshLayoutOnce(fieldLayoutGroup);
            GameUIManager.Inst.RefreshLayoutOnce(fieldUnLockGrid);
            GameUIManager.Inst.RefreshLayoutOnce(fieldLockGrid);
            GameModeManager.Inst.jointOperationsManager.InitContentUnlockConditionCount();
        }
    }


    public void AddUnitButtonClickListener(Button button, UnitData unit, UnitTabUIInitializer.InfoType infoType)
    {
        button.onClick.RemoveAllListeners();

        Transform deckList = unit.unitClass == UnitClass.Sephira ? DeckManager.Inst.equipmentSephiraDeckList.transform : DeckManager.Inst.equipmentUnitDeckList.transform;
        GameObject tab = unit.unitClass == UnitClass.Sephira ? sephiraTab : unitTab;

        button.onClick.AddListener(() => OpenUnitInfoPanel(unit, infoType));
    }

    public void AddFieldButtonClickListener(Button button, FieldData field, FieldTabUIInitializer.InfoType infoType)
    {
        button.onClick.RemoveAllListeners();

        Transform deckList = DeckManager.Inst.equipmentFieldDeckList.transform;

        GameObject tab = fieldTab;

        button.onClick.AddListener(() => OpenFieldInfoPanel(field, infoType));
    }

    void OpenUnitInfoPanel(UnitData unit, UnitTabUIInitializer.InfoType infoType)
    {
        if (DeckManager.Inst.isSelected)
        {
            OnHideSelectPanel();
            return;
        }

        unitInfoWindow.GetComponent<PopUpUI>().OnShowShrink(unitInfoWindow);
        unitInfoWindow.GetComponent<UnitTabUIInitializer>().Init(unit, UnitTabUIInitializer.TabType.info, infoType);
    }

    void OpenFieldInfoPanel(FieldData field, FieldTabUIInitializer.InfoType infoType)
    {
        fieldInfoWindow.GetComponent<PopUpUI>().OnShowShrink(fieldInfoWindow);
        fieldInfoWindow.GetComponent<FieldTabUIInitializer>().Init(field, FieldTabUIInitializer.TabType.info, infoType);
    }

    public void OnHideSelectPanel()
    {
        GameObject deckList = null;
        GameObject tab = null;

        if (unitTab.activeInHierarchy)
        {
            deckList = DeckManager.Inst.equipmentUnitDeckList;
            tab = unitTab;
            for (int i = 0; i < DeckManager.Inst.equipmentUnitLists.Length; i++)
            {
                DeckManager.Inst.equipmentUnitLists[i].equipArrowImage.SetActive(false);
            }
        }
        if (sephiraTab.activeInHierarchy)
        {
            deckList = DeckManager.Inst.equipmentSephiraDeckList;
            tab = sephiraTab;
            DeckManager.Inst.equipmentSephiraLists.equipArrowImage.SetActive(false);
        }
        DeckManager.Inst.selectData = null;

        deckList.transform.SetParent(tab.transform, true);
        DeckManager.Inst.equipmentdeckToggle.transform.SetParent(equimentTab.transform, true);

        DeckManager.Inst.selectUI.SetActive(false);
        DeckManager.Inst.isSelected = false;
    }

    public void UpdateAllEquippedStatus()
    {
        for (int i = 0; i < unlockUnitTabItems.Count; i++)
        {
            unlockUnitTabItems[i].GetComponent<UnitTabUIInitializer>().SetEquippedStatus();
        }

        for (int i = 0; i < unlockSephiraTabItems.Count; i++)
        {
            unlockSephiraTabItems[i].GetComponent<UnitTabUIInitializer>().SetEquippedStatus();
        }

        for (int i = 0; i < unlockFieldTabItems.Count; i++)
        {
            unlockFieldTabItems[i].GetComponent<FieldTabUIInitializer>().SetEquippedStatus();
        }
    }


    public void UnlockUnit(UnitData unitData)
    {
        if (lockUnitTabItems.Count == 0)
            return;
        unitData.isUnlock = true;
        if (unitData.isTrans)
        {
            unitData.transData.isUnlock = true;
        }
        UnlockUnitOrSephiraUpdateUI(unitData, unlockUnitTabItems, lockUnitTabItems, unlockUnitCardList.transform);

        RefreshUnitContentSize(tabContentRect, DataManager.Inst.unitDatas);


        if (PlayerManager.Inst.isJointOperationUnlocked)
        {
            DeckManager.Inst.jointOperationsDeckManager.RefreshJointOperationLists();
        }
        else
        {
            InGameEventManager.Inst.jointOperationConditionChecker.TryUnlockJointOperation();
        }

        for (int i = 0; i < tabToggleGroup.transform.childCount; i++)
        {
            Transform child = tabToggleGroup.transform.GetChild(i);
            Toggle toggle = child.GetComponent<Toggle>();

            if (toggle.isOn)
            {
                OnClickResizeContent(i);
                break;
            }
        }

    }

    public void UnlockSephira(UnitData sephiraData)
    {
        if (lockSephiraTabItems.Count == 0)
            return;

        sephiraData.isUnlock = true;
        if (sephiraData.isTrans)
        {
            sephiraData.transData.isUnlock = true;
        }

        UnlockUnitOrSephiraUpdateUI(sephiraData, unlockSephiraTabItems, lockSephiraTabItems, unlockSephiraCardList.transform);

        RefreshUnitContentSize(tabContentRect, DataManager.Inst.sephiraUnitDatas);

        if (PlayerManager.Inst.isJointOperationUnlocked)
        {
            DeckManager.Inst.jointOperationsDeckManager.RefreshJointOperationLists();
        }
        else
        {
            InGameEventManager.Inst.jointOperationConditionChecker.TryUnlockJointOperation();
        }

        for (int i = 0; i < tabToggleGroup.transform.childCount; i++)
        {
            Transform child = tabToggleGroup.transform.GetChild(i);
            Toggle toggle = child.GetComponent<Toggle>();

            if (toggle.isOn)
            {
                OnClickResizeContent(i);
                break;
            }
        }
    }

    public void UnlockField(FieldData fieldData)
    {
        if (lockFieldTabItems.Count == 0)
            return;

        fieldData.isUnlock = true;
        UnlockFieldAndUpdateUI(fieldData);
        RefreshFieldContentSize(tabContentRect, DataManager.Inst.fieldDatas);

        if (PlayerManager.Inst.isJointOperationUnlocked)
        {
            DeckManager.Inst.jointOperationsDeckManager.RefreshJointOperationLists();
        }
        else
        {
            InGameEventManager.Inst.jointOperationConditionChecker.TryUnlockJointOperation();
        }

        for (int i = 0; i < tabToggleGroup.transform.childCount; i++)
        {
            Transform child = tabToggleGroup.transform.GetChild(i);
            Toggle toggle = child.GetComponent<Toggle>();

            if (toggle != null && toggle.isOn)
            {
                break;
            }
        }
    }

    void RefreshAllUnitAndSephiraLayouts()
    {
        GameUIManager.Inst.RefreshLayoutOnce(unitLayoutGroup);
        GameUIManager.Inst.RefreshLayoutOnce(unitUnlockGrid);
        GameUIManager.Inst.RefreshLayoutOnce(unitLockGrid);

        GameUIManager.Inst.RefreshLayoutOnce(sephiraLayoutGroup);
        GameUIManager.Inst.RefreshLayoutOnce(sephiraUnlockGrid);
        GameUIManager.Inst.RefreshLayoutOnce(sephiraLockGrid);

        GameUIManager.Inst.RefreshElementOnce(unitTabElement);
        GameUIManager.Inst.RefreshElementOnce(sephiraTabElement);
    }

    void RefreshAllFieldLayouts()
    {
        GameUIManager.Inst.RefreshLayoutOnce(fieldLayoutGroup);
        GameUIManager.Inst.RefreshLayoutOnce(fieldUnLockGrid);
        GameUIManager.Inst.RefreshLayoutOnce(fieldLockGrid);

        GameUIManager.Inst.RefreshElementOnce(fieldTabElement);
    }
}
