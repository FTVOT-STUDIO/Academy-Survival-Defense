using UnityEngine;

[CreateAssetMenu(fileName = "The516thBatterSkill", menuName = "Unit Skill Data/The516thBatterSkill")]
public class The516thBatterSkill : UnitSkillBase<The516thBatterSkill.The516thBatterRumtimeSkill, IUnitSkill>
{
    public class The516thBatterRumtimeSkill : IUnitSkill, IAttackPassive, ICoolTimeSkill, IDamageModifier, IOnEnablePassive
    {
        Unit unit;
        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.BaseBallIntro);
        }

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();

            if (unit.hitCount > 0 && unit.hitCount % 4 == 0)
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout * 1.5f, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy, UnitUtility.DamageType.Fixed, hitSound: AudioManager.Sfx.BatHit);
            }
            else
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy, UnitUtility.DamageType.Fixed);
            }

        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 10, 100, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
            bullet.SetVisualEffect(EffectManager.PrefabType.EnemyHitEffect, true, AudioManager.Sfx.HomeRun);
            bullet.Launch(unit.unitScanner.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 2, 0.1f);

            EffectManager.Inst.Get(unit.unitData.effectPrefab, unit.transform.position);
            //AudioManager.Inst.PlaySfx(AudioManager.Sfx.HomeRun);
        }

        public float GetDamageMultiplier()
        {
            return 1 / unit.attackSpeed;
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
