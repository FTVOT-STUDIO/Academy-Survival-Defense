using UnityEngine;

[CreateAssetMenu(fileName = "Card_AlliesAtkDownCoolTimeBoost", menuName = "Card Effect/Card_AlliesAtkDownCoolTimeBoost")]
public class Card_AlliesAtkDownCoolTimeBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.DivideGameDamage(2);
        GameStatManager.Inst.MultiplyGameCoolTime(2);
    }
}
