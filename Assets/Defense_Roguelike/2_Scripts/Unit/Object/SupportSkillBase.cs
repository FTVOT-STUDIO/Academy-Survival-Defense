using UnityEditor;
using UnityEngine;

public abstract class SingleSupportSkillBase : ISupportApplyOnce, IResetSingleSupportSkill, ISingleSupportBuffUpdate
{
    public abstract void ApplySupport(Unit unit, Unit targetUnit);
    public abstract void ClearSupport(Unit unit, Unit targetUnit);

    protected float curValue;

    public void ResetBuff(Unit unit, Unit scannedUnit)
    {
        unit.isBuffed = false;
        scannedUnit.isBuffed = false;

        unit.buffIcon.SetActive(false);
        scannedUnit.buffIcon.SetActive(false);
        unit.materialChanger.SetOriginalMaterial();
        scannedUnit.materialChanger.SetOriginalMaterial();

        scannedUnit.unitScanner.buffUnit = null;
        unit.unitDrag.exiUnit = null;
        unit.unitScanner.buffUnit = null;

        unit.effect.transform.SetParent(PoolManager.Inst.transform);
        unit.effect.SetActive(false);

        unit.unitScanner.nearestUnit = null;

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SupOff);
        unit.anim.SetTrigger("Attack");
        scannedUnit.anim.SetTrigger("Attack");

    }

    public void ResetSupport(Unit unit, Unit targetUnit)
    {
        ClearSupport(unit, targetUnit);
        ResetBuff(unit, targetUnit);
    }

    public void ApplySupportApplyOnce(Unit unit)
    {
        Unit scannedUnit = unit.unitScanner.GetCurrentBuffedUnit();

        ApplySupport(unit, scannedUnit);
        unit.isBuffed = true;
        scannedUnit.isBuffed = true;

        scannedUnit.buffIcon.SetActive(true);
        unit.buffIcon.SetActive(true);
        unit.materialChanger.SetReplacementMaterial();
        scannedUnit.materialChanger.SetReplacementMaterial();

        scannedUnit.unitScanner.buffUnit = unit.gameObject;
        unit.unitScanner.buffUnit = scannedUnit.gameObject;

        unit.effect = EffectManager.Inst.Get(unit.unitData.effectPrefab, scannedUnit.transform.position);
        unit.effect.transform.SetParent(scannedUnit.buffPrefabParent);
        unit.effect.transform.localPosition = Vector3.zero;
        scannedUnit.effect = unit.effect;

        unit.unitDrag.curUnit = scannedUnit;

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SupOn);
        unit.anim.SetTrigger("Attack");
        scannedUnit.anim.SetTrigger("Attack");
    }

    public void ApplySupportBuffUpdate(Unit unit, Unit targetUnit)
    {
        ClearSupport(unit, targetUnit);
        unit.RecalculateStats();
        ApplySupport(unit, targetUnit);
    }
}

public abstract class MultSupportSkillBase : ISupportApplyOnce, IResetMultSupportSkill, IMultSupportBuffUpdate

{
    public abstract void ApplySupport(Unit unit);
    public abstract void ClearSupport(Unit unit);

    protected float curValue;

    public void ResetSupport(Unit unit)
    {
        ClearSupport(unit);
    }

    public void ApplySupportApplyOnce(Unit unit)
    {
        ApplySupport(unit);
    }

    public void ApplySupportBuffUpdate(Unit unit)
    {
        foreach (var hit in unit.unitSupportBuffController.curBuffFieldSlots)
        {
            FieldPlacementTrigger slot = hit.GetComponent<FieldPlacementTrigger>();
            slot.RemoveUnitPlacedEvent(unit.unitEvent.onUnitPlacedAction, unit.unitEvent.onUnitRemovedAction);
        }

        unit.RecalculateStats();

        foreach (var hit in unit.unitSupportBuffController.curBuffFieldSlots)
        {
            FieldPlacementTrigger slot = hit.GetComponent<FieldPlacementTrigger>();
            slot.RegisterUnitPlacedEvent(unit.unitEvent.onUnitPlacedAction);
            slot.RegisterUnitRemovedEvent(unit.unitEvent.onUnitRemovedAction);
        }
    }
}
