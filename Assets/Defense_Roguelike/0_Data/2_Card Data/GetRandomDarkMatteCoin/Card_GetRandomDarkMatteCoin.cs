using UnityEngine;

[CreateAssetMenu(fileName = "Card_GetRandomDarkMatteCoin", menuName = "Card Effect/Card_GetRandomDarkMatteCoin")]
public class Card_GetRandomDarkMatteCoin : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        int ranDarkCoinValue = (int)Random.Range(0, Amount);
        GameManager.Inst.ModifyDarkMatteCoin(ranDarkCoinValue);
    }
}
