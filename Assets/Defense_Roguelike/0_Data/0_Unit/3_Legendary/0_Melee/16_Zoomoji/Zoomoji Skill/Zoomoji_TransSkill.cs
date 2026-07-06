using UnityEngine;

[CreateAssetMenu(fileName = "Zoomoji_TransSkill", menuName = "Unit Skill Data/Zoomoji_TransSkill")]
public class Zoomoji_TransSkill : UnitSkillBase<Zoomoji_TransSkill.Zoomoji_TransRumtimeSkill, IUnitSkill>
{
    public class Zoomoji_TransRumtimeSkill : IUnitSkill, IAttackPassive, IOnEnablePassive, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();
            Vector3 targetPos = enemy.transform.position;

            unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy);
            if (enemy.health / enemy.maxHealth <= 0.05f)
            {
                enemy.enemyEvent.InsDeathAll();
            }

            if (Random.value < UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                BombBullet bombBullet = PoolManager.Inst.bulletGenerator.GetBombBullet(PoolManager.PrefabType.ZoomojiTntBullet, unit.gameObject);
                bombBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 5, unit.criticalPer, unit.criticalDamage, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
                bombBullet.SetDebuffInfo(Bullet.DebuffState.InsDeathAll, (float)(enemy.health / enemy.maxHealth), damage: 0.05f);
                bombBullet.SetVisualEffect(EffectManager.PrefabType.ZoomojiExploEffect, true, AudioManager.Sfx.Explo3);
                bombBullet.Launch(targetPos, Bullet.BulletOwnerType.Unit, 2, 1, 0.1f);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();
            Vector3 targetPos = unit.unitScanner.nearestTarget.position;
            Vector3 spawnPos = new Vector3(targetPos.x, targetPos.y + 2f, targetPos.z);

            LaserBullet laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.ZoomojiAbilityBullet, unit.gameObject);

            laserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 3, unit.criticalPer, unit.criticalDamage, unit.areaDamage, 0.5f, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier, 0.3f);
            laserBullet.SetDebuffInfo(Bullet.DebuffState.InsDeathAll, (float)(enemy.health / enemy.maxHealth), damage: 0.05f);
            laserBullet.Launch(spawnPos, Bullet.BulletOwnerType.Unit);
            laserBullet.objectLifecycleManager.SetDelay(10);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Shotdown);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo4);
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            unit.unitEvent.IncrementSpecialShield();
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
