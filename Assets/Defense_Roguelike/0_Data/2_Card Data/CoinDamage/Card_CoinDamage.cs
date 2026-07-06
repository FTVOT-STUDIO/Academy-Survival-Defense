using UnityEngine;

[CreateAssetMenu(fileName = "Card_CoinDamage", menuName = "Card Effect/Card_CoinDamage")]
public class Card_CoinDamage : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddCoinDamage(Amount);
    }

    public void Register()
    {
        InGameEventManager.Inst.GetCoin += GameStatManager.Inst.MarkStatsDirty;
    }
}
