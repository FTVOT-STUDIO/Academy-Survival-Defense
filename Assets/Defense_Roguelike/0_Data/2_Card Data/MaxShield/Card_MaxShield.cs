using UnityEngine;

[CreateAssetMenu(fileName = "Card_MaxShield", menuName = "Card Effect/Card_MaxShield")]
public class Card_MaxShield : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameMaxHealth(Amount);
    }
}
