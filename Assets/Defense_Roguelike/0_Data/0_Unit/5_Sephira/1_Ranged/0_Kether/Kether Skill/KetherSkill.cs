using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "KetherSkill", menuName = "Unit Skill Data/KetherSkill")]
public class KetherSkill : SephiraSkillBase<KetherSkill.KetherRuntimeSkill, ISephiraSkill>
{
    public class KetherRuntimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive
    {
        SephiraUnit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.nearestTarget.GetComponent<Enemy>();
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Attack1);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.5f))
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * (1 + unit.skillAmount), unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
                bullet.SetDebuffInfo(Bullet.DebuffState.Slow, 0.8f, 3);
                bullet.SetVisualEffect(EffectManager.PrefabType.KetherSkillEffect);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 1, 0.1f);
            }
            else
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 1, 0.1f);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {

            monoBehaviour.StartCoroutine(OnSkill(unit, monoBehaviour));
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameWeakness(0.3f);
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            Bullet startBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            startBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, 0, 0, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            startBullet.SetDebuffInfo(Bullet.DebuffState.Slow, 0.5f, 1.5f);
            startBullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 999, 0.1f);

            Vector3 effectPos = new Vector3(unit.transform.position.x, unit.transform.position.y + 2, unit.transform.position.z);
            EffectManager.Inst.Get(EffectManager.PrefabType.ketherEffect, effectPos);

            CameraShake.Inst.Shake(1, 1, unit.abllityApplyTime);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Gravity);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.vibration);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.KetherAbility2);
            yield return new WaitForSeconds(unit.abllityApplyTime);

            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, Color.white);

            Bullet endBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            Bullet dotBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            Bullet stunBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            endBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 10, 100, unit.criticalDamage * 2, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            endBullet.SetDebuffInfo(Bullet.DebuffState.Slow, unit.skillAmount, 5);
            endBullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 999, 0.1f);

            dotBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 10, 100, unit.criticalDamage * 2, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            dotBullet.SetDebuffInfo(Bullet.DebuffState.Dot, 5, 0.3f, unit.damageAmout);
            dotBullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 999, 0.1f);

            stunBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 10, 100, unit.criticalDamage * 2, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            stunBullet.SetDebuffInfo(Bullet.DebuffState.Stun, unit.skillAmount, 2f);
            stunBullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 999, 0.1f);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Boomb);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.KetherAbility1);
        }
    }

}
