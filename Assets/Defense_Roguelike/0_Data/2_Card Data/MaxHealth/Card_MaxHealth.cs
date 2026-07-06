using UnityEngine;

[CreateAssetMenu(fileName = "Card_MaxHealth", menuName = "Card Effect/Card_MaxHealth")]
public class Card_MaxHealth : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameMaxHealth(Amount);
    }
}
