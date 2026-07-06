using UnityEngine;

[CreateAssetMenu(fileName = "ShockBeatSkill", menuName = "Unit Skill Data/ShockBeatSkill")]
public class ShockBeatSkill : UnitSkillBase<ShockBeatSkill.ShockBeatRuntimeSkill, IUnitSkill>
{
    public class ShockBeatRuntimeSkill : IUnitSkill, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalDamage, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
            bullet.SetDebuffInfo(Bullet.DebuffState.Stun, unit.skillAmount, 2);
            bullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 3, 0.1f);

            EffectManager.Inst.Get(EffectManager.PrefabType.ShockBeatSkillEffect, unit.transform.position);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ShockBeatSkill);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.DJ);

        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
