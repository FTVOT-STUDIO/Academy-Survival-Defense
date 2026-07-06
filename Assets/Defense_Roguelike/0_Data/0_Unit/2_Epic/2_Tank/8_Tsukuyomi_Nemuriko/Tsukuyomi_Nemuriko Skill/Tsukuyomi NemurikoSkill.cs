using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TsukuyomiNemurikoSkill", menuName = "Unit Skill Data/TsukuyomiNemurikoSkill")]
public class TsukuyomiNemurikoSkill : UnitSkillBase<TsukuyomiNemurikoSkill.TsukuyomiNemurikoRumtimeSkill, IUnitSkill>
{
    public class TsukuyomiNemurikoRumtimeSkill : IUnitSkill, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.isSkillActive)
                return;
            monoBehaviour.StartCoroutine(SetSkill(unit));
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator SetSkill(Unit unit)
        {
            unit.unitEvent.ApplyDefenseStats(unit.skillAmount, true);

            yield return new WaitForSeconds(10f);

            unit.unitEvent.ApplyDefenseStats(unit.skillAmount, false);
        }
    }
}
