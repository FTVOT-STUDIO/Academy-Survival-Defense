using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebuffManager : MonoBehaviour
{
    public static DebuffManager Inst;

    public bool blockLevelUpUI = false;
    public bool isUnitSilenceDebuff = false;
    public bool isMergeRandomizedDebuff = false;

    public UnitData blockUnitData;

    void Awake()
    {
        Inst = this;
    }

    // HardMode Debuff
    public void ApplyRandomDebuff(int unitCount)
    {
        for (int i = 0; i < unitCount; i++)
        {
            Action[] actions = {
            () => ApplyRandomizeDebuff(1),
            () => ApplySyntaxErrorDebuff(1, 0.5f, 0.5f, 30),
            () => ApplyParasiticDebuff(1, 5 * GameManager.Inst.wave * 0.5f, 30, 1),
            () => ApplySilencedDebuff(1,10)
        }
        ;

            int ran = Random.Range(0, actions.Length);

            actions[ran]();
        }
    }

    public void ApplyRandomizeDebuff(int unitCount)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Illusion);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Laugh0);
        List<Unit> availableUnits = new List<Unit>();

        foreach (var fieldUnit in FieldManager.Inst.fieldUnits)
        {
            if (fieldUnit != null && !fieldUnit.GetComponent<UnitEvent>().IsInvulnerable && !fieldUnit.GetComponent<UnitEvent>().IsIgnored)
            {
                availableUnits.Add(fieldUnit.GetComponent<Unit>());
            }
        }

        if (availableUnits.Count > 0)
        {
            List<Unit> selectedUnits = new();

            int unitsToApply = Mathf.Min(unitCount, availableUnits.Count);

            if (availableUnits.Count <= unitCount)
            {
                selectedUnits.AddRange(availableUnits);
            }
            else
            {
                while (selectedUnits.Count < unitsToApply)
                {
                    Unit selectedUnit = availableUnits[Random.Range(0, availableUnits.Count)];

                    if (!selectedUnits.Contains(selectedUnit))
                    {
                        selectedUnits.Add(selectedUnit);
                    }
                }
            }

            foreach (Unit selectedUnit in selectedUnits)
            {
                UnitEvent unitEvent = selectedUnit.GetComponent<UnitEvent>();
                unitEvent.ApplyRandomizeUnit();
            }

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.DingDong);
        }
    }

    public void ApplySilencedDebuff(int unitCount, float duration)
    {

        List<Unit> availableUnits = new List<Unit>();

        foreach (var fieldUnit in FieldManager.Inst.fieldUnits)
        {
            if (fieldUnit != null && !fieldUnit.GetComponent<UnitEvent>().IsSilenced && !fieldUnit.GetComponent<UnitEvent>().IsInvulnerable && !fieldUnit.GetComponent<UnitEvent>().IsIgnored)
            {
                availableUnits.Add(fieldUnit.GetComponent<Unit>());
            }
        }

        if (availableUnits.Count > 0)
        {
            List<Unit> selectedUnits = new();

            int unitsToApply = Mathf.Min(unitCount, availableUnits.Count);

            if (availableUnits.Count <= unitCount)
            {
                selectedUnits.AddRange(availableUnits);
            }
            else
            {
                while (selectedUnits.Count < unitsToApply)
                {
                    Unit selectedUnit = availableUnits[Random.Range(0, availableUnits.Count)];

                    if (!selectedUnits.Contains(selectedUnit))
                    {
                        selectedUnits.Add(selectedUnit);
                    }
                }
            }

            foreach (Unit selectedUnit in selectedUnits)
            {
                UnitEvent unitEvent = selectedUnit.GetComponent<UnitEvent>();
                unitEvent.ApplySilencedUnit(duration);
            }

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Slience);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Silence1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn2);
        }
    }

    public void ApplySyntaxErrorDebuff(int unitCount, float hitChanceDecrease, float damageDecrease, float duration)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsSkill4);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill2);
        List<Unit> availableUnits = new List<Unit>();

        foreach (var fieldUnit in FieldManager.Inst.fieldUnits)
        {
            if (fieldUnit != null && !fieldUnit.GetComponent<UnitEvent>().IsSyntaxError && !fieldUnit.GetComponent<UnitEvent>().IsInvulnerable && !fieldUnit.GetComponent<UnitEvent>().IsIgnored)
            {
                availableUnits.Add(fieldUnit.GetComponent<Unit>());
            }
        }

        if (availableUnits.Count > 0)
        {
            List<Unit> selectedUnits = new();

            int unitsToApply = Mathf.Min(unitCount, availableUnits.Count);

            if (availableUnits.Count <= unitCount)
            {
                selectedUnits.AddRange(availableUnits);
            }
            else
            {
                while (selectedUnits.Count < unitsToApply)
                {
                    Unit selectedUnit = availableUnits[Random.Range(0, availableUnits.Count)];

                    if (!selectedUnits.Contains(selectedUnit))
                    {
                        selectedUnits.Add(selectedUnit);
                    }
                }
            }

            foreach (Unit selectedUnit in selectedUnits)
            {
                UnitEvent unitEvent = selectedUnit.GetComponent<UnitEvent>();
                unitEvent.ApplySyntaxErrorUnit(hitChanceDecrease, damageDecrease, duration);
            }

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsSkill0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsSkill1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsSkill2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsSkill3);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsSkill5);
        }
    }

    public void ApplyParasiticDebuff(int unitCount, float damage, float duration, float interval)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill4);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill5);
        List<Unit> availableUnits = new List<Unit>();

        foreach (var fieldUnit in FieldManager.Inst.fieldUnits)
        {
            if (fieldUnit != null && !fieldUnit.GetComponent<UnitEvent>().IsParasitic && !fieldUnit.GetComponent<UnitEvent>().IsInvulnerable && !fieldUnit.GetComponent<UnitEvent>().IsIgnored)
            {
                availableUnits.Add(fieldUnit.GetComponent<Unit>());
            }
        }

        if (availableUnits.Count > 0)
        {
            List<Unit> selectedUnits = new();

            int unitsToApply = Mathf.Min(unitCount, availableUnits.Count);

            if (availableUnits.Count <= unitCount)
            {
                selectedUnits.AddRange(availableUnits);
            }
            else
            {
                while (selectedUnits.Count < unitsToApply)
                {
                    Unit selectedUnit = availableUnits[Random.Range(0, availableUnits.Count)];

                    if (!selectedUnits.Contains(selectedUnit))
                    {
                        selectedUnits.Add(selectedUnit);
                    }
                }
            }

            foreach (Unit selectedUnit in selectedUnits)
            {
                UnitEvent unitEvent = selectedUnit.GetComponent<UnitEvent>();
                unitEvent.ApplyParasiticUnit(damage * 0.5f, duration, interval);
            }

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill1);
            // AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill3);
        }
    }
}
