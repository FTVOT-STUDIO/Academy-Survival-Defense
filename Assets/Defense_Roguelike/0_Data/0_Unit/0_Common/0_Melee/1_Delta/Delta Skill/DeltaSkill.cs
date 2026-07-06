using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DeltaSkill", menuName = "Unit Skill Data/DeltaSkill")]
public class DeltaSkill : UnitSkillBase<DeltaSkill.DeltaRuntimeSkill, IUnitSkill>
{
    public class DeltaRuntimeSkill : IUnitSkill, IAttackPassive, ICoolTimeSkill
    {
        Unit unit;
        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.nearestTarget.GetComponent<Enemy>();

            if (unit.hitCount % 3 == 0 && unit.hitCount != 0)
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout * 2, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy);

                EffectManager.Inst.Get(EffectManager.PrefabType.DeltaEffect, unit.unitScanner.nearestTarget.transform.position);
            }
            else
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy);
            }

        }
        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.isSkillActive)
                return;
            unit.unitWeapon.SetSkillCoroutine(SetSkill(unit));
        }


        IEnumerator SetSkill(Unit unit)
        {
            unit.unitWeapon.isSkillActive = true;
            unit.unitEvent.ApplyAttackSpeedStats(unit.skillAmount, true);

            yield return new WaitForSeconds(5f);
            unit.unitWeapon.isSkillActive = false;
            unit.unitEvent.ApplyAttackSpeedStats(unit.skillAmount, false);
        }
    }
}
