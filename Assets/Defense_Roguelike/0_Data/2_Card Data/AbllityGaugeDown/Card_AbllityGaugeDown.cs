using UnityEngine;

[CreateAssetMenu(fileName = "Card_AbllityGaugeDown", menuName = "Card Effect/Card_AbllityGaugeDownard_")]
public class Card_AbllityGaugeDown : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        FieldManager.Inst.fieldSephira.sephiraUnitEvent.MultiplyAbilityStatsDecrease(Amount);
    }
}
