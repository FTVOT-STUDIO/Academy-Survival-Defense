using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveShieldRecovery", menuName = "Card Effect/Card_WaveShieldRecovery")]
public class Card_WaveShieldRecovery : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddUnitShieldOnWaveEndAmount(Amount);
    }

    void RecoveryShieldOnWaveEnd(int wave)
    {
        List<Unit> curUnits = FieldManager.Inst.GetFieldBatchUnits();
        if (curUnits == null || curUnits.Count == 0) return;

        for (int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i] == null)
                continue;

            curUnits[i].unitEvent.ApplyGetShield(curUnits[i].maxShield * GameStatManager.Inst.UnitShieldOnWaveEndAmount, GameStatManager.Inst.GameShieldMultiplier, GameStatManager.Inst.GameShieldReceivedMultiplier);
        }
    }

    public void Register()
    {
        GameManager.Inst.OnWaveChanged += RecoveryShieldOnWaveEnd;
    }
}
