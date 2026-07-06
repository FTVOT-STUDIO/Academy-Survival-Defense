using UnityEngine;

[CreateAssetMenu(fileName = "Card_DarkMatteGainBoost", menuName = "Card Effect/Card_DarkMatteGainBoost")]
public class Card_DarkMatteGainBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameFlowManager.Inst.ChangeDarkMatteGain((int)Amount);
    }
}
