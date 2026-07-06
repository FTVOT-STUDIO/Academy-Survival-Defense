using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ChokmahSkill", menuName = "Unit Skill Data/ChokmahSkill")]
public class ChokmahSkill : SephiraSkillBase<ChokmahSkill.ChokmahRumtimeSkill, ISephiraSkill>
{
    public class ChokmahRumtimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive, IOnDisablePassive
    {
        SephiraUnit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, 0, 0, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.Linear, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 0.1f, 5);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Attack2);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                List<Unit> availableUnits = FieldManager.Inst.GetFieldBatchUnits().Where(u => u.unitSkill is ICoolTimePassive || u.unitSkill is ICoolTimeSkill).ToList();

                if (availableUnits.Count == 0)
                    return;

                Unit targetUnit = availableUnits[Random.Range(0, availableUnits.Count)];

                targetUnit.unitEvent.ApplyGetCoolTime(targetUnit.coolTime);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ChokmahManaRegen);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            float curHealth = unit.health;
            unit.TransInit(unit.unitData.transData);
            unit.health = curHealth;
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ChokmahTrans);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsIntro1);
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameCoolTime(-0.5f);
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameCoolTime(0.5f);
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
