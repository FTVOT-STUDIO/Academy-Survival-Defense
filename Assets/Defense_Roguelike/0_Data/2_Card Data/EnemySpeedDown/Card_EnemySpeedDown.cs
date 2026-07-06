using UnityEngine;

[CreateAssetMenu(fileName = "Card_EnemySpeedDown", menuName = "Card Effect/Card_EnemySpeedDown")]
public class Card_EnemySpeedDown : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameSlowValue(Amount);
    }
}
