using UnityEngine;

[CreateAssetMenu(fileName = "Card_CoolTime", menuName = "Card Effect/Card_CoolTime")]
public class Card_CoolTime : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameCoolTime(Amount);
    }
}
