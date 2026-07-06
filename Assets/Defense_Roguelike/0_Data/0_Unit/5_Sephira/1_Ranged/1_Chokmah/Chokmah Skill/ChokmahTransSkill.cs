using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[CreateAssetMenu(fileName = "ChokmahTransSkill", menuName = "Unit Skill Data/ChokmahTransSkill")]
public class ChokmahTransSkill : SephiraSkillBase<ChokmahTransSkill.ChokmahTransRumtimeSkill, ISephiraSkill>
{
    public class ChokmahTransRumtimeSkill : ISephiraSkill, IOnEnableSkill, IAttackPassive
    {
        SephiraUnit unit;
        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);

            bullet.SetDebuffInfo(Bullet.DebuffState.Dot, 0.3f, 3, unit.damageAmout);
            bullet.SetVisualEffect(EffectManager.PrefabType.KetherSkillEffect, false);
            bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 0.1f, 0.1f);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.DotDamage);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {

                List<Unit> candidates = FieldManager.Inst.GetFieldBatchUnits()
                    .Where(u => u.unitSkill is ICoolTimePassive || u.unitSkill is ICoolTimeSkill)
                    .ToList();

                if (candidates.Count == 0)
                    return;

                List<Unit> selectedUnits = candidates
                    .OrderBy(u => Random.value)
                    .Take(3)
                    .ToList();

                foreach (Unit targetUnit in selectedUnits)
                {
                    targetUnit.unitEvent.ApplyGetCoolTime(targetUnit.coolTime);

                    EffectManager.Inst.Get(
                        EffectManager.PrefabType.ChokmahManaRegenEffect,
                        targetUnit.transform.position);

                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.ChokmahManaRegen);
                }
            }


        }

        public void ApplyOnEnableSkill(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(OnSkill(unit));
        }

        IEnumerator OnSkill(SephiraUnit unit)
        {
            GameStatManager.Inst.AddGameCoolTime(0.5f);

            EffectManager.Inst.Get(EffectManager.PrefabType.ChokmahEffect, unit.transform.position);

            for (int i = 0; i < FieldManager.Inst.GetFieldBatchUnits().Count; i++)
            {
                Unit fieldUnit = FieldManager.Inst.GetFieldBatchUnits()[i];

                fieldUnit.unitWeapon.skillTime = fieldUnit.unitData.coolTime;
                Vector3 effectPos = new Vector3(fieldUnit.transform.position.x, fieldUnit.transform.position.y + 0.5f, fieldUnit.transform.position.z);
            }
            yield return new WaitForSeconds(unit.abllityApplyTime);
            GameStatManager.Inst.AddGameCoolTime(-0.5f);
            float curHealth = unit.health;
            unit.TransInit(unit.unitData.transData);
            unit.health = curHealth;
            EffectManager.Inst.Get(EffectManager.PrefabType.ZoomojiTransFailEffect, unit.transform.position);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Gas);
        }
    }



}
