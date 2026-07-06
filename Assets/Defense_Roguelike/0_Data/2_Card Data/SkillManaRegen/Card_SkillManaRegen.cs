using UnityEngine;

[CreateAssetMenu(fileName = "Card_SkillManaRegen", menuName = "Card Effect/Card_SkillManaRegen")]
public class Card_SkillManaRegen : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddManaRegenRate(Amount);
    }

    private void ManaRegenRate(Unit unit)
    {
        unit.unitEvent.ApplyGetCoolTime(unit.coolTime * GameStatManager.Inst.ManaRegenRate);
    }

    public void Register()
    {
        InGameEventManager.Inst.UnitSkillEvent += ManaRegenRate;
    }
}
