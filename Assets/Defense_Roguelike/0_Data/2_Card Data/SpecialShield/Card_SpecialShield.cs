using UnityEngine;

[CreateAssetMenu(fileName = "Card_SpecialShield", menuName = "Card Effect/Card_SpecialShield")]
public class Card_SpecialShield : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        var units = FieldManager.Inst.GetFieldBatchUnits();
        if (units == null || units.Count == 0)
            return;

        foreach (var unit in units)
        {
            if (unit == null)
                continue;

            unit.unitEvent.IncrementSpecialShield();
        }
    }
}
