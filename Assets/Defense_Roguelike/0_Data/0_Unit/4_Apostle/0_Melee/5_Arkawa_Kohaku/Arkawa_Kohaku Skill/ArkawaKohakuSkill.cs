using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ArkawaKohakuSkill", menuName = "Unit Skill Data/ArkawaKohakuSkill")]
public class ArkawaKohakuSkill : UnitSkillBase<ArkawaKohakuSkill.ArkawaKohakuRuntimeSkill, IUnitSkill>
{
    public class ArkawaKohakuRuntimeSkill : IUnitSkill, IAttackPassive, ICoolTimeSkill
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, 0, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, unit.unitScanner.nearestTarget.GetComponent<Enemy>(), UnitUtility.DamageType.ByMaxHP, unit.skillAmount);

            ; GameObject swordingEffect = EffectManager.Inst.Get(unit.unitData.effectPrefab, unit.transform.position);
            swordingEffect.transform.localScale = new Vector3(unit.sp.flipX ? -1 : 1, 1, 1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Sword2);
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.unitScanner.GetCurrentTarget();
            if (enemy == null)
                return;

            enemy.StartCoroutine(OnSkill());

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.KohakuAblity);
            EffectManager.Inst.Get(EffectManager.PrefabType.KohakuAblityEffect, unit.unitScanner.nearestTarget.position);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill()
        {
            Enemy enemy = unit.unitScanner.GetCurrentTarget();
            if (enemy == null)
                yield break;

            int ticks = 5;
            float interval = 0.05f;
            unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout * 2, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier, enemy, UnitUtility.DamageType.Fixed);
            for (int i = 0; i < ticks; i++)
            {
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, 0, 0, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier, enemy, UnitUtility.DamageType.ByMaxHP, unit.skillAmount);
                yield return new WaitForSeconds(interval);
            }
        }

    }

}

