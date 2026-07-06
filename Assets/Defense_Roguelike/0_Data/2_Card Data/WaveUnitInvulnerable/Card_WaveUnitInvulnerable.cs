using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveUnitInvulnerable", menuName = "Card Effect/Card_WaveUnitInvulnerable")]
public class Card_WaveUnitInvulnerable : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameManager.Inst.OnWaveChanged += StartTemporaryInvulnerable;
    }

    void StartTemporaryInvulnerable(int wave)
    {
        List<Unit> units = FieldManager.Inst.GetFieldBatchUnits();
        if (units == null || units.Count == 0) return;

        Unit ranUnit = units[Random.Range(0, units.Count)];

        ranUnit.unitEvent.ApplyInvulnerable(20);
    }
}
