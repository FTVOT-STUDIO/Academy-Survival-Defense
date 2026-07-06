using UnityEngine;

[CreateAssetMenu(fileName = "SnowWhiteSkill", menuName = "Unit Skill Data/SnowWhiteSkill")]
public class SnowWhiteSkill : UnitSkillBase<SnowWhiteSkill.SnowWhiteRuntimeSkill, IUnitSkill>
{
    public class SnowWhiteRuntimeSkill : IUnitSkill, ICoolTimeOverride, IAttackPassive, IButtonAttckPassive, IButtonInfoinitialize
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.Launch(unit.unitScanner.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 0.8f, 0.1f);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.2f))
            {
                unit.unitWeapon.skillTime += 5;
            }
        }

        public void ApplyButtonAttckPassive(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.skillTime >= unit.coolTime)
            {
                unit.TransInit(unit.unitData.transData, unit.starLevel);

                Vector3 effectPos = new Vector3(unit.transform.position.x, unit.transform.position.y + 0.32f, unit.transform.position.z);
                EffectManager.Inst.Get(EffectManager.PrefabType.SnowWhiteTransEffect, effectPos);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteTrans0);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteTrans1);
            }
        }


        public void ApplyICoolTimeOverride(MonoBehaviour monoBehaviour)
        {
        }

        public void InitializeSkillButtonUI(MonoBehaviour monoBehaviour)
        {
            unit.unitInfo.InitializeSkillButton("슈트 장착");
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}


