using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MalkuthSkill", menuName = "Unit Skill Data/MalkuthSkill")]
public class MalkuthSkill : SephiraSkillBase<MalkuthSkill.MalkuthRumtimeSkill, ISephiraSkill>
{
    public class MalkuthRumtimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive, IOnDisablePassive
    {
        SephiraUnit unit;
        private Action cheatCallback;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 0.1f, 0.1f);

            if (unit.hitCount > 0 && unit.hitCount % 5 == 0)
            {
                FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(1);
            }

            if (unit.hitCount > 0 && unit.hitCount % 10 == 0)
            {
                unit.baseAttackSpeed += 0.1f;
                EffectManager.Inst.Get(EffectManager.PrefabType.MalkuthUpgradeEffect, unit.transform.position);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.malkuthAttackSpeed);
            }
        }
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            RemoveCheatAction();
            unit.TransInit(unit.unitData.transData);
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            SetupCheatAction(unit);
            GameStatManager.Inst.AddGameAttackSpeed(0.2f);
        }

        public void SetupCheatAction(SephiraUnit unit)
        {
            RemoveCheatAction();
            cheatCallback = () => unit.sephiraUnitEvent.MultiplyAbilityStats(0.1f);
            GameManager.Inst.cheatCodeManager.onCheatActivated += cheatCallback;
        }


        public void RemoveCheatAction()
        {
            GameManager.Inst.cheatCodeManager.onCheatActivated -= cheatCallback;
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameAttackSpeed(-0.2f);
        }
    }
}
