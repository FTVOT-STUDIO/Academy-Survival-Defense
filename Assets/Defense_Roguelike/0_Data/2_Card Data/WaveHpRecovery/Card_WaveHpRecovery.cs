using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveHpRecovery", menuName = "Card Effect/Card_WaveHpRecovery")]
public class Card_WaveHpRecovery : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddUnitHealOnWaveEndAmount(Amount);
    }


    void RecoveryHpOnWaveEnd(int wave)
    {
        List<Unit> curUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (curUnits == null || curUnits.Count == 0) return;

        for (int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i] == null)
                continue;

            curUnits[i].unitEvent.ApplyGetHeal(curUnits[i].maxHealth * GameStatManager.Inst.UnitHealOnWaveEndAmount, GameStatManager.Inst.GameHealingMultiplier, GameStatManager.Inst.GameHealingReceivedMultiplier);
        }
    }

    public void Register()
    {
        GameManager.Inst.OnWaveChanged += RecoveryHpOnWaveEnd;
    }
}
