using UnityEngine;


[CreateAssetMenu(fileName = "LunaSkill", menuName = "Unit Skill Data/LunaSkill")]
public class LunaSkill : UnitSkillBase<LunaSkill.LunaRuntimeSkill, IUnitSkill>
{
    public class LunaRuntimeSkill : IUnitSkill, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitScanner.nearestTarget == null)
                return;

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * unit.criticalDamage / 10, 100, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
            bullet.SetVisualEffect(EffectManager.PrefabType.LunaEffect);
            bullet.Launch(unit.unitScanner.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 1, 0.1f);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
