using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SayaCharacterManager : MonoBehaviour
{
    public static SayaCharacterManager Inst;

    public int fieldLevel;
    public int hunterLevel;
    public int shamanLevel;

    public AnimatorOverrideController[] frontAnim;
    public AnimatorOverrideController[] middleAnim;
    public AnimatorOverrideController[] backAnim;

    public UnitData fieldData;
    public UnitData hunterData;
    public UnitData shamanData;

    public int rollingCost { get; } = 300;

    public enum UnitType { field, hunter, shaman }

    void Awake()
    {
        Inst = this;

        fieldLevel = 0;
        hunterLevel = 0;
        shamanLevel = 0;
    }

    public void TryRolling()
    {
        int curCoin = GameManager.Inst.coin;

        if (curCoin >= rollingCost)
        {
            GameManager.Inst.ModifyCoin(-rollingCost);
            StartCoroutine(TryRollingCoroutune());
            Debug.Log("롤링 시작");
        }
        else
        {
            Debug.Log("돈 부족");
        }

    }

    private IEnumerator TryRollingCoroutune()
    {
        yield return new WaitForSeconds(2);
        int ran = UnitUtility.GetRandomValue(0, 100);
        Debug.Log("현재 롤링 결과: " + ran);
    }

    public UnitData GetUnitData(FieldSlot.FormationRow formationRow)
    {
        switch (formationRow)
        {
            case FieldSlot.FormationRow.Front:
                return fieldData;
            case FieldSlot.FormationRow.Middle:
                return hunterData;
            case FieldSlot.FormationRow.Back:
                return shamanData;
        }
        return null;
    }

    public AnimatorOverrideController GetAnimatorController(FieldSlot.FormationRow formationRow)
    {
        switch (formationRow)
        {
            case FieldSlot.FormationRow.Front:
                switch (fieldLevel)
                {
                    case 0:
                        return frontAnim[0];
                    case 1:
                        return frontAnim[1];
                    case 2:
                        return frontAnim[2];
                    case 3:
                        return frontAnim[3];
                    case 4:
                        return frontAnim[4];
                }
                break;
            case FieldSlot.FormationRow.Middle:
                switch (hunterLevel)
                {
                    case 0:
                        return middleAnim[0];
                    case 1:
                        return middleAnim[1];
                    case 2:
                        return middleAnim[2];
                    case 3:
                        return middleAnim[3];
                    case 4:
                        return middleAnim[4];
                }
                break;
            case FieldSlot.FormationRow.Back:
                switch (shamanLevel)
                {
                    case 0:
                        return backAnim[0];
                    case 1:
                        return backAnim[1];
                    case 2:
                        return backAnim[2];
                    case 3:
                        return backAnim[3];
                    case 4:
                        return backAnim[4];
                }
                break;
        }
        return null;
    }

    public void ApplyTransformNextFrame(Unit unit)
    {
        unit.StartCoroutine(ApplyTransformNextFrameCoroutine(unit));
    }

    private IEnumerator ApplyTransformNextFrameCoroutine(Unit unit)
    {
        yield return new WaitForEndOfFrame();
        unit.TransInit(GetUnitData(unit.unitDrag.curBatchFieldTrigger.formationRow), unit.starLevel, false);
        unit.anim.runtimeAnimatorController = GetAnimatorController(unit.unitDrag.curBatchFieldTrigger.formationRow);
    }

    public void UpgradeUnit(UnitType unitType)
    {
        int targetId = 0;
        switch (unitType)
        {
            case UnitType.field:
                if (fieldLevel >= 4)
                    return;
                fieldLevel++;
                targetId = fieldData.unitID;
                break;
            case UnitType.hunter:
                if (hunterLevel >= 4)
                    return;
                hunterLevel++;
                targetId = hunterData.unitID;
                break;
            case UnitType.shaman:
                if (shamanLevel >= 4)
                    return;
                shamanLevel++;
                targetId = shamanData.unitID;
                break;
        }

        List<Unit> targetUnits = FieldManager.Inst.GetFieldUnitsById(targetId);

        if (targetUnits.Count == 0)
            return;

        foreach (Unit unit in targetUnits)
        {
            ApplyTransformNextFrame(unit);
        }
    }




    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            UpgradeUnit(UnitType.field);
            UpgradeUnit(UnitType.hunter);
            UpgradeUnit(UnitType.shaman);
        }
    }






}
