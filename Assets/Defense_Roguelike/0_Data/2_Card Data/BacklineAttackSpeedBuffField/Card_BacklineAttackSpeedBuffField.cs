using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_BacklineAttackSpeedBuffField", menuName = "Card Effect/Card_BacklineAttackSpeedBuffField")]
public class Card_BacklineAttackSpeedBuffField : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Transform> fields = FieldManager.Inst.fieldSlots.ToList();

        foreach (var field in fields)
        {
            FieldSlot fieldSlot = field.GetComponent<FieldSlot>();
            if (fieldSlot.formationRow != FieldSlot.FormationRow.Back)
                continue;

            FieldPlacementTrigger trigger = fieldSlot.GetComponent<FieldPlacementTrigger>();

            trigger.RegisterUnitPlacedEvent(unit => unit.unitEvent.ApplyAttackSpeedStats(1, true));

            trigger.RegisterUnitRemovedEvent(unit => unit.unitEvent.ApplyAttackSpeedStats(1, false));
        }
    }
}
