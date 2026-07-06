using UnityEngine;

[CreateAssetMenu(fileName = "Card_GetUnitAutoFill", menuName = "Card Effect/Card_GetUnitAutoFill")]
public class Card_GetUnitAutoFill : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameManager.Inst.unitSpawn.AutoFillRandomUnit(DeckManager.Inst.currentInGameDeck.unitdatas);
    }
}
