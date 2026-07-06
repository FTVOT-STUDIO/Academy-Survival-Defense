using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveCoolTimeRecovery", menuName = "Card Effect/Card_WaveCoolTimeRecovery")]
public class Card_WaveCoolTimeRecovery : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddUnitCoolTimeRecoveryOnWaveEndAmount(Amount);
    }

    void RecoveryCoolTimeOnWaveEnd(int wave)
    {
        List<Unit> curUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (curUnits == null || curUnits.Count == 0) return;

        for (int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i] == null)
                continue;

            curUnits[i].unitEvent.ApplyGetCoolTime(curUnits[i].coolTime * GameStatManager.Inst.UnitCoolTimeRecoveryOnWaveEndAmount);
        }
    }

    public void Register()
    {
        GameManager.Inst.OnWaveChanged += RecoveryCoolTimeOnWaveEnd;
    }
}
