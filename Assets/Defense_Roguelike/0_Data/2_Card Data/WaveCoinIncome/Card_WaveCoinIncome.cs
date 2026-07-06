using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveCoinIncome", menuName = "Card Effect/Card_WaveCoinIncome")]
public class Card_WaveCoinIncome : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddRewardCoinsOnWaveEndAmount(Amount);
    }

    void RewardCoinsOnWaveEnd(int wave)
    {
        int getCoin = Mathf.FloorToInt(GameManager.Inst.coin * GameStatManager.Inst.RewardCoinsOnWaveEndAmount);
        GameManager.Inst.ModifyCoin(getCoin);
    }

    public void Register()
    {
        GameManager.Inst.OnWaveChanged += RewardCoinsOnWaveEnd;
    }
}
