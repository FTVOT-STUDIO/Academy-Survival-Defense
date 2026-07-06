using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JointOperationsDeckManager : MonoBehaviour
{
    [SerializeField] GameObject[] inputLockImages;
    public JointOperationUnitSlot[] unitSlots;
    public JointOperationSephiraSlot sephiraSlot;
    public JointOperationFieldSlot fieldSlot;

    [SerializeField] GameObject unitCardPrefab;
    [SerializeField] GameObject sephiraCardPrefab;
    [SerializeField] GameObject fieldCardPrefab;

    [SerializeField] GameObject unitListParent;
    [SerializeField] GameObject sephiraListParent;
    [SerializeField] GameObject fieldListParent;

    [SerializeField] ContentSizeFitter equimentContentSizeFitter;

    [SerializeField] Scrollbar scrollbar;

    public List<JointOperationUnitList> unitList = new List<JointOperationUnitList>();
    public List<JointOperationSephiraList> sephiraList = new List<JointOperationSephiraList>();
    public List<JointOperationFieldList> fieldList = new List<JointOperationFieldList>();

    public int curUnitSlotIndex;

    public void InitializeJointOperationUIAndDeck()
    {
        SetParentsActive(true, true, true);

        List<UnitData> unlockUnitData = DataManager.Inst.unitDatas.Where(unit => unit.isUnlock).OrderBy(unit => unit.unitClass).ToList();
        List<UnitData> unlockSephiraData = DataManager.Inst.sephiraUnitDatas.Where(unit => unit.isUnlock).ToList();
        List<FieldData> unlockFieldData = DataManager.Inst.fieldDatas.Where(unit => unit.isUnlock).ToList();

        for (int i = 0; i < unlockUnitData.Count; i++)
        {
            GameObject unitList = PoolManager.Inst.Get(PoolManager.PrefabType.JOP_UnitList, transform.position);
            unitList.transform.SetParent(unitListParent.transform, false);
            unitList.GetComponent<UnitList>().Init(unlockUnitData[i]);
            unitList.GetComponent<Toggle>().onValueChanged.AddListener((_) => unitList.GetComponent<JointOperationUnitList>().ChangeUnit());
            this.unitList.Add(unitList.GetComponent<JointOperationUnitList>());
        }
        for (int i = 0; i < unlockSephiraData.Count; i++)
        {
            GameObject sephiraList = PoolManager.Inst.Get(PoolManager.PrefabType.JOP_SephiraList, transform.position);
            sephiraList.transform.SetParent(sephiraListParent.transform, false);
            sephiraList.GetComponent<UnitList>().Init(unlockSephiraData[i]);
            sephiraList.GetComponent<Toggle>().onValueChanged.AddListener((_) => sephiraList.GetComponent<JointOperationSephiraList>().ChangeSephira());
            this.sephiraList.Add(sephiraList.GetComponent<JointOperationSephiraList>());
        }
        for (int i = 0; i < unlockFieldData.Count; i++)
        {
            GameObject fieldList = PoolManager.Inst.Get(PoolManager.PrefabType.JOP_FieldList, transform.position);
            fieldList.transform.SetParent(fieldListParent.transform, false);
            fieldList.GetComponent<FieldList>().Init(unlockFieldData[i]);
            fieldList.GetComponent<Toggle>().onValueChanged.AddListener((_) => fieldList.GetComponent<JointOperationFieldList>().ChangeField());
            this.fieldList.Add(fieldList.GetComponent<JointOperationFieldList>());
        }

        if (!PlayerManager.Inst.isJointOperationUnlocked)
            InitializeJointOperationDeck(unlockUnitData, unlockSephiraData[0], unlockFieldData[0]);
        else
            InitializeJointOperationDeck(DeckManager.Inst.jointOperationDeckUnitDatas, DeckManager.Inst.jointOperationDeckSephiraData, DeckManager.Inst.jointOperationDeckFieldData);

        foreach (JointOperationUnitSlot slot in unitSlots)
        {
            UnitData data = slot.unitList.unitData;
            JointOperationUnitList match = unitList.Find(u => u.unitList.unitData == data);
            slot.jointOperationUnitList = match;
            match.ActiveEquipmentImage(true);
        }

        {
            UnitData data = sephiraSlot.unitList.unitData;
            JointOperationSephiraList match = sephiraList.Find(s => s.unitList.unitData == data);
            sephiraSlot.jointOperationSephiraList = match;
            match.ActiveEquipmentImage(true);
        }

        {
            FieldData data = fieldSlot.fieldList.fieldData;
            JointOperationFieldList match = fieldList.Find(f => f.fieldList.fieldData == data);
            fieldSlot.jointOperationFieldList = match;
            match.ActiveEquipmentImage(true);
        }

        curUnitSlotIndex = 0;
        SetParentsActive(true, false, false);
        ResetScrollbar();
    }


    public void DisableInputLock(int index)
    {
        for (int i = 0; i < inputLockImages.Length; i++)
        {
            inputLockImages[i].SetActive(index != i);
        }
    }

    public void TurnOnToggle(Toggle toggle)
    {
        toggle.isOn = true;
    }

    public void ResetScrollbar()
    {
        CoroutineRunner.Inst.RunCoroutine(ResetScrollbarRoution());
    }

    IEnumerator ResetScrollbarRoution()
    {
        yield return null;
        scrollbar.value = 1;
    }

    void SetParentsActive(bool isActiveUnit, bool isActiveSephira, bool isActiveField)
    {
        unitListParent.SetActive(isActiveUnit);
        sephiraListParent.SetActive(isActiveSephira);
        fieldListParent.SetActive(isActiveField);
    }

    public void InitializeJointOperationDeck(List<UnitData> unitDataList, UnitData sephiraData, FieldData fieldData)
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            UnitData data = unitDataList[i];
            JointOperationUnitSlot slot = unitSlots[i];
            slot.unitList.Init(data);
            DeckManager.Inst.jointOperationDeckUnitDatas[i] = data;
        }

        sephiraSlot.unitList.Init(sephiraData);
        DeckManager.Inst.jointOperationDeckSephiraData = sephiraData;

        fieldSlot.fieldList.Init(fieldData);
        DeckManager.Inst.jointOperationDeckFieldData = fieldData;
    }

    public void RefreshJointOperationLists()
    {
        foreach (JointOperationUnitList item in unitList)
        {
            item.gameObject.SetActive(false);
            item.ActiveEquipmentImage(false);
        }

        unitList.Clear();

        foreach (JointOperationSephiraList item in sephiraList)
        {
            item.gameObject.SetActive(false);
            item.ActiveEquipmentImage(false);
        }
        sephiraList.Clear();

        foreach (JointOperationFieldList item in fieldList)
        {
            item.gameObject.SetActive(false);
            item.ActiveEquipmentImage(false);
        }
        fieldList.Clear();

        List<UnitData> unlockUnitData = DataManager.Inst.unitDatas
            .Where(unit => unit.isUnlock)
            .OrderBy(unit => unit.unitClass)
            .ToList();

        foreach (var data in unlockUnitData)
        {
            GameObject obj = PoolManager.Inst.Get(PoolManager.PrefabType.JOP_UnitList, transform.position);
            obj.transform.SetParent(unitListParent.transform, false);
            obj.GetComponent<UnitList>().Init(data);
            obj.GetComponent<Toggle>().onValueChanged.AddListener((_) => obj.GetComponent<JointOperationUnitList>().ChangeUnit());
            unitList.Add(obj.GetComponent<JointOperationUnitList>());
        }

        List<UnitData> unlockSephiraData = DataManager.Inst.sephiraUnitDatas
            .Where(unit => unit.isUnlock)
            .ToList();

        foreach (var data in unlockSephiraData)
        {
            GameObject obj = PoolManager.Inst.Get(PoolManager.PrefabType.JOP_SephiraList, transform.position);
            obj.transform.SetParent(sephiraListParent.transform, false);
            obj.GetComponent<UnitList>().Init(data);
            obj.GetComponent<Toggle>().onValueChanged.AddListener((_) => obj.GetComponent<JointOperationSephiraList>().ChangeSephira());
            sephiraList.Add(obj.GetComponent<JointOperationSephiraList>());
        }

        List<FieldData> unlockFieldData = DataManager.Inst.fieldDatas
            .Where(field => field.isUnlock)
            .ToList();

        foreach (var data in unlockFieldData)
        {
            GameObject obj = PoolManager.Inst.Get(PoolManager.PrefabType.JOP_FieldList, transform.position);
            obj.transform.SetParent(fieldListParent.transform, false);
            obj.GetComponent<FieldList>().Init(data);
            obj.GetComponent<Toggle>().onValueChanged.AddListener((_) => obj.GetComponent<JointOperationFieldList>().ChangeField());
            fieldList.Add(obj.GetComponent<JointOperationFieldList>());
        }

        foreach (JointOperationUnitSlot slot in unitSlots)
        {
            UnitData data = slot.unitList.unitData;
            JointOperationUnitList match = unitList.Find(u => u.unitList.unitData == data);
            slot.jointOperationUnitList = match;
            match.ActiveEquipmentImage(true);
        }

        {
            UnitData data = sephiraSlot.unitList.unitData;
            JointOperationSephiraList match = sephiraList.Find(s => s.unitList.unitData == data);
            sephiraSlot.jointOperationSephiraList = match;
            match.ActiveEquipmentImage(true);
        }

        {
            FieldData data = fieldSlot.fieldList.fieldData;
            JointOperationFieldList match = fieldList.Find(f => f.fieldList.fieldData == data);
            fieldSlot.jointOperationFieldList = match;
            match.ActiveEquipmentImage(true);
        }

        CoroutineRunner.Inst.RunCoroutine(ResetScrollbarRoution());
    }

    public void InitializeJointOperationsAllList()
    {
        for (int i = 0; i < unitSlots.Length; i++)
        {
            unitSlots[i].unitList.Init(unitSlots[i].unitList.unitData);
        }
        sephiraSlot.unitList.Init(sephiraSlot.unitList.unitData);
        fieldSlot.fieldList.Init(fieldSlot.fieldList.fieldData);

        for (int i = 0; i < unitList.Count; i++)
        {
            unitList[i].unitList.Init(unitList[i].unitList.unitData);
        }
        for (int i = 0; i < sephiraList.Count; i++)
        {
            sephiraList[i].unitList.Init(sephiraList[i].unitList.unitData);
        }
        for (int i = 0; i < fieldList.Count; i++)
        {
            fieldList[i].fieldList.Init(fieldList[i].fieldList.fieldData);
        }

        for (int i = 0; i < DeckManager.Inst.jointOperationsUnitList.Length; i++)
        {
            DeckManager.Inst.jointOperationsUnitList[i].Init(DeckManager.Inst.jointOperationsUnitList[i].unitData);
        }
        DeckManager.Inst.jointOperationsSephiraList.Init(DeckManager.Inst.jointOperationsSephiraList.unitData);
        DeckManager.Inst.jointOperationsFieldList.Init(DeckManager.Inst.jointOperationsFieldList.fieldData);
    }

}
