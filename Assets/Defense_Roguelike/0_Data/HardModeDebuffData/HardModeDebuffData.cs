using UnityEngine;

[CreateAssetMenu(fileName = "HardModeDebuffData", menuName = "HardMode Debuff Data/HardModeDebuffData")]
public class HardModeDebuffData : ScriptableObject
{
    public enum DebuffType
    {
        AbilityChargePenalty,
        SummonCostIncrease,
        LevelUpCardTheft,
        FieldSlotRestriction,
        EndWaveCoinLoss,
        EndWaveRandomDebuff,
        UnitSilence,
        MergeRandomization
    }

    public DebuffType type;
    [Space]
    public Sprite icon;
    [Space]
    public string debuffName;
    [Space]
    [TextArea]
    public string debuffDesc;

    public void ApplyDebuff()
    {
        switch (type)
        {
            case DebuffType.AbilityChargePenalty:
                GameStatManager.Inst.MultiplyGameMaxAbilityCount(2);
                break;
            case DebuffType.EndWaveCoinLoss:
                GameManager.Inst.OnWaveChanged += (_) => GameManager.Inst.ModifyCoin(-Mathf.FloorToInt(GameManager.Inst.coin * 0.1f));
                break;
            case DebuffType.EndWaveRandomDebuff:
                GameManager.Inst.OnWaveChanged += (_) => DebuffManager.Inst.ApplyRandomDebuff(2);
                break;
            case DebuffType.FieldSlotRestriction:
                for (int i = 0; i < 3; i++)
                    GameManager.Inst.unitSpawn.GenerateUnit(DebuffManager.Inst.blockUnitData);
                break;
            case DebuffType.LevelUpCardTheft:
                DebuffManager.Inst.blockLevelUpUI = true;
                break;
            case DebuffType.SummonCostIncrease:
                GameManager.Inst.unitSpawnCost *= 2;
                break;
            case DebuffType.UnitSilence:
                DebuffManager.Inst.isUnitSilenceDebuff = true;
                break;
            case DebuffType.MergeRandomization:
                DebuffManager.Inst.isMergeRandomizedDebuff = true;
                break;

        }
    }

}
