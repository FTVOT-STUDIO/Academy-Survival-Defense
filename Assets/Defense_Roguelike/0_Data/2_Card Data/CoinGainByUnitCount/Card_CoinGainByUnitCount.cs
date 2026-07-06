using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_CoinGainByUnitCount", menuName = "Card Effect/Card_CoinGainByUnitCount")]
public class Card_CoinGainByUnitCount : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        var curUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (curUnits == null || curUnits.Count == 0)
            return;


        int getCoin = FieldManager.Inst.GetFieldBatchUnits().Count * 100;
        GameManager.Inst.ModifyCoin(getCoin);
    }
}
