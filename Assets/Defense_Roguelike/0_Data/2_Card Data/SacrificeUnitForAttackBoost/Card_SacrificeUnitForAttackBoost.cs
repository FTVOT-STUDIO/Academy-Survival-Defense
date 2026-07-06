using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_SacrificeUnitForAttackBoost", menuName = "Card Effect/Card_SacrificeUnitForAttackBoost")]
public class Card_SacrificeUnitForAttackBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        List<Unit> curFieldUnit = FieldManager.Inst.GetFieldBatchUnits();
        if (curFieldUnit == null || curFieldUnit.Count == 0)
            return;

        if (curFieldUnit.Count >= 3)
        {
            List<Unit> selectedUnits = new List<Unit>();
            while (selectedUnits.Count < 3)
            {
                Unit candidate = curFieldUnit[Random.Range(0, curFieldUnit.Count)];
                if (!selectedUnits.Contains(candidate))
                    selectedUnits.Add(candidate);
            }

            foreach (Unit unit in selectedUnits)
            {
                unit.GetComponent<UnitEvent>().InstantSaleUnit(unit);
            }

            GameStatManager.Inst.AddGameDamage(20);
        }
    }
}