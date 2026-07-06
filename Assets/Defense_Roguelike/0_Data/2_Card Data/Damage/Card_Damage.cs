using UnityEngine;

[CreateAssetMenu(fileName = "Card_Damage", menuName = "Card Effect/Card_Damage")]
public class Card_Damage : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameDamage(Amount);
    }
}
