using UnityEngine;

[CreateAssetMenu(fileName = "MinimelSkill", menuName = "Unit Skill Data/MinimelSkill")]
public class MinimelSkill : UnitSkillBase<MinimelSkill.MinimelRuntimeSkill, IUnitSkill>
{
    public class MinimelRuntimeSkill : MultSupportSkillBase, IUnitSkill
    {
        Unit unit;
        public override void ApplySupport(Unit unit)
        {
            curValue = this.unit.skillAmount;
            unit.unitEvent.ApplyAttackSpeedStats(curValue, true);
        }

        public override void ClearSupport(Unit unit)
        {
            unit.unitEvent.ApplyAttackSpeedStats(curValue, false);

        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
