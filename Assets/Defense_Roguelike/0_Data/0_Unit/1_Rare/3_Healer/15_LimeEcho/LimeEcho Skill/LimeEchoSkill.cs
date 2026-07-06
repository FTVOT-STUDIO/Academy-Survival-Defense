using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LimeEchoSkill", menuName = "Unit Skill Data/LimeEchoSkill")]
public class LimeEchoSkill : UnitSkillBase<LimeEchoSkill.LimeEchoRuntimeSkill, IUnitSkill>
{
    public class LimeEchoRuntimeSkill : IUnitSkill, ICoolTimeSkill
    {
        Unit unit;

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            CoroutineRunner.Inst.RunCoroutine(OnSkill(unit));
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill(Unit unit)
        {
            if (unit.unitScanner.lowestHealthTarget == null)
                yield break;
            Unit lowestHpUnit = unit.unitScanner.lowestHealthTarget.GetComponent<Unit>();
            if (lowestHpUnit == null)
                yield break;

            Transform effect = EffectManager.Inst.Get(EffectManager.PrefabType.LimeEchoEffect, lowestHpUnit.transform.position).transform;
            effect.SetParent(lowestHpUnit.transform);
            yield return new WaitForSeconds(1.7f);

            if (lowestHpUnit == null || unit.unitScanner.lowestHealthTarget == null)
                yield break;

            lowestHpUnit.unitEvent.ApplyDurationHealToUnit((float)unit.damageAmout, unit.healingMultiplier, lowestHpUnit.healingReceivedMultiplier, 5, 1f, unit.unitData);
        }

    }
}
