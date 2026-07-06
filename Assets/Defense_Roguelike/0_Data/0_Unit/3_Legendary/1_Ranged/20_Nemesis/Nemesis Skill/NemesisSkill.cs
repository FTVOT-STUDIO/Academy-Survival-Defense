using UnityEngine;

[CreateAssetMenu(fileName = "NemesisSkill", menuName = "Unit Skill Data/NemesisSkill")]
public class NemesisSkill : UnitSkillBase<NemesisSkill.NemesisRumtimeSkill, IUnitSkill>
{
    public class NemesisRumtimeSkill : IUnitSkill, IAttackPassive, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy targetEnemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetDebuffInfo(Bullet.DebuffState.Terrified, unit.skillAmount, 1);
                bullet.SetVisualEffect(EffectManager.PrefabType.NemesisTerrifiedEffect);
                bullet.Launch(targetEnemy.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Unit, 1, 0.1f);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NemesisTerrifiedEffect0);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NemesisTerrifiedEffect1);
            }
            else
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, targetEnemy);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Enemy targetEnemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
            bullet.SetDebuffInfo(Bullet.DebuffState.Terrified, unit.skillAmount, 2);
            bullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 4, 0.1f, Bullet.BulletSpeedType.None, action: (enemy) => { enemy.enemyEvent.debuffCount += 2; });

            EffectManager.Inst.Get(EffectManager.PrefabType.NemesisSkillEffect, unit.transform.position);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NemesisSkillEffect0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NemesisSkillEffect1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NemesisSkillEffect2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NemesisSkillEffect3);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
