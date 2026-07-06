using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "TsubasaShiraiSkill", menuName = "Unit Skill Data/TsubasaShiraiSkill")]
public class TsubasaShiraiSkill : UnitSkillBase<TsubasaShiraiSkill.TsubasaShiraiRumtimeSkill, IUnitSkill>
{
    public class TsubasaShiraiRumtimeSkill : IUnitSkill, IAttackPassive, ICriticalDamageModifier
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.nearestTarget.transform.GetComponent<Enemy>();
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            if (Random.value < 0.25f)
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 2, 100, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetDebuffInfo(Bullet.DebuffState.Weakness, unit.skillAmount, 5);
                bullet.Launch(enemy.transform.position, Bullet.BulletInitMode.Linear, Bullet.BulletType.Single, Bullet.BulletOwnerType.Unit, 0.3f, 5);

                Vector3 effectPos = new Vector3(unit.transform.position.x, unit.transform.position.y + 0.5f, unit.transform.position.z);
                EffectManager.Inst.Get(EffectManager.PrefabType.TsubasaShiraiEffect, effectPos);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ShotGun2);

            }
            else
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.Launch(enemy.transform.position, Bullet.BulletInitMode.Linear, Bullet.BulletType.Single, Bullet.BulletOwnerType.Unit, 0.3f, 5);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ShotGun);
            }

        }

        public float GetCriticalDamageMultiplier()
        {
            float ratio = unit.unitWeapon.skillTime / unit.coolTime;
            return 1.0f + ratio;
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
