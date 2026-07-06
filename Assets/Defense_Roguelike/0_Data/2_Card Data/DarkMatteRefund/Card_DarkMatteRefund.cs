using UnityEngine;

[CreateAssetMenu(fileName = "Card_DarkMatteRefund", menuName = "Card Effect/Card_DarkMatteRefund")]
public class Card_DarkMatteRefund : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        int result = GameFlowManager.Inst.spentDarkMatteCoin / 2;
        GameManager.Inst.ModifyDarkMatteCoin(result);
    }
}
