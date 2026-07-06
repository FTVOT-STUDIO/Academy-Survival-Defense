using UnityEngine;

[CreateAssetMenu(fileName = "Card_Weakness", menuName = "Card Effect/Card_Weakness")]
public class Card_Weakness : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameWeakness(Amount);
    }
}
