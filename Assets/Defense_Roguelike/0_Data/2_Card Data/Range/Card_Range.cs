using UnityEngine;

[CreateAssetMenu(fileName = "Card_Range", menuName = "Card Effect/Card_Range")]
public class Card_Range : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameRange(Amount);
    }
}
