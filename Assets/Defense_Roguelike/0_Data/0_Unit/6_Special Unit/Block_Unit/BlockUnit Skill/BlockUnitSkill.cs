using UnityEngine;

[CreateAssetMenu(fileName = "BlockUnitSkill", menuName = "Special Object/BlockUnitSkill")]
public class BlockUnitSkill : UnitSkillBase<BlockUnitSkill.BlockUnitRuntimeSkill, IUnitSkill>
{
    public class BlockUnitRuntimeSkill : IUnitSkill, IOnEnablePassive
    {
        Unit unit;
        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            //unit.sp.sortingOrder = 5;
            unit.unitEvent.IsUnitLocked = true;
            unit.unitEvent.IsInvulnerable = true;
            unit.unitEvent.IsIgnored = true;
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}


