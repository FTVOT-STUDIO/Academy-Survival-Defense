using UnityEngine;

[CreateAssetMenu(fileName = "EpsilonSkill", menuName = "Unit Skill Data/EpsilonSkill")]
public class EpsilonSkill : UnitSkillBase<EpsilonSkill.EpsilonRumtimeSkill, IUnitSkill>
{
    public class EpsilonRumtimeSkill : IUnitSkill, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Vector3 effectPos = new Vector3(unit.transform.position.x, unit.transform.position.y + 0.5f, unit.transform.position.z);
            EffectManager.Inst.Get(EffectManager.PrefabType.SheldEffect, effectPos);
            unit.unitEvent.ApplyGetShield(unit.maxShield * unit.skillAmount, unit.shieldMultiplier, unit.shieldReceivedMultiplier);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }

}
