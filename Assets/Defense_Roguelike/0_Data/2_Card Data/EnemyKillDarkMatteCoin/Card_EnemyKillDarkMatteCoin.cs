using UnityEngine;

[CreateAssetMenu(fileName = "Card_EnemyKillDarkMatteCoin", menuName = "Card Effect/Card_EnemyKillDarkMatteCoin")]
public class Card_EnemyKillDarkMatteCoin : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameGetDarkMatteCoin(Amount);
    }
}
