using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieSkill", menuName = "Unit Skill Data/PieSkill")]
public class PieSkill : UnitSkillBase<PieSkill.PieRuntimeSkill, IUnitSkill>
{
    public class PieRuntimeSkill : IUnitSkill, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            List<Unit> fieldUnits = FieldManager.Inst.GetFieldBatchUnits();
            if (fieldUnits == null || fieldUnits.Count == 0)
                return;

            foreach (Unit fieldUnit in fieldUnits)
            {
                if (fieldUnit == null || !fieldUnit.isLive)
                    continue;

                if (fieldUnit.health < fieldUnit.maxHealth)
                {
                    float healAmount = (float)unit.damageAmout * (1 + unit.skillAmount);
                    fieldUnit.unitEvent.ApplyGetHeal(healAmount, unit.healingMultiplier, fieldUnit.healingReceivedMultiplier, unit.unitData);
                }
            }

        }

        public void ApplyOnDiePassive()
        {

        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
