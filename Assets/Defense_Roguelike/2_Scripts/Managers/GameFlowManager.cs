using TMPro;
using UnityEngine;


public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Inst;

    public TMP_Text spentAnomalyText;
    public TMP_Text spentCoinText;
    public TMP_Text spentDarkMatteCoinText;
    public TMP_Text spentUnitSpawnCountText;
    public TMP_Text spentAbilityText;
    public TMP_Text spentDeadUnitText;
    public TMP_Text enemyKillCountText;
    public TMP_Text enemyCountText;
    public TMP_Text coinGainText;
    public TMP_Text darkMatteGainText;
    public TMP_Text cardSpawnLevelText;

    [HideInInspector] public int spentAnomaly;
    [HideInInspector] public int spentCoin;
    [HideInInspector] public int spentDarkMatteCoin;
    [HideInInspector] public int spentUnitSpawnCount;
    [HideInInspector] public int spentAbility;
    [HideInInspector] public int deadUnits;
    [HideInInspector] public int enemyKillCount;
    [HideInInspector] public int spentCardSpawnCost;
    [HideInInspector] public int darkMatteGainMultiplier = 1;
    [HideInInspector] public int coinGainMultiplier = 1;
    [HideInInspector] public int cardSpawnLevel = 0;

    void Awake()
    {
        Inst = this;
    }

    public void InitializedGameFlow()
    {
        spentAnomaly = 0;
        spentCoin = 0;
        spentDarkMatteCoin = 0;
        spentUnitSpawnCount = 0;
        spentAbility = 0;
        deadUnits = 0;
        enemyKillCount = 0;
        spentCardSpawnCost = 0;
        darkMatteGainMultiplier = 0;
        coinGainMultiplier = 0;
        cardSpawnLevel = 0;

        ChangeEnemyCount(0);
        ChangeKillCount(0);
        ChangeSpentAnomaly(0);
        ChangeDeadUnits(0);
        ChangeSpentCoin(0);
        ChangeSpentDarkMatteCoin(0);
        ChangeSpentUnitSpawnCount(0);
        ChangeAbilityCount(0);
        ChangeCoinGain(1);
        ChangeDarkMatteGain(1);
        ChangeCardSpawnLevel(0);

        InGameUIManager.Inst.skillCardCostText.text = "x" + spentCardSpawnCost.ToString();
    }

    public void ChangeEnemyCount(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref EnemySpawnManager.Inst.enemyCount, delta, enemyCountText, "x ");
    }

    public void ChangeKillCount(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref enemyKillCount, delta, enemyKillCountText, "적 처치 횟수 : ");
    }

    public void ChangeSpentAnomaly(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref spentAnomaly, delta, spentAnomalyText, "이상현상 발동 : ");
    }

    public void ChangeDeadUnits(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref deadUnits, delta, spentDeadUnitText, "사망한 유닛 : ");
    }

    public void ChangeSpentCoin(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref spentCoin, delta, spentCoinText, "소모한 코인 : ");
    }

    public void ChangeSpentDarkMatteCoin(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref spentDarkMatteCoin, delta, spentDarkMatteCoinText, "소모한 암흑물질 : ");
    }

    public void ChangeSpentUnitSpawnCount(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref spentUnitSpawnCount, delta, spentUnitSpawnCountText, "유닛 소환 횟수 : ");
    }

    public void ChangeAbilityCount(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref spentAbility, delta, spentAbilityText, "궁극기 사용 횟수 : ");
    }

    public void ChangeCoinGain(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref coinGainMultiplier, delta, coinGainText, "코인 획득량 증가 : x");
    }
    public void ChangeDarkMatteGain(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref darkMatteGainMultiplier, delta, darkMatteGainText, "암흑물질 획득량 증가 : x");
    }
    public void ChangeCardSpawnLevel(int delta)
    {
        InGameUIManager.Inst.ChangeCount(ref cardSpawnLevel, delta, cardSpawnLevelText, "스킬카드 소환 레벨 : ");
    }
}
