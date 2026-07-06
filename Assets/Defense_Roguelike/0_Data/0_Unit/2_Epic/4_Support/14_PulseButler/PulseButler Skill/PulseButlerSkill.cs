using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "PulseButlerSkill", menuName = "Unit Skill Data/PulseButlerSkill")]
public class PulseButlerSkill : UnitSkillBase<PulseButlerSkill.PulseButlerRuntimeSkill, IUnitSkill>
{
    public class PulseButlerRuntimeSkill : SingleSupportSkillBase, IUnitSkill
    {
        Unit unit;

        public override void ApplySupport(Unit unit, Unit targetUnit)
        {
            curValue = unit.skillAmount;
            targetUnit.unitEvent.ApplyDamageStats(curValue, true);
            unit.unitEvent.unitEventHandler = () => AttackSupport(targetUnit);

            targetUnit.unitWeapon.OnAttack += unit.unitEvent.unitEventHandler;
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public override void ClearSupport(Unit unit, Unit targetUnit)
        {
            targetUnit.unitEvent.ApplyDamageStats(curValue, false);
            targetUnit.unitWeapon.OnAttack -= unit.unitEvent.unitEventHandler;
        }

        void AttackSupport(Unit targetUnit)
        {
            if (targetUnit.unitScanner.nearestTarget != null || targetUnit.unitScanner.lowestHealthTarget != null)
            {
                targetUnit.StartCoroutine(OnSkill(targetUnit));
            }
        }

        IEnumerator OnSkill(Unit targetUnit)
        {
            if (targetUnit == null || targetUnit.unitScanner == null)
                yield break;

            for (int i = 0; i < unit.starLevel; i++)
            {
                yield return new WaitForSeconds(0.2f);

                bool targetExists =
                    targetUnit.unitScanner.nearestTarget != null ||
                    targetUnit.unitScanner.nearestUnit != null ||
                    targetUnit.unitScanner.lowestHealthTarget != null;

                if (!targetExists)
                    yield break;

                targetUnit.unitWeapon.UnitAttack();
            }
        }
    }
}


