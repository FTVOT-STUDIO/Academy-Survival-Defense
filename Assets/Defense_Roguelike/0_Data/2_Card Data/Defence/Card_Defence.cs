using UnityEngine;

[CreateAssetMenu(fileName = "Card_Defence", menuName = "Card Effect/Card_Defence")]
public class Card_Defence : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameDefense(Amount);
    }
}
