using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "EpicaSkill", menuName = "Unit Skill Data/EpicaSkill")]
public class EpicaSkill : UnitSkillBase<EpicaSkill.EpicaRuntimeSkill, IUnitSkill>
{
    public class EpicaRuntimeSkill : SingleSupportSkillBase, IUnitSkill, IDeActiveSkill
    {
        Unit unit;

        public override void ApplySupport(Unit unit, Unit targetUnit)
        {
            curValue = this.unit.skillAmount;
            targetUnit.unitEvent.ApplyCoolTimeState(curValue, true);

            unit.unitEvent.unitEventHandler = () => SkillSupport(unit, targetUnit);
            targetUnit.unitWeapon.OnSkill += unit.unitEvent.unitEventHandler;
        }

        public override void ClearSupport(Unit unit, Unit targetUnit)
        {
            targetUnit.unitEvent.ApplyCoolTimeState(curValue, false);
            targetUnit.unitWeapon.OnSkill -= unit.unitEvent.unitEventHandler;
        }

        void SkillSupport(Unit unit, Unit targetUnit)
        {
            if (targetUnit.unitScanner.nearestTarget != null || targetUnit.unitScanner.lowestHealthTarget != null)
                OnSkill(unit, targetUnit);
        }

        void OnSkill(Unit unit, Unit targetUnit)
        {
            unit.anim.SetTrigger("Attack");
            targetUnit.unitWeapon.skillTime = targetUnit.coolTime / 10;
            CoroutineRunner.Inst.RunCoroutine(SetUpRocket(unit, targetUnit));
            EffectManager.Inst.Get(EffectManager.PrefabType.EpicaIdeaEffect, unit.transform.position);
        }

        IEnumerator SetUpRocket(Unit unit, Unit targetUnit)
        {
            EffectManager.Inst.Get(EffectManager.PrefabType.EpicaRocketEffect0, unit.transform.position);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Idea2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.EpicaSkill1);

            Vector3 targetPos;

            if (targetUnit.unitData.type == Type.Healer)
                targetPos = targetUnit.unitScanner.lowestHealthTarget.transform.position;
            else if (targetUnit.unitData.type == Type.Support)
                targetPos = unit.transform.position;
            else
                targetPos = targetUnit.unitScanner.nearestTarget.transform.position;

            yield return new WaitForSeconds(2);
            EffectManager.Inst.Get(EffectManager.PrefabType.EpicaRocketEffect1, targetPos);
            yield return new WaitForSeconds(0.2f);

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * (1 + unit.skillAmount), unit.criticalPer * (1 + unit.skillAmount), unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier);
            bullet.SetVisualEffect(EffectManager.PrefabType.LunaEffect, true, AudioManager.Sfx.Crash1);
            bullet.Launch(targetPos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 3, 0.1f);

            EffectManager.Inst.Get(EffectManager.PrefabType.EpicaExplosionEffect, targetPos);

            //  AudioManager.Inst.PlaySfx(AudioManager.Sfx.Crash1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo3);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.EpicaSkill0);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public void ApplyDeActiveSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitScanner.nearestUnit != null)
                CoroutineRunner.Inst.RunCoroutine(SetUpRocket(unit, unit));
        }
    }
}
