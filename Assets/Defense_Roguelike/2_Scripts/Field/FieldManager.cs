using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using VInspector;

public class UnitSnapshot : IUnitBaseStat
{
    public Unit originUnit;
    public int starLevel;
    public bool isBuffed;
    public bool isTrans;

    public float BaseDamage { get; set; }
    public float BaseSkillValue { get; set; }
    public float BaseDefense { get; set; }
    public float BaseMaxHealth { get; set; }
    public float BaseMaxShield { get; set; }
    public float BaseCoolTime { get; set; }
    public float BaseRange { get; set; }
    public float BaseAttackSpeed { get; set; }

    public float BaseCriticalPer { get; set; }
    public float BaseCriticalDamage { get; set; }
    public float BaseHitChance { get; set; }
    public float BaseSkillTriggerChance { get; set; }
    public float BaseAreaDamage { get; set; }
    public float BaseSingleTargetDamage { get; set; }
    public float BaseEvasion { get; set; }
    public float BaseDefensePenetration { get; set; }
    public float BaseAttributeResistancePenetration { get; set; }
    public float BaseAttributeDamageMultiplier { get; set; }

    public float BaseHealingMultiplier { get; set; }
    public float BaseHealingReceivedMultiplier { get; set; }

    public float BaseShieldMultiplier { get; set; }
    public float BaseShieldReceivedMultiplier { get; set; }
    public float BaseNormalAttackMultiplier { get; set; }
    public float BaseSkillMultiplier { get; set; }
    public float BaseAbilityMultiplier { get; set; }
    public float BaseManaReceivedMultiplier { get; set; }
    public float BaseDamageTakenIncrease { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
public class FieldManager : MonoBehaviour
{
    public static FieldManager Inst;

    [Header("# Field ID")]
    [SerializeField] int fieldID;
    [Tab("Field Info")]
    [Header("# Field Info")]
    public int fieldUnitCount = 0;
    public Transform[] fieldSlots;
    public List<GameObject> placementMarker;
    public List<GameObject> levelUpMarker;
    public List<GameObject> supportMarker;
    [Header("# Batch Unit")]
    public GameObject[] fieldUnits;
    public SephiraUnit fieldSephira;
    [Space]
    private InGameFieldInitializer currentField;
    public GameObject normalFieldPrefab;
    public GameObject jointOpertaionFieldPrefab;
    [Tab("Select Data")]
    public GameObject selectUnit;
    public GameObject lastUnit;
    public List<SpriteRenderer> sp;


    void Awake()
    {
        Inst = this;
    }

    void OnEnable()
    {
        InGameFieldInitializer inGameField;
        switch (GameManager.Inst.gameMode)
        {
            case GameManager.GameMode.Normal:
                inGameField = Instantiate(normalFieldPrefab, transform).GetComponent<InGameFieldInitializer>();
                break;
            case GameManager.GameMode.JointOperation:
                inGameField = Instantiate(jointOpertaionFieldPrefab, transform).GetComponent<InGameFieldInitializer>();
                break;
            default:
                inGameField = Instantiate(normalFieldPrefab, transform).GetComponent<InGameFieldInitializer>();
                break;
        }

        currentField = inGameField;
        inGameField.gameObject.SetActive(true);
        inGameField.InitializeAllSlotMarkers();

        for (int i = 0; i < currentField.fieldSlots.Length; i++)
        {
            sp.Add(currentField.fieldSlots[i].GetComponent<SpriteRenderer>());
        }

        fieldSlots = new Transform[currentField.fieldSlots.Length];
        for (int i = 0; i < currentField.fieldSlots.Length; i++)
        {
            fieldSlots[i] = currentField.fieldSlots[i].transform;
        }

        fieldUnits = new GameObject[currentField.fieldSlots.Length];
        currentField.backField.sprite = DeckManager.Inst.currentInGameDeck.fieldData.fieldBackGround;

        for (int i = 0; i < currentField.fieldSlots.Length; i++)
        {
            currentField.fieldSlots[i].GetComponent<SpriteRenderer>().sprite = DeckManager.Inst.currentInGameDeck.fieldData.fieldSlot;
        }

        fieldSephira.transform.position = currentField.sephiraSlot.position;
        fieldSephira.sp.sortingOrder = currentField.sephiraSlot.GetComponent<FieldSlot>().sortingOrder;

    }

    public void UnitBatch(GameObject unit, int slotIndex, bool shouldInvokeBatchEvent)
    {
        if (slotIndex != -1)
        {
            Unit unitScript = unit.GetComponent<Unit>();
            unit.transform.position = fieldSlots[slotIndex].position;
            fieldUnits[slotIndex] = unit;
            unitScript.sp.sortingOrder = fieldSlots[slotIndex].
            GetComponent<FieldSlot>().sortingOrder;
            unitScript.unitDrag.curIndex = slotIndex;
            fieldSlots[slotIndex].GetComponent<FieldPlacementTrigger>().PlaceUnit(unitScript);

            if (unitScript.unitData.type == Type.Support && unitScript.unitData.unitSupportType == UnitSupportType.Mult)
                unitScript.unitSupportBuffController.ApplySupportBuffToNearbyFields();


            if (!shouldInvokeBatchEvent)
                return;



            if (unitScript.unitSkill is IUnitFieldBatchPassive unitFieldBatchPassive)
            {
                unitFieldBatchPassive.ApplyUnitFieldBatchPassive(unitScript);
            }
            if (unitScript.unitSkill is IUnitFieldBatchSkill unitFieldBatchSkill)
            {
                unitFieldBatchSkill.ApplyUnitFieldBatchSkill(unitScript);
            }
        }
    }

    public void ResetFieldState()
    {
        for (int j = 0; j < fieldUnits.Length; j++)
        {
            if (fieldUnits[j] == null)
                continue;

            Unit fieldUnit = fieldUnits[j].GetComponent<Unit>();
            SpriteRenderer[] sprites = fieldUnit.gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (var sprite in sprites)
            {
                sprite.enabled = true;
            }

            if (fieldUnit == null)
                continue;
            fieldUnit.sp.enabled = true;
        }

        for (int i = 0; i < fieldSlots.Length; i++)
        {
            placementMarker[i].SetActive(false);
            levelUpMarker[i].SetActive(false);
            supportMarker[i].SetActive(false);

            if (fieldUnits[i] != null)
            {
                sp[i].color = new Color(0.7f, 0.7f, 0.7f, 1);
            }
            else
            {
                sp[i].color = Color.white;
            }
        }
    }

    public void ResetFieldUnitLocate()
    {
        for (int i = 0; i < fieldUnits.Length; i++)
        {
            if (fieldUnits[i] != null)
            {
                fieldUnits[i].GetComponent<Unit>().unitDrag.ResetLoaction();
            }
        }
    }

    public List<Unit> GetFieldBatchUnits()
    {
        List<Unit> unitsList = new List<Unit>();

        for (int i = 0; i < fieldUnits.Length; i++)
        {
            if (fieldUnits[i] != null)
            {
                Unit unit = fieldUnits[i].GetComponent<Unit>();
                if (unit != null && !unit.unitEvent.IsIgnored)
                {
                    unitsList.Add(unit);
                }
            }
        }
        return unitsList;
    }

    public List<Unit> GetFieldUnitsById(int targetUnitId)
    {
        List<Unit> result = new List<Unit>();

        for (int i = 0; i < fieldUnits.Length; i++)
        {
            var field = fieldUnits[i];
            if (field == null) continue;

            Unit unit = field.GetComponent<Unit>();
            if (unit == null || unit.unitEvent.IsIgnored) continue;

            if (unit.unitData.unitID == targetUnitId)
            {
                result.Add(unit);
            }
        }

        return result;
    }

    public List<GameObject> GetEmptyUnitSlots()
    {
        List<GameObject> unitSlots = new List<GameObject>();

        for (int i = 0; i < fieldUnits.Length; i++)
        {
            if (fieldUnits[i] == null)
            {
                unitSlots.Add(fieldUnits[i]);
            }
        }
        return unitSlots;
    }

    public void ModifySpawnedUnitCount(int count)
    {
        int previousfieldUnitCount = fieldUnitCount;
        fieldUnitCount += count;

        InGameUIManager.Inst.spawnUnitCountText.text += count;
        GameFlowManager.Inst.ChangeSpentUnitSpawnCount(1);
        NumberCounter.StartCounting(this, fieldUnitCount, previousfieldUnitCount, 0.3f, InGameUIManager.Inst.spawnUnitCountText, "", "/" + fieldSlots.Length);
        InGameUIManager.Inst.spawnUnitCountText.GetComponentInParent<PopUpUI>(true).PopUpText(InGameUIManager.Inst.spawnUnitCountText.transform.parent.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AutoMergeUnits();
        }
    }

    public void AutoMergeUnits()
    {
        var fieldUnits = this.fieldUnits;

        for (int i = 0; i < fieldUnits.Length; i++)
        {
            var unitObjA = fieldUnits[i];
            InGameUIManager.Inst.unitInfoUI.currentUnit = unitObjA;
            if (unitObjA == null) continue;

            Unit unitA = unitObjA.GetComponent<Unit>();
            if (unitA == null) continue;

            if (unitA.starLevel >= unitA.unitLevel.maxLevel || unitA.unitEvent.IsSilenced || unitA.unitEvent.IsIgnored)
                continue;

            for (int j = i + 1; j < fieldUnits.Length; j++)
            {
                var unitObjB = fieldUnits[j];
                if (unitObjB == null) continue;

                Unit unitB = unitObjB.GetComponent<Unit>();
                if (unitB == null) continue;

                if (unitB.starLevel >= unitB.unitLevel.maxLevel || unitB.unitEvent.IsSilenced || unitB.unitEvent.IsIgnored)
                    continue;

                if (unitA.unitData == unitB.unitData && unitA.starLevel == unitB.starLevel)
                {
                    int batchIndex = unitB.unitDrag.curIndex;

                    UnitDrag dragA = unitA.GetComponent<UnitDrag>();
                    dragA.Merge(unitB, unitA);
                    dragA.MergeEffect(unitA);
                    UnitBatch(dragA.gameObject, batchIndex, true);
                    break;
                }
            }
        }
        ResetFieldState();
    }

    public List<Unit> GetOverlapUnit(int targetID, Unit owner)
    {
        var fieldUnits = GetFieldBatchUnits();
        List<Unit> result = new List<Unit>(fieldUnits.Count);

        foreach (var unit in fieldUnits)
        {
            if (unit.unitData.unitID == targetID && unit != owner)
            {
                result.Add(unit);
            }
        }

        return result;
    }

    public void RemoveBatchUnit(GameObject unit)
    {
        for (int i = 0; i < fieldUnits.Length; i++)
        {
            if (unit == fieldUnits[i])
            {
                fieldUnits[i] = null;
                break;
            }
        }
    }


    public void CaptureFieldSnapshot()
    {

    }
}