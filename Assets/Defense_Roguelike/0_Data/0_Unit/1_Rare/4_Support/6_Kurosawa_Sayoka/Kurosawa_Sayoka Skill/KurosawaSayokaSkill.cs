using UnityEngine;

[CreateAssetMenu(fileName = "KurosawaSayokaSkill", menuName = "Unit Skill Data/KurosawaSayokaSkill")]
public class KurosawaSayokaSkill : UnitSkillBase<KurosawaSayokaSkill.KurosawaSayokaRuntimeSkill, IUnitSkill>
{
    public class KurosawaSayokaRuntimeSkill : MultSupportSkillBase, IUnitSkill
    {
        Unit unit;
        public override void ApplySupport(Unit unit)
        {
            curValue = this.unit.skillAmount;
            unit.unitEvent.ApplyCriticalPerStats(curValue, true);

        }

        public override void ClearSupport(Unit unit)
        {
            unit.unitEvent.ApplyCriticalPerStats(curValue, false);

        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
