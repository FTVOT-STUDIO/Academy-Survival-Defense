using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_GetRandomUnitLevelUp", menuName = "Card Effect/Card_GetRandomUnitLevelUp")]
public class Card_GetRandomUnitLevelUp : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Unit> fieldBatchUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (fieldBatchUnits == null || fieldBatchUnits.Count == 0)
            return;

        int batchCount = fieldBatchUnits.Count;

        if (Random.value < 0.5f)
        {
            for (int i = 0; i < batchCount; i++)
            {
                fieldBatchUnits[i].unitEvent.ApplyLevelDownUnit(2);
            }
        }
        else
        {
            for (int i = 0; i < batchCount; i++)
            {
                fieldBatchUnits[i].unitEvent.ApplyLevelUpUnit(2);
            }
        }
    }
}
