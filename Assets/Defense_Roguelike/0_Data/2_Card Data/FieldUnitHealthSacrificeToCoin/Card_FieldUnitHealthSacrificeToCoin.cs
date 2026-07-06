using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_FieldUnitHealthSacrificeToCoin", menuName = "Card Effect/Card_FieldUnitHealthSacrificeToCoin")]
public class Card_FieldUnitHealthSacrificeToCoin : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        int getCoin = 0;

        List<Unit> curFieldUnit = FieldManager.Inst.GetFieldBatchUnits();
        if (curFieldUnit == null || curFieldUnit.Count == 0)
            return;

        for (int i = 0; i < curFieldUnit.Count; i++)
        {
            if (curFieldUnit[i] == null)
                continue;

            float targetHealth = curFieldUnit[i].maxHealth * 0.01f;
            getCoin += Mathf.FloorToInt(curFieldUnit[i].health - targetHealth);
            curFieldUnit[i].health = targetHealth;
        }

        GameManager.Inst.ModifyCoin(getCoin);
    }
}
