using UnityEngine;

[CreateAssetMenu(fileName = "Card_ExpGainBoost", menuName = "Card Effect/Card_ExpGainBoost")]
public class Card_ExpGainBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameExpValue(Amount);
    }
}
