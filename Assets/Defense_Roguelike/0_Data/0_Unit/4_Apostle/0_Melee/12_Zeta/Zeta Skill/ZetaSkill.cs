using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZetaSkill", menuName = "Unit Skill Data/ZetaSkill")]
public class ZetaSkill : UnitSkillBase<ZetaSkill.ZetaRumtimeSkill, IUnitSkill>
{
    public class ZetaRumtimeSkill : IUnitSkill, IAttackPassive, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.2f))
            {
                unit.unitScanner.nearestTarget.GetComponent<Enemy>().enemyEvent.TryWeakness(unit.skillAmount, 2);
            }

            unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout * (1 + (enemy.enemyEvent.debuffCount * 0.1)), unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy);
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            GameManager.Inst.StartCoroutine(OnSkill(unit));
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill(Unit unit)
        {
            Enemy target = unit.unitScanner.nearestTarget.GetComponent<Enemy>();
            Vector3 targetPos = unit.unitScanner.nearestTarget.GetComponent<Enemy>().transform.position;
            float debuffBonus = 1f + (target.enemyEvent.debuffCount * 0.1f);
            float weaknessBonus = target.weaknessValue;
            float finalMultiplier = debuffBonus * weaknessBonus;

            Bullet gravityPullBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            gravityPullBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * finalMultiplier, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillAmount);
            gravityPullBullet.SetVisualEffect(EffectManager.PrefabType.LunaEffect, true, AudioManager.Sfx.GlassBreak);

            gravityPullBullet.SetDebuffInfo(Bullet.DebuffState.Slow, 0.5f, 2f);
            gravityPullBullet.Launch(targetPos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 2, 0.1f);

            EffectManager.Inst.Get(EffectManager.PrefabType.ZetaEffect, target.transform.position);
            //AudioManager.Inst.PlaySfx(AudioManager.Sfx.GlassBreak);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Fire);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ZetaAbility);
            yield return new WaitForSeconds(1f);

            Bullet endBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            endBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * finalMultiplier, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.skillAmount);
            endBullet.SetVisualEffect(EffectManager.PrefabType.LunaEffect, true, AudioManager.Sfx.GlassBreak);
            endBullet.SetDebuffInfo(Bullet.DebuffState.Weakness, unit.skillAmount, 2);
            endBullet.Launch(targetPos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 3, 0.1f);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.FingerSnap);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXIntro2);
        }
    }
}
