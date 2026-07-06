using UnityEngine;

[CreateAssetMenu(fileName = "HodSkill", menuName = "Unit Skill Data/HodSkill")]
public class HodSkill : SephiraSkillBase<HodSkill.HodRumtimeSkill, ISephiraSkill>
{
    public class HodRumtimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IKillEvent, IOnEnablePassive
    {
        SephiraUnit unit;

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            unit.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, 0, 0, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, unit.nearestTarget.GetComponent<Enemy>(), UnitUtility.DamageType.Fixed);
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            LaserBullet laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.HodAbilityBullet, unit.gameObject);

            laserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier, 0.2f);
            laserBullet.Launch(unit.transform.position, Bullet.BulletOwnerType.Sephira);

            if (unit.nearestTarget != null)
            {
                unit.nearestTarget.GetComponent<Enemy>().enemyEvent.ApplyDebuff(Bullet.DebuffState.Stun, 0.1f, 3, unit.damageAmout);
            }

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodAbility0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodAbility1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodScream);
        }

        public void ApplyKillEvent(MonoBehaviour monoBehaviour)
        {
            Vector3 currentScale = unit.visualGroupObject.transform.localScale;
            float scaleFactor = 0.01f;

            unit.visualGroupObject.transform.localScale = currentScale + new Vector3(scaleFactor, scaleFactor, scaleFactor);

            unit.baseDamage += 0.3f;
            unit.killCount++;

            if (unit.killCount % 10 == 0 && unit.killCount != 0 && FieldManager.Inst.fieldUnitCount < FieldManager.Inst.fieldUnits.Length)
            {
                Unit summon = GameManager.Inst.unitSpawn.GenerateUnit(unit.unitData.summonedUnits[0]);

                EffectManager.Inst.Get(EffectManager.PrefabType.HoldSummonSpawnEffect, summon.transform.position);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodSummon0);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodSummon1);
            }

            {
                GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.HodKillEffect, unit.transform.position);
                effect.transform.SetParent(unit.buffPrefabParent);
            }
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodKill);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodScream);

        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameDamage(0.5f);
            unit.unitData.summonedUnits[0].cardLevel = unit.unitData.cardLevel;
            unit.unitData.summonedUnits[0].InitUnitState();

            Color c = unit.sp.color;
            c.a = 0.8f;
            unit.sp.color = c;
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
