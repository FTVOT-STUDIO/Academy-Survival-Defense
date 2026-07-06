using UnityEngine;

[CreateAssetMenu(fileName = "Card_CoinGainBoost", menuName = "Card Effect/Card_CoinGainBoost")]
public class Card_CoinGainBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameFlowManager.Inst.ChangeCoinGain((int)Amount);
    }
}
