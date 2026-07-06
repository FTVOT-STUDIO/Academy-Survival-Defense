using UnityEngine;

[CreateAssetMenu(fileName = "Card_AbilityRecharge", menuName = "Card Effect/Card_AbilityRecharge")]
public class Card_AbilityRecharge : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        FieldManager.Inst.fieldSephira.sephiraUnitEvent.MultiplyAbilityStats(Amount);
    }
}
