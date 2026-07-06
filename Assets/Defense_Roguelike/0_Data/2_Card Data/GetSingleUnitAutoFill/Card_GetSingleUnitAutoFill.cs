using UnityEngine;

[CreateAssetMenu(fileName = "Card_GetSingleUnitAutoFill", menuName = "Card Effect/Card_GetSingleUnitAutoFill")]
public class Card_GetSingleUnitAutoFill : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        UnitData data = DeckManager.Inst.currentInGameDeck.unitdatas[Random.Range(0, DeckManager.Inst.currentInGameDeck.unitdatas.Count)];
        GameManager.Inst.unitSpawn.AutoFillFixedUnit(data);
    }
}
