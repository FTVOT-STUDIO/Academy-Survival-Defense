using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveUnitStarLevelBoost", menuName = "Card Effect/Card_WaveUnitStarLevelBoost")]
public class Card_WaveUnitStarLevelBoost : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
    }

    void UnitStarLevelBoostOnWaveEnd(int wave)
    {
        List<Unit> curUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (curUnits == null || curUnits.Count == 0) return;

        int ranUnit = Random.Range(0, curUnits.Count);
        curUnits[ranUnit].unitEvent.ApplyLevelUpUnit(1);
    }
    public void Register()
    {
        GameManager.Inst.OnWaveChanged += UnitStarLevelBoostOnWaveEnd;
    }
}
