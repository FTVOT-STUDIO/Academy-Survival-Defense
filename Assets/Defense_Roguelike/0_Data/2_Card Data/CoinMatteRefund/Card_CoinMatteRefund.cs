using UnityEngine;

[CreateAssetMenu(fileName = "Card_CoinMatteRefund", menuName = "Card Effect/Card_CoinMatteRefund")]
public class Card_CoinMatteRefund : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        int result = GameFlowManager.Inst.spentCoin / 2;
        GameManager.Inst.ModifyCoin(result);
    }
}
