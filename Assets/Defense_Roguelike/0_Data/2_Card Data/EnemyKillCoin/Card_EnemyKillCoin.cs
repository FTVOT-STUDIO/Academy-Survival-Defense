using UnityEngine;

[CreateAssetMenu(fileName = "Card_EnemyKillCoin", menuName = "Card Effect/Card_EnemyKillCoin")]
public class Card_EnemyKillCoin : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameGetCoin(Amount);
    }
}
