using UnityEngine;

[CreateAssetMenu(fileName = "Card_SpawnCCard_UnitSaleRewardMultiplierostReduction", menuName = "Card Effect/Card_UnitSaleRewardMultiplier")]
public class Card_UnitSaleRewardMultiplier : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.MultiplyUnitSaleRewardMultiplier((int)Amount);
    }

}
