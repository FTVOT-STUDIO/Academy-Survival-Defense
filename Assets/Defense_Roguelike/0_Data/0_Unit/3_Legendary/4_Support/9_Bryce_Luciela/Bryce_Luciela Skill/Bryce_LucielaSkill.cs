using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Bryce_LucielaSkill", menuName = "Unit Skill Data/Bryce_LucielaSkill")]
public class Bryce_LucielaSkill : UnitSkillBase<Bryce_LucielaSkill.Bryce_LucielaRuntimeSkill, IUnitSkill>
{
    public class Bryce_LucielaRuntimeSkill : SingleSupportSkillBase, IUnitSkill, ISupportDetectable
    {
        Unit unit;
        bool isApply = false;
        public override void ApplySupport(Unit unit, Unit targetUnit)
        {
            if (isApply)
                return;
            isApply = true;
            Unit curUnit = targetUnit;
            EffectManager.Inst.Get(unit.unitData.effectPrefab, unit.transform.position);
            unit.StartCoroutine(ClearBuff(unit, curUnit));

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Laugh1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Trans);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.BryceEffect);

        }

        public override void ClearSupport(Unit unit, Unit targetUnit)
        {
        }
        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
        IEnumerator ClearBuff(Unit unit, Unit curUnit)
        {
            yield return null;

            unit.unitEvent.ClearBuff(unit, curUnit);
            unit.unitData = curUnit.unitData;

            unit.Init(unit.starLevel, true, true);
            unit.SetUnitClassColor();
            unit.anim.SetTrigger("Attack");
        }

    }
}
