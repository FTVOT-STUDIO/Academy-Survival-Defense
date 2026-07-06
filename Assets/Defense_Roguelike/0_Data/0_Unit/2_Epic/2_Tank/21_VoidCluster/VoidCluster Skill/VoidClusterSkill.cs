using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidClusterSkill", menuName = "Unit Skill Data/VoidClusterSkill")]
public class VoidClusterSkill : UnitSkillBase<VoidClusterSkill.VoidClusterRumtimeSkill, IUnitSkill>
{
    public class VoidClusterRumtimeSkill : IUnitSkill, IAttackPassive, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy targetEnemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, 0, 0, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetDebuffInfo(Bullet.DebuffState.Stun, unit.skillAmount, 1);
                bullet.SetVisualEffect(EffectManager.PrefabType.VoidClusterStunEffect);
                bullet.Launch(targetEnemy.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 1, 0.1f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.VoidClusterStun);
            }
            else
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, targetEnemy);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(OnSkill(unit));
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill(Unit unit)
        {
            unit.unitEvent.ApplySkillTriggerChanceState(0.1f, true);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.VoidClusterSkill);
            yield return new WaitForSeconds(10);
            unit.unitEvent.ApplySkillTriggerChanceState(0.1f, false);
        }

    }
}
