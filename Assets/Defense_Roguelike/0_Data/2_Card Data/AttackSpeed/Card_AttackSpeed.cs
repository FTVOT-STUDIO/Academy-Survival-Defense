using UnityEngine;

[CreateAssetMenu(fileName = "Card_AttackSpeed", menuName = "Card Effect/Card_AttackSpeed")]
public class Card_AttackSpeed : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameAttackSpeed(Amount);
    }
}
