using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_FrontlineHealthBuffField", menuName = "Card Effect/Card_FrontlineHealthBuffField")]
public class Card_FrontlineHealthBuffField : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Transform> fields = FieldManager.Inst.fieldSlots.ToList();

        foreach (var field in fields)
        {
            FieldSlot fieldSlot = field.GetComponent<FieldSlot>();
            if (fieldSlot.formationRow != FieldSlot.FormationRow.Front)
                continue;

            FieldPlacementTrigger trigger = fieldSlot.GetComponent<FieldPlacementTrigger>();

            trigger.RegisterUnitPlacedEvent(unit => unit.unitEvent.ApplyHealthStats(1f, true));

            trigger.RegisterUnitRemovedEvent(unit => unit.unitEvent.ApplyHealthStats(1f, false));
        }
    }
}
