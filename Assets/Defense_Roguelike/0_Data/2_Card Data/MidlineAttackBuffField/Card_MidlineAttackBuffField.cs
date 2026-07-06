using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_MidlineAttackBuffField", menuName = "Card Effect/Card_MidlineAttackBuffField")]
public class Card_MidlineAttackBuffField : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Transform> fields = FieldManager.Inst.fieldSlots.ToList();

        foreach (var field in fields)
        {
            FieldSlot fieldSlot = field.GetComponent<FieldSlot>();
            if (fieldSlot.formationRow != FieldSlot.FormationRow.Middle)
                continue;

            FieldPlacementTrigger trigger = fieldSlot.GetComponent<FieldPlacementTrigger>();

            trigger.RegisterUnitPlacedEvent(unit => unit.unitEvent.ApplyDamageStats(1f, true));

            trigger.RegisterUnitRemovedEvent(unit => unit.unitEvent.ApplyDamageStats(1f, false));
        }
    }
}
