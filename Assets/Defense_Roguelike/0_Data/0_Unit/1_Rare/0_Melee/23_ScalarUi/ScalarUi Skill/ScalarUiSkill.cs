using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "ScalarUiSkill", menuName = "Unit Skill Data/ScalarUiSkill")]
public class ScalarUiSkill : UnitSkillBase<ScalarUiSkill.ScalarUiRuntimeSkill, IUnitSkill>
{
    public class ScalarUiRuntimeSkill : IUnitSkill, ICoolTimeSkill, IOnUnitDeathOverride
    {
        Unit unit;
        bool isLive = true;

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            if (!isLive)
                return;

            float damageAmount = unit.maxHealth * 0.2f;

            float minHealthLimit = 1f;

            if (unit.health - damageAmount < minHealthLimit)
            {
                damageAmount = unit.health - minHealthLimit;
            }

            unit.unitEvent.TakeDamageInstant(damageAmount);

            unit.health = Mathf.Max(unit.health, 1f);

            Enemy target = unit.unitScanner.GetCurrentTarget();
            if (target == null)
                return;

            unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout + (unit.maxHealth - unit.health) * 1.2f, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Skill, unit.skillMultiplier, target, UnitUtility.DamageType.Fixed);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUiSkill);
        }

        public void ApplyOnUnitDeathOverride(MonoBehaviour monoBehaviour)
        {
            if (!isLive)
                return;
            monoBehaviour.StartCoroutine(OnSkill());
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
            isLive = true;
        }

        IEnumerator OnSkill()
        {
            isLive = false;
            unit.unitDrag.isInputLocked = true;
            unit.anim.runtimeAnimatorController = unit.unitData.overrideAnimator;
            unit.unitEvent.ActivateDebuffClear();
            unit.unitEvent.ApplyInvulnerable(5);
            unit.unitEvent.ApplyCriticalPerStats(0.5f, true);
            unit.unitEvent.ApplyAttackSpeedStats(4, true);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXIntro2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill5);

            yield return new WaitForSeconds(5);
            unit.Dead();
        }
    }
}
