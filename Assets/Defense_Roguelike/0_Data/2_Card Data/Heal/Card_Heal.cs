using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_Heal", menuName = "Card Effect/Card_Heal")]
public class Card_Heal : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Unit> fieldUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (fieldUnits == null || fieldUnits.Count == 0)
            return;

        for (int i = 0; i < fieldUnits.Count; i++)
        {
            fieldUnits[i].health = fieldUnits[i].maxHealth;
        }
    }
}
