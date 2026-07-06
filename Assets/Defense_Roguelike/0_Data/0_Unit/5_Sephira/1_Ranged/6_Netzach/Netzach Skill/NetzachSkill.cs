using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NetzachSkill", menuName = "Unit Skill Data/NetzachSkill")]
public class NetzachSkill : SephiraSkillBase<NetzachSkill.NetzachRuntimeSkill, ISephiraSkill>
{
    public class NetzachRuntimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive
    {
        SephiraUnit unit;

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.GetNearestTarget();
            if (enemy == null)
                return;

            Vector3 targetPos = enemy.transform.position;
            float multiplier = 1f;
            bool isHealthRatio = false;

            GameObject attackEffect = EffectManager.Inst.Get(EffectManager.PrefabType.NetzachAttackEffect, unit.transform.position);

            attackEffect.transform.position = targetPos;

            Vector2 dir = enemy.enemyMove.nextVec.normalized;

            if (dir == Vector2.zero)
            {
                attackEffect.transform.rotation = Quaternion.identity;
            }
            else
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                attackEffect.transform.rotation = Quaternion.Euler(0, 0, angle);
            }


            if (unit.nearestTarget == unit.lastNearestTarget)
            {
                multiplier *= 1f + Mathf.Log(unit.sameTargetHitCount + 1, 2f) * 0.2f;
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachSameTarget);
            }

            double finalDamage = unit.damageAmout * multiplier;

            if ((enemy.health / enemy.maxHealth) < 0.3f)
            {
                finalDamage += UnitUtility.CalculateDamage(
                    UnitUtility.DamageType.ByLostHP,
                    0,
                    0.02f,
                    enemy
                );
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachLostHp);
                EffectManager.Inst.Get(EffectManager.PrefabType.NetzachLostHpEffect, targetPos);
                isHealthRatio = true;
            }

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                finalDamage += UnitUtility.CalculateDamage(
                    UnitUtility.DamageType.ByCurrentHP,
                    0,
                    0.03f,
                    enemy
                );
                EffectManager.Inst.Get(EffectManager.PrefabType.NetzachByCurrentHPEffect, targetPos);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachCurHp);
                isHealthRatio = true;
            }

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, finalDamage, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, isHealthRatio);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAttack);

            bullet.Launch(
                targetPos,
                Bullet.BulletInitMode.HitScan,
                Bullet.BulletType.Single,
                Bullet.BulletOwnerType.Sephira,
                0.3f, 0.1f
            );

            if (enemy.health / enemy.maxHealth <= 0.1f)
            {
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachInstDeath);
                enemy.enemyEvent.InsDeathAll();
            }



        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.GetNearestTarget();
            if (enemy == null)
            {
                unit.sephiraUnitEvent.AddAbllityStats(unit.maxAbllityCount);
                return;
            }
            Vector3 targetPos = enemy.transform.position;

            CameraShake.Inst.Shake(2f, 2, 0.1f);
            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0.1f, 0), 0.4f);
            unit.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, 0, unit.criticalDamage, unit.singleTargetDamage, 0, 0, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier, enemy, UnitUtility.DamageType.ByMaxHP, 0.25f);
            EffectManager.Inst.Get(EffectManager.PrefabType.BloodEffect, targetPos);
            float missingHp = unit.maxHealth - unit.health;
            float healAmount = missingHp * 0.5f;
            unit.sephiraUnitEvent.ApplyHeal(healAmount, unit.healingMultiplier, unit.healingReceivedMultiplier);

            enemy.enemyEvent.TryHealben(unit.skillAmount, 60);
            enemy.enemyEvent.TryShieldBen(unit.skillAmount, 60);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility2);
            //AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility3);
            // AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility4);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility5);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.KO);

            unit.anim.SetTrigger("Attack");
            EffectManager.Inst.Get(EffectManager.PrefabType.NetzachAbilityEffect, unit.transform.position);
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameDamage(0.5f);
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }


    }
}
