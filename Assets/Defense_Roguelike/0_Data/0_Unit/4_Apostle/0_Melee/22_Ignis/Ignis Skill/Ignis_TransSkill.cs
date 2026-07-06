using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ignis_TransSkill", menuName = "Unit Skill Data/Ignis_TransSkill")]
public class Ignis_TransSkill : UnitSkillBase<Ignis_TransSkill.IgnisTransRuntimeSkill, IUnitSkill>
{
    public class IgnisTransRuntimeSkill : IUnitSkill, IOnEnablePassive, IOnDisablePassive, ITriggerBeforeTransform, ITriggerAfterTransform, IAttackPassive, ICoolTimeSkill
    {
        Unit unit;

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameDamage(0.3f);
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameDamage(-0.3f);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public void OnTriggerBeforeTransform()
        {
            UnitData transData = unit.unitData;
            UnitData normalData = unit.unitData.transData;

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
            // UnitUtility.TriggerBeforeTransform(unit, IgnisTransManager.Inst, unit.unitData.transData, unit.unitData, EffectManager.PrefabType.IgnisTransOnEffect, EffectManager.PrefabType.IgnisTransOffEffect, AudioManager.Sfx.IgnisTransOn0, AudioManager.Sfx.IgnisTransOff, IgnisTransManager.Inst.IsEvenUnitCountIncludingSelf(unit));
        }

        public void OnTriggerAfterTransform()
        {
            UnitData transData = unit.unitData;
            UnitData normalData = unit.unitData.transData;

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

            // UnitUtility.TriggerAfterTransform(unit, IgnisTransManager.Inst, unit.unitData.transData, unit.unitData, EffectManager.PrefabType.IgnisTransOnEffect, EffectManager.PrefabType.IgnisTransOffEffect, AudioManager.Sfx.IgnisTransOn0, AudioManager.Sfx.IgnisTransOff, IgnisTransManager.Inst.IsEvenOverlapUnitCount(unit));
        }

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.GetCurrentTarget();
            if (enemy == null)
                return;
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.SetVisualEffect(EffectManager.PrefabType.IgnisNormalAttackEffect);
            bullet.Launch(enemy.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 1, 0.1f);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                Bullet enforceBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

                enforceBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 3, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                enforceBullet.SetVisualEffect(EffectManager.PrefabType.IgnisSuperAttackEffect);
                enforceBullet.Launch(enemy.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 2, 0.1f);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisSuperAttack);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            CoroutineRunner.Inst.RunCoroutine(OnSkill());
        }

        IEnumerator OnSkill()
        {
            Enemy enemy = unit.unitScanner.GetCurrentTarget();
            if (enemy == null)
                yield break;

            Vector3 targetPos = unit.unitScanner.GetCurrentTarget().transform.position;
            LaserBullet laserBullet;
            if (enemy.enemyMove.nextVec.x != 0)
            {
                laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.IgnisSkillHorizontalBullet, unit.gameObject);
            }
            else
            {
                laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.IgnisSkillVerticalBullet, unit.gameObject);

            }

            laserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration + 0.2f, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            laserBullet.Launch(targetPos, Bullet.BulletOwnerType.Unit);
            laserBullet.objectLifecycleManager.SetDelay(3);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisSkill0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisSkill1);
            yield return new WaitForSeconds(3);

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 5, 100, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            bullet.Launch(targetPos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 4, 0.1f);
            EffectManager.Inst.Get(EffectManager.PrefabType.IgnisBombEffect, targetPos);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.IgnisSkill2);
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
        //             u.TransInit(normalData, false);
        //             u.isTrans = true;
        //             EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOnEffect, u.transform.position);
        //         }
        //         else if (!shouldTransform)
        //         {
        //             u.TransInit(transData, false);
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
        //             u.TransInit(normalData, false);
        //             u.isTrans = true;
        //             EffectManager.Inst.Get(EffectManager.PrefabType.IgnisTransOnEffect, u.transform.position);
        //         }
        //         else if (!shouldTransform)
        //         {
        //             u.TransInit(transData, false);
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

