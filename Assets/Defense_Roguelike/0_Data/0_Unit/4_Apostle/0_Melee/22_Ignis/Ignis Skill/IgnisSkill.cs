using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(fileName = "IgnisSkill", menuName = "Unit Skill Data/IgnisSkill")]
public class IgnisSkill : UnitSkillBase<IgnisSkill.IgnisRuntimeSkill, IUnitSkill>, IUnitManagerFactory
{
    public void CreateUnitManager()
    {
        GameObject manager = PoolManager.Inst.Get(PoolManager.PrefabType.IgnisTransManager, Vector3.zero);
        manager.transform.SetParent(GameManager.Inst.unitManagerGroup);
    }

    public class IgnisRuntimeSkill : IUnitSkill, IOnEnablePassive, IOnDisablePassive, ITriggerBeforeTransform, ITriggerAfterTransform
    {
        Unit unit;

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameDamage(0.1f);
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameDamage(-0.1f);
        }


        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public void OnTriggerBeforeTransform()
        {
            UnitData transData = unit.unitData.transData;
            UnitData normalData = unit.unitData;

            List<Unit> overlapUnits = FieldManager.Inst.GetOverlapUnit(unit.unitData.unitID, unit);

            var allUnits = new List<Unit>(overlapUnits.Count + 1) { unit };
            allUnits.AddRange(overlapUnits);

            bool shouldTransform = IgnisTransManager.Inst.IsEvenUnitCountIncludingSelf(unit);

            if (shouldTransform)
            {
                for (int i = 0; i < allUnits.Count; i++)
                {
                    allUnits[i].TransInit(transData, allUnits[i].starLevel, false);
                }
                IgnisTransManager.Inst.PlayTransOnEffect(unit);
            }
            else
            {
                for (int i = 0; i < allUnits.Count; i++)
                {
                    allUnits[i].TransInit(normalData, allUnits[i].starLevel, false);
                }
                IgnisTransManager.Inst.PlayTransOffEffect(unit);
            }


            // UnitUtility.TriggerBeforeTransform(unit, IgnisTransManager.Inst, unit.unitData, unit.unitData.transData, EffectManager.PrefabType.IgnisTransOnEffect, EffectManager.PrefabType.IgnisTransOffEffect, AudioManager.Sfx.IgnisTransOn0, AudioManager.Sfx.IgnisTransOff, IgnisTransManager.Inst.IsEvenUnitCountIncludingSelf(unit));
        }

        public void OnTriggerAfterTransform()
        {
            UnitData transData = unit.unitData.transData;
            UnitData normalData = unit.unitData;

            List<Unit> overlapUnits = FieldManager.Inst.GetOverlapUnit(unit.unitData.unitID, unit);

            bool shouldTransform = IgnisTransManager.Inst.IsEvenOverlapUnitCount(unit);

            if (shouldTransform)
            {
                for (int i = 0; i < overlapUnits.Count; i++)
                {
                    overlapUnits[i].TransInit(transData, overlapUnits[i].starLevel, false);
                }
                IgnisTransManager.Inst.PlayTransOnEffect(unit);
            }
            else
            {
                for (int i = 0; i < overlapUnits.Count; i++)
                {
                    overlapUnits[i].TransInit(normalData, overlapUnits[i].starLevel, false);
                }
                IgnisTransManager.Inst.PlayTransOffEffect(unit);
            }

            // UnitUtility.TriggerBeforeTransform(unit, IgnisTransManager.Inst, unit.unitData, unit.unitData.transData, EffectManager.PrefabType.IgnisTransOnEffect, EffectManager.PrefabType.IgnisTransOffEffect, AudioManager.Sfx.IgnisTransOn0, AudioManager.Sfx.IgnisTransOff, IgnisTransManager.Inst.IsEvenOverlapUnitCount(unit));
        }

        // IEnumerator TriggerBeforeTransformCoroutine()
        // {
        //     yield return null;
        //     IgnisTransManager.Inst.IsTransformationInProgress = false;

        //     UnitData normalData = unit.unitData;
        //     UnitData transData = unit.unitData.transData;

        //     unit.isTrans = false;
        //     List<Unit> overlapUnits = FieldManager.Inst.GetOverlapUnit(unit.unitData.unitID, unit);

        //     var allUnits = new List<Unit>(overlapUnits.Count + 1) { unit };
        //     allUnits.AddRange(overlapUnits);

        //     bool shouldTransform = allUnits.Count % 2 == 0;

        //     foreach (var u in allUnits)
        //     {
        //         if (shouldTransform)
        //         {
        //             u.TransInit(transData, false);
        //             u.isTrans = true;
        //             EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOnEffect, u.transform.position);
        //         }
        //         else if (!shouldTransform)
        //         {
        //             u.TransInit(normalData, false);
        //             u.isTrans = false;
        //             EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOffEffect, u.transform.position);
        //         }
        //     }
        //     if (shouldTransform)
        //         AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisTransOn0);
        //     else if (!shouldTransform)
        //         AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisTransOff);
        // }

        // IEnumerator OnTriggerAfterTransformCoroutine()
        // {
        //     yield return null;
        //     IgnisTransManager.Inst.IsTransformationInProgress = false;

        //     UnitData normalData = unit.unitData;
        //     UnitData transData = unit.unitData.transData;

        //     unit.isTrans = false;
        //     List<Unit> overlapUnits = FieldManager.Inst.GetOverlapUnit(unit.unitData.unitID, unit);

        //     bool shouldTransform = overlapUnits.Count % 2 == 0;

        //     FieldManager.Inst.RemoveBatchUnit(unit.gameObject);
        //     foreach (var u in overlapUnits)
        //     {
        //         if (shouldTransform)
        //         {
        //             u.TransInit(transData, false);
        //             u.isTrans = true;
        //             EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOnEffect, u.transform.position);
        //         }
        //         else if (!shouldTransform)
        //         {
        //             u.TransInit(normalData, false);
        //             u.isTrans = false;
        //             EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOffEffect, u.transform.position);
        //         }
        //     }

        //     if (shouldTransform)
        //         AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisTransOn0);
        //     else if (!shouldTransform)
        //         AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisTransOff);

        // }
    }
}
