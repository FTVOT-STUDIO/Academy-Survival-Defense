using UnityEngine;

[CreateAssetMenu(fileName = "HodHod_SummonSkillSkill", menuName = "Unit Skill Data/Hod_SummonSkill")]
public class Hod_SummonSkill : UnitSkillBase<Hod_SummonSkill.Hod_SummonRumtimeSkill, IUnitSkill>
{
    public class Hod_SummonRumtimeSkill : IUnitSkill, IOnEnableSkill
    {
        Unit unit;
        public void ApplyOnEnableSkill(MonoBehaviour monoBehaviour)
        {
            FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddDamage(0.1f);
            unit.unitEvent.IsMergeRandomized = true;
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
