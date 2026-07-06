using UnityEngine;

[CreateAssetMenu(fileName = "Card_GetRandomCoin", menuName = "Card Effect/Card_GetRandomCoin")]
public class Card_GetRandomCoin : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        int ranCoinValue = (int)Random.Range(0, Amount);
        GameManager.Inst.ModifyCoin(ranCoinValue);
    }
}
