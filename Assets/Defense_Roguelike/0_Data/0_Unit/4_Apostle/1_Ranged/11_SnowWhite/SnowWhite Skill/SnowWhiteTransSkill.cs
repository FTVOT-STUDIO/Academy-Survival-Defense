using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SnowWhiteTransSkill", menuName = "Unit Skill Data/SnowWhiteTransSkill")]
public class SnowWhiteTransSkill : UnitSkillBase<SnowWhiteTransSkill.SnowWhiteTransRuntimeSkill, IUnitSkill>
{
    public class SnowWhiteTransRuntimeSkill : IUnitSkill, ICoolTimeOverride, IAttackPassive, IOnEnablePassive
    {
        Unit unit;
        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            unit.shield = unit.maxShield;
            unit.unitWeapon.skillTime = unit.coolTime;
            unit.unitWeapon.SetSkillCoroutine(SkillCooldownCoroutine(unit));
            unit.anim.SetTrigger("Attack");
        }


        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Vector3 targetPos = unit.unitScanner.nearestTarget.position;
            double damage = unit.damageAmout * (unit.unitScanner.nearestTarget.GetComponent<Enemy>().weaknessValue * 2);

            if (unit.hitCount == 3)
            {
                unit.unitEvent.ApplyRangeStats(0.3f, true);
            }
            else if (unit.hitCount >= 4)
            {
                unit.unitEvent.ApplyRangeStats(0.3f, false);
                unit.hitCount = 0;
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteAttack4);
            }


            AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteAttack0);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.2f))
            {
                LaserBullet laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.SnowWhiteLaser, unit.gameObject);
                laserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, damage, unit.criticalPer, unit.criticalDamage, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillAmount, 0.3f);
                laserBullet.Launch(targetPos, Bullet.BulletOwnerType.Unit);
                laserBullet.objectLifecycleManager.SetDelay(1f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteAttack1);
                EffectManager.Inst.Get(EffectManager.PrefabType.SnowWhiteAttackEffec1, targetPos);

            }
            else if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.4f))
            {
                LaserBullet circleLaserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.SnowWhiteCircleLaser, unit.gameObject);
                circleLaserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, damage, unit.criticalPer, unit.criticalDamage, unit.areaDamage, 0, 0, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier, 0.5f);
                circleLaserBullet.Launch(targetPos, Bullet.BulletOwnerType.Unit);

                circleLaserBullet.objectLifecycleManager.SetDelay(3f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteAttack3);
                EffectManager.Inst.Get(EffectManager.PrefabType.SnowWhiteAttackEffec1, targetPos);
            }
            else
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, damage, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillAmount);
                bullet.Launch(unit.unitScanner.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 3, 0.1f);
            }

            EffectManager.Inst.Get(EffectManager.PrefabType.SnowWhiteAttackEffect0, unit.unitScanner.nearestTarget.position);



        }

        public void ApplyICoolTimeOverride(MonoBehaviour monoBehaviour) { }

        IEnumerator SkillCooldownCoroutine(Unit unit)
        {
            while (unit.unitWeapon.skillTime > 0)
            {
                //  while (GameManager.Inst.isIntroDelay)
                //  yield return null;
                unit.unitWeapon.skillTime = Mathf.Max(unit.unitWeapon.skillTime - Time.deltaTime * 0.5f, 0);
                unit.UpdateManaBar();
                yield return null;
            }

            unit.TransInit(unit.unitData.transData, unit.starLevel);
            Vector3 effectPos = new Vector3(unit.transform.localPosition.x, unit.transform.position.y + 0.32f, unit.transform.localPosition.z);
            EffectManager.Inst.Get(EffectManager.PrefabType.SnowWhiteTransEffect, effectPos);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.SnowWhiteTrans2);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
