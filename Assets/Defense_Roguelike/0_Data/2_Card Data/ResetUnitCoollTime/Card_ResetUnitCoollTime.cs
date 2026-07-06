using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_ResetUnitCoollTime", menuName = "Card Effect/Card_ResetUnitCoollTime")]
public class Card_ResetUnitCoollTime : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Unit> units = FieldManager.Inst.GetFieldBatchUnits();
        if (units == null || units.Count == 0)
            return;

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null)
                continue;

            units[i].unitEvent.ApplyGetCoolTime(units[i].coolTime);
        }
    }
}
