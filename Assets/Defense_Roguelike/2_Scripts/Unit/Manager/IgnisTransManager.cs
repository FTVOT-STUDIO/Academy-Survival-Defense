using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class IgnisTransManager : MonoBehaviour, ITransformationState
{
    public static IgnisTransManager Inst;

    public bool IsTransformationInProgress { get; set; }
    public UnitData ignisData;
    public UnitData solarIgnisData;

    public bool isTransOn;
    public bool isTransOff;

    void Awake()
    {
        Inst = this;
    }

    public bool IsEvenUnitCountIncludingSelf(Unit unit)
    {
        return GetOverlapUnitsIncludingSelf(unit).Count % 2 == 0 && GetOverlapUnitsIncludingSelf(unit).Count
    != 0;
    }

    public bool IsEvenOverlapUnitCount(Unit unit)
    {
        return GetOverlapUnits(unit).Count % 2 == 0 && GetOverlapUnits(unit).Count != 0;
    }

    private List<Unit> GetOverlapUnits(Unit unit)
    {
        return FieldManager.Inst.GetOverlapUnit(unit.unitData.unitID, unit);
    }

    private List<Unit> GetOverlapUnitsIncludingSelf(Unit unit)
    {
        List<Unit> overlapUnits = FieldManager.Inst.GetOverlapUnit(unit.unitData.unitID, unit);

        var allUnits = new List<Unit>(overlapUnits.Count + 1) { unit };
        allUnits.AddRange(overlapUnits);

        return allUnits;
    }

    public void PlayTransOnEffect(Unit unit)
    {
        if (isTransOn)
            return;
        StartCoroutine(PlayTransOnEffectCoroutine(unit));
    }

    public void PlayTransOffEffect(Unit unit)
    {
        if (isTransOff)
            return;
        StartCoroutine(PlayTransOffEffectCoroutine(unit));
    }

    private IEnumerator PlayTransOnEffectCoroutine(Unit unit)
    {
        var allUnits = GetOverlapUnitsIncludingSelf(unit);
        for (int i = 0; i < allUnits.Count; i++)
        {
            EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOnEffect, allUnits[i].transform.position);
        }
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisTransOn0);
        isTransOn = true;
        yield return new WaitForSeconds(0.1f);
        isTransOn = false;
    }

    private IEnumerator PlayTransOffEffectCoroutine(Unit unit)
    {
        var overlapUnits = GetOverlapUnits(unit);

        for (int i = 0; i < overlapUnits.Count; i++)
        {
            EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOffEffect, overlapUnits[i].transform.position);
        }
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisTransOff);
        isTransOff = true;
        yield return new WaitForSeconds(0.1f);
        isTransOff = false;
    }
}
