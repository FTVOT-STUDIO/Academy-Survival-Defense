using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameStatManager : MonoBehaviour
{
    public static GameStatManager Inst;
    public event Action OnStatsChanged;
    private float gameDamage = 1;
    private float gameSkillAmount = 1;
    private float gameDefense = 1;
    private float gameMaxHealth = 1;
    private float gameMaxShield = 1;
    private float gameCriticalPer = 1;
    private float gameCriticalDamage = 1;
    private float gameAttackSpeed = 1;
    private float gameRange = 1;
    private float gameCoolTime = 1;
    private float gameMaxAbilityCount = 1;
    private float gameGetCoin = 1;
    private float gameGetDarkMatteCoin = 0;
    private float gameSkillTriggerChance = 0;
    private float coinDamage = 1;
    private float gameWeakness = 1;
    private float gameExpValue = 1;
    private float gameSlowValue = 1;
    private float gameBossCoolTime = 1;
    private float gameDefensePenetration = 0;
    private float gameAttributeResistancePenetration = 0;
    private float gameAttributeDamageMultiplier = 1;
    private float gameDamageTakenIncrease = 1;

    private float gameHealingMultiplier = 1;
    private float gameHealingReceivedMultiplier = 1;

    private float gameShieldMultiplier = 1;
    private float gameShieldReceivedMultiplier = 1;

    private float gameManaReceivedMultiplier = 1;

    private float gameAreaDamage = 1;
    public float gameSingleTargetDamagefloat = 1;
    private float rewardCoinsOnWaveEndAmount = 0;
    private float unitHealOnWaveEndAmount = 0;
    private float unitCoolTimeRecoveryOnWaveEndAmount = 0;
    private float unitShieldOnWaveEndAmount = 0;
    private int unitSaleRewardMultiplier = 1;
    private int cardSpawnProbabilityBonus;
    private int showSkillCardCount;
    private float manaRegenRate;
    private float enemyHpRatioDamageReduction;
    private float gameHealReceivedMultiplier;
    private float enemyDebuffDurationMultiplier;
    private float gameEvasionValue;
    private int extraAttackTriggerInterval;

    private float gameNormalAttackMultiplier;
    private float gameSkillMultiplier;
    private float gameAbilityMultiplier;

    private float gameEnemyAttackSpeedMultiplier;

    private float gameEnemyNormalAttackDamageTakenMultiplier;
    private float gameEnemySkillDamageTakenMultiplier;
    private float gameEnemyAbilityDamageTakenMultiplier;

    [SerializeField] AttributeDatabase attributeDatabase;
    public AttributeModifierData[] gameAttributeDamageBonus;

    public AttributeModifierData[] gameAttributeWeaknessValues;
    public AttributeModifierData[] gameAttributeResistanceValues;

    public float GameDamage => gameDamage;
    public float GameSkillAmount => gameSkillAmount;
    public float GameDefense => gameDefense;
    public float GameMaxHealth => gameMaxHealth;
    public float GameMaxShield => gameMaxShield;
    public float GameCriticalPer => gameCriticalPer;
    public float GameCriticalDamage => gameCriticalDamage;
    public float GameAttackSpeed => gameAttackSpeed;
    public float GameRange => gameRange;
    public float GameCoolTime => gameCoolTime;
    public float GameMaxAbilityCount => gameMaxAbilityCount;
    public float GameGetCoin => gameGetCoin;
    public float GameGetDarkMatteCoin => gameGetDarkMatteCoin;
    public float GameSkillTriggerChance => gameSkillTriggerChance;
    public float CoinDamage => coinDamage;
    public float GameWeakness => gameWeakness;
    public float GameExpValue => gameExpValue;
    public float GameSlowValue => gameSlowValue;
    public float GameBossCoolTime => gameBossCoolTime;
    public float GameDefensePenetration => gameDefensePenetration;
    public float GameAttributeResistancePenetration => gameAttributeResistancePenetration;
    public float GameAttributeDamageMultiplier => gameAttributeDamageMultiplier;

    public float GameHealingMultiplier => gameHealingMultiplier;
    public float GameHealingReceivedMultiplier => gameHealingReceivedMultiplier;

    public float GameShieldMultiplier => gameShieldMultiplier;
    public float GameShieldReceivedMultiplier => gameShieldReceivedMultiplier;

    public float GameAreaDamage => gameAreaDamage;
    public float GameSingleTargetDamagefloat => gameSingleTargetDamagefloat;
    public float RewardCoinsOnWaveEndAmount => rewardCoinsOnWaveEndAmount;
    public float UnitHealOnWaveEndAmount => unitHealOnWaveEndAmount;
    public float UnitCoolTimeRecoveryOnWaveEndAmount => unitCoolTimeRecoveryOnWaveEndAmount;
    public float UnitShieldOnWaveEndAmount => unitShieldOnWaveEndAmount;
    public int UnitSaleRewardMultiplier => unitSaleRewardMultiplier;
    public int CardSpawnProbabilityBonus => cardSpawnProbabilityBonus;
    public int ShowSkillCardCount => showSkillCardCount;
    public float ManaRegenRate => manaRegenRate;
    public float EnemyHpRatioDamageReduction => enemyHpRatioDamageReduction;
    public float EnemyDebuffDurationMultiplier => enemyDebuffDurationMultiplier;
    public float GameEvasionValue => gameEvasionValue;
    public int ExtraAttackTriggerInterval => extraAttackTriggerInterval;

    public float GameNormalAttackMultiplier => gameNormalAttackMultiplier;
    public float GameSkillMultiplier => gameSkillMultiplier;
    public float GameAbilityMultiplier => gameAbilityMultiplier;

    public float GameEnemyAttackSpeedMultiplier => gameEnemyAttackSpeedMultiplier;

    public float GameEnemyNormalAttackDamageTakenMultiplier => gameEnemyNormalAttackDamageTakenMultiplier;
    public float GameEnemySkillDamageTakenMultiplier => gameEnemySkillDamageTakenMultiplier;
    public float GameEnemyAbilityDamageTakenMultiplier => gameEnemyAbilityDamageTakenMultiplier;

    public float GameManaReceivedMultiplier => gameManaReceivedMultiplier;
    public float GameDamageTakenIncrease => gameDamageTakenIncrease;

    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text skillValueText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text shieldText;
    [SerializeField] private TMP_Text criticalPerText;
    [SerializeField] private TMP_Text criticalDamageText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private TMP_Text coolTimeText;
    [SerializeField] private TMP_Text getCoinText;
    [SerializeField] private TMP_Text coinDamageText;
    [SerializeField] private TMP_Text weaknessText;
    [SerializeField] private TMP_Text expValueText;
    [SerializeField] private TMP_Text slowValueText;
    [SerializeField] private TMP_Text bossCoolTimeText;
    [SerializeField] private TMP_Text rewardCoinsOnWaveEndAmountText;
    [SerializeField] private TMP_Text healReceivedMultiplierText;
    [SerializeField] private TMP_Text unitEvasionText;
    [SerializeField] private TMP_Text gameSkillTriggerChanceText;
    [SerializeField] private TMP_Text enemyHpRatioDamageReductionText;
    [SerializeField] private TMP_Text enemyDebuffDurationMultiplierText;


    void Awake()
    {
        Inst = this;
        OnStatsChanged += UpdateGameStats;
    }

    void Start()
    {
        gameAttributeDamageBonus = CreateAttributeArray();
        gameAttributeWeaknessValues = CreateAttributeArray();
        gameAttributeResistanceValues = CreateAttributeArray();
    }

    private AttributeModifierData[] CreateAttributeArray()
    {
        AttributeModifierData[] array =
            new AttributeModifierData[attributeDatabase.attributeDatas.Count];

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = new AttributeModifierData();

            array[i].data = attributeDatabase.attributeDatas[i];
            array[i].value = 1f;
        }
        return array;
    }

    public void InitializedGameState()
    {
        gameDamage = 1;
        gameSkillAmount = 1;
        gameDefense = 1;
        gameMaxHealth = 1;
        gameMaxShield = 1;
        gameCriticalPer = 1;
        gameCriticalDamage = 1;
        gameAttackSpeed = 1;
        gameRange = 1;
        gameCoolTime = 1;
        gameMaxAbilityCount = 1;
        gameSkillTriggerChance = 0;
        gameGetCoin = 1;
        gameGetDarkMatteCoin = 0;
        rewardCoinsOnWaveEndAmount = 0;
        unitHealOnWaveEndAmount = 0;
        unitCoolTimeRecoveryOnWaveEndAmount = 0;
        unitShieldOnWaveEndAmount = 0;
        coinDamage = 0;
        gameWeakness = 1;
        gameExpValue = 1;
        gameSlowValue = 1;
        gameBossCoolTime = 1;
        gameDefensePenetration = 0;
        gameAttributeResistancePenetration = 0;
        unitSaleRewardMultiplier = 1;
        cardSpawnProbabilityBonus = 0;
        showSkillCardCount = 3;
        manaRegenRate = 0;
        enemyHpRatioDamageReduction = 0;
        gameHealReceivedMultiplier = 1;
        enemyDebuffDurationMultiplier = 1;
        gameEvasionValue = 0;
        extraAttackTriggerInterval = 0;

        gameNormalAttackMultiplier = 1;
        gameSkillMultiplier = 1;
        gameAbilityMultiplier = 1;

        gameEnemyAttackSpeedMultiplier = 1;

        gameEnemyNormalAttackDamageTakenMultiplier = 1;
        gameEnemySkillDamageTakenMultiplier = 1;
        gameEnemyAbilityDamageTakenMultiplier = 1;

        gameManaReceivedMultiplier = 1;

        gameDamageTakenIncrease = 1;

        UpdateGameStats();
    }

    public void MarkStatsDirty()
    {
        OnStatsChanged?.Invoke();
    }

    private void UpdateGameStats()
    {
        damageText.text = "공격력 증가 : " + ((GameDamage - 1) * 100f).ToString("F1") + "%";
        skillValueText.text = "스킬계수 증가 : " + ((GameSkillAmount - 1) * 100f).ToString("F1") + "%";
        defenseText.text = "방어력 증가 : " + ((GameDefense - 1) * 100f).ToString("F1") + "%";
        healthText.text = "체력 증가 : " + ((GameMaxHealth - 1) * 100f).ToString("F1") + "%";
        shieldText.text = "보호막 증가 : " + ((GameDefense - 1) * 100f).ToString("F1") + "%";
        criticalPerText.text = "치명타 확률 증가: " + ((GameCriticalPer - 1) * 100f).ToString("F1") + "%";
        criticalDamageText.text = "치명타 피해 증가 : " + ((GameCriticalDamage - 1) * 100f).ToString("F1") + "%";
        attackSpeedText.text = "공격속도 증가 : " + ((GameAttackSpeed - 1) * 100f).ToString("F1") + "%";
        rangeText.text = "사정거리 증가 : " + ((GameRange - 1) * 100f).ToString("F1") + "%";
        coolTimeText.text = "쿨타임 감소 : " + ((GameCoolTime - 1) * 100f).ToString("F1") + "%";
        // coolTimeText.text = "쿨타임 감소 : " + ((GameCoolTime - 1) * 100f).ToString("F1") + "%";
        getCoinText.text = "코인 획득량 증가 : " + ((GameGetCoin - 1) * 100f).ToString("F1") + "%";
        float coinMultiplier = Mathf.Max(CoinDamage * (GameManager.Inst.coin / 10000f), 0f);
        float coinBonusPercent = coinMultiplier * 100f;
        coinDamageText.text = $"코인 데미지 증가 : +{coinBonusPercent:F0}%";
        weaknessText.text = "적 약점 증가 : " + ((GameWeakness - 1) * 100f).ToString("F1") + "%";
        expValueText.text = "겅험치 획득량 : " + ((GameExpValue - 1) * 100f).ToString("F1") + "%";
        slowValueText.text = "적 이동속도 감소 : " + ((GameSlowValue - 1) * 100f).ToString("F1") + "%";
        bossCoolTimeText.text = "보스 스킬 쿨타임 증가 : " + ((GameBossCoolTime - 1) * 100f).ToString("F1") + "%";
        rewardCoinsOnWaveEndAmountText.text = "웨이브 종료 획득 코인 : " + ((rewardCoinsOnWaveEndAmount) * 100).ToString("F1") + "%";
        healReceivedMultiplierText.text = "회복량 증가 : " + ((gameHealReceivedMultiplier - 1) * 100).ToString("F1") + "%";
        unitEvasionText.text = "유닛 회피 확률 : " + ((gameEvasionValue) * 100).ToString("F1") + "%";
        gameSkillTriggerChanceText.text = "스킬 발동 확률 증가 : " + ((GameSkillTriggerChance) * 100).ToString("F1") + "%";
        enemyDebuffDurationMultiplierText.text = "적 디버프 지속시간 증가 : " + ((EnemyDebuffDurationMultiplier - 1) * 100).ToString("F1") + "%";
        enemyHpRatioDamageReductionText.text = "체력비례 데미지 효율 감소 : " + ((EnemyHpRatioDamageReduction) * 100).ToString("F1") + "%";
    }

    // ===== Set (=) =====
    public void SetEnemyHpRatioDamageReduction(float value) { enemyHpRatioDamageReduction = value; MarkStatsDirty(); }
    public void SetExtraAttackTriggerInterval(int value) { extraAttackTriggerInterval = value; MarkStatsDirty(); }

    // ===== Add (+=) =====
    public void AddGameDamage(float value) { gameDamage += value; MarkStatsDirty(); }
    public void AddGameSkillAmount(float value) { gameSkillAmount += value; MarkStatsDirty(); }
    public void AddGameDefense(float value) { gameDefense += value; MarkStatsDirty(); }
    public void AddGameMaxHealth(float value) { gameMaxHealth += value; MarkStatsDirty(); }
    public void AddGameMaxShield(float value) { gameMaxShield += value; MarkStatsDirty(); }
    public void AddGameCriticalPer(float value) { gameCriticalPer += value; MarkStatsDirty(); }
    public void AddGameCriticalDamage(float value) { gameCriticalDamage += value; MarkStatsDirty(); }
    public void AddGameAttackSpeed(float value) { gameAttackSpeed += value; MarkStatsDirty(); }
    public void AddGameRange(float value) { gameRange += value; MarkStatsDirty(); }
    public void AddGameCoolTime(float value) { gameCoolTime += value; MarkStatsDirty(); }
    public void AddGameMaxAbilityCount(float value) { gameMaxAbilityCount += value; MarkStatsDirty(); }
    public void AddGameGetCoin(float value) { gameGetCoin += value; MarkStatsDirty(); }
    public void AddGameGetDarkMatteCoin(float value) { gameGetDarkMatteCoin += value; MarkStatsDirty(); }
    public void AddGameSkillTriggerChance(float value) { gameSkillTriggerChance += value; MarkStatsDirty(); }
    public void AddGameDefensePenetration(float value) { gameDefensePenetration += value; MarkStatsDirty(); }
    public void AddGameAttributeResistancePenetration(float value) { gameAttributeResistancePenetration += value; MarkStatsDirty(); }
    public void AddGameAttributeDamageMultiplier(float value) { gameAttributeDamageMultiplier += value; MarkStatsDirty(); }

    public void AddGameHealingMultiplier(float value) { gameHealingMultiplier += value; MarkStatsDirty(); }
    public void AddGameHealingReceivedMultiplier(float value) { gameHealingReceivedMultiplier += value; MarkStatsDirty(); }
    public void AddGameShieldMultiplier(float value) { gameShieldMultiplier += value; MarkStatsDirty(); }
    public void AddGameShieldReceivedMultiplier(float value) { gameShieldReceivedMultiplier += value; MarkStatsDirty(); }



    public void AddGameAreaDamage(float value) { gameAreaDamage += value; MarkStatsDirty(); }
    public void AddGameSingleTargetDamage(float value) { gameSingleTargetDamagefloat += value; MarkStatsDirty(); }
    public void AddCoinDamage(float value) { coinDamage += value; MarkStatsDirty(); }
    public void AddGameWeakness(float value) { gameWeakness += value; MarkStatsDirty(); }
    public void AddGameExpValue(float value) { gameExpValue += value; MarkStatsDirty(); }
    public void AddGameSlowValue(float value) { gameSlowValue += value; MarkStatsDirty(); }
    public void AddGameBossCoolTime(float value) { gameBossCoolTime += value; MarkStatsDirty(); }
    public void AddRewardCoinsOnWaveEndAmount(float value) { rewardCoinsOnWaveEndAmount += value; MarkStatsDirty(); }
    public void AddUnitHealOnWaveEndAmount(float value) { unitHealOnWaveEndAmount += value; MarkStatsDirty(); }
    public void AddUnitCoolTimeRecoveryOnWaveEndAmount(float value) { unitCoolTimeRecoveryOnWaveEndAmount += value; MarkStatsDirty(); }
    public void AddUnitShieldOnWaveEndAmount(float value) { unitShieldOnWaveEndAmount += value; MarkStatsDirty(); }
    public void AddUnitSaleRewardMultiplier(int value) { unitSaleRewardMultiplier += value; MarkStatsDirty(); }
    public void AddCardSpawnProbabilityBonus(int value) { cardSpawnProbabilityBonus += value; MarkStatsDirty(); }
    public void AddShowSkillCardCount(int value) { showSkillCardCount += value; MarkStatsDirty(); }
    public void AddManaRegenRate(float value) { manaRegenRate += value; MarkStatsDirty(); }
    public void AddHealReceivedMultiplier(float value) { gameHealReceivedMultiplier += value; MarkStatsDirty(); }
    public void AddEnemyDebuffDurationMultiplier(float value) { enemyDebuffDurationMultiplier += value; MarkStatsDirty(); }
    public void AddGameUnitEvasionValue(float value) { gameEvasionValue += value; MarkStatsDirty(); }
    public void AddExtraAttackTriggerInterval(int value) { extraAttackTriggerInterval += value; MarkStatsDirty(); }


    // ===== Multiply (*=) =====
    public void MultiplyGameDamage(float value) { gameDamage *= value; MarkStatsDirty(); }
    public void MultiplyGameSkillAmount(float value) { gameSkillAmount *= value; MarkStatsDirty(); }
    public void MultiplyGameDefense(float value) { gameDefense *= value; MarkStatsDirty(); }
    public void MultiplyGameMaxHealth(float value) { gameMaxHealth *= value; MarkStatsDirty(); }
    public void MultiplyGameMaxShield(float value) { gameMaxShield *= value; MarkStatsDirty(); }
    public void MultiplyGameCriticalPer(float value) { gameCriticalPer *= value; MarkStatsDirty(); }
    public void MultiplyGameCriticalDamage(float value) { gameCriticalDamage *= value; MarkStatsDirty(); }
    public void MultiplyGameAttackSpeed(float value) { gameAttackSpeed *= value; MarkStatsDirty(); }
    public void MultiplyGameRange(float value) { gameRange *= value; MarkStatsDirty(); }
    public void MultiplyGameCoolTime(float value) { gameCoolTime *= value; MarkStatsDirty(); }
    public void MultiplyGameMaxAbilityCount(float value) { gameMaxAbilityCount *= value; MarkStatsDirty(); }
    public void MultiplyGameGetCoin(float value) { gameGetCoin *= value; MarkStatsDirty(); }
    public void MultiplyGameGetDarkMatteCoin(float value) { gameGetDarkMatteCoin *= value; MarkStatsDirty(); }
    public void MultiplyGameSkillTriggerChance(float value) { gameSkillTriggerChance *= value; MarkStatsDirty(); }
    public void MultiplyCoinDamage(float value) { coinDamage *= value; MarkStatsDirty(); }
    public void MultiplyGameWeakness(float value) { gameWeakness *= value; MarkStatsDirty(); }
    public void MultiplyGameExpValue(float value) { gameExpValue *= value; MarkStatsDirty(); }
    public void MultiplyGameSlowValue(float value) { gameSlowValue *= value; MarkStatsDirty(); }
    public void MultiplyGameBossCoolTime(float value) { gameBossCoolTime *= value; MarkStatsDirty(); }
    public void MultiplyRewardCoinsOnWaveEndAmount(float value) { rewardCoinsOnWaveEndAmount *= value; MarkStatsDirty(); }
    public void MultiplyUnitHealOnWaveEndAmount(float value) { unitHealOnWaveEndAmount *= value; MarkStatsDirty(); }
    public void MultiplyUnitCoolTimeRecoveryOnWaveEndAmount(float value) { unitCoolTimeRecoveryOnWaveEndAmount *= value; MarkStatsDirty(); }
    public void MultiplyUnitShieldOnWaveEndAmount(float value) { unitShieldOnWaveEndAmount *= value; MarkStatsDirty(); }
    public void MultiplyUnitSaleRewardMultiplier(float value) { unitSaleRewardMultiplier = Mathf.FloorToInt(unitSaleRewardMultiplier * value); MarkStatsDirty(); }
    public void MultiplyManaRegenRate(float value) { manaRegenRate *= value; MarkStatsDirty(); }
    public void MultiplyHealReceivedMultiplier(float value) { gameHealReceivedMultiplier *= value; MarkStatsDirty(); }
    public void MultiplyEnemyDebuffDurationMultiplier(float value) { enemyDebuffDurationMultiplier *= value; MarkStatsDirty(); }


    // ===== Divide (/=) =====
    public void DivideGameDamage(float value) { gameDamage /= value; MarkStatsDirty(); }
    public void DivideGameSkillAmount(float value) { gameSkillAmount /= value; MarkStatsDirty(); }
    public void DivideGameDefense(float value) { gameDefense /= value; MarkStatsDirty(); }
    public void DivideGameMaxHealth(float value) { gameMaxHealth /= value; MarkStatsDirty(); }
    public void DivideGameMaxShield(float value) { gameMaxShield /= value; MarkStatsDirty(); }
    public void DivideGameCriticalPer(float value) { gameCriticalPer /= value; MarkStatsDirty(); }
    public void DivideGameCriticalDamage(float value) { gameCriticalDamage /= value; MarkStatsDirty(); }
    public void DivideGameAttackSpeed(float value) { gameAttackSpeed /= value; MarkStatsDirty(); }
    public void DivideGameRange(float value) { gameRange /= value; MarkStatsDirty(); }
    public void DivideGameCoolTime(float value) { gameCoolTime /= value; MarkStatsDirty(); }
    public void DividGameMaxAbilityCount(float value) { gameMaxAbilityCount /= value; MarkStatsDirty(); }
    public void DivideGameGetCoin(float value) { gameGetCoin /= value; MarkStatsDirty(); }
    public void DivideGameGetDarkMatteCoin(float value) { gameGetDarkMatteCoin /= value; MarkStatsDirty(); }
    public void DivideGameSkillTriggerChance(float value) { gameSkillTriggerChance /= value; MarkStatsDirty(); }
    public void DivideCoinDamage(float value) { coinDamage /= value; MarkStatsDirty(); }
    public void DivideGameWeakness(float value) { gameWeakness /= value; MarkStatsDirty(); }
    public void DivideGameExpValue(float value) { gameExpValue /= value; MarkStatsDirty(); }
    public void DivideGameSlowValue(float value) { gameSlowValue /= value; MarkStatsDirty(); }
    public void DivideGameBossCoolTime(float value) { gameBossCoolTime /= value; MarkStatsDirty(); }
    public void DivideRewardCoinsOnWaveEndAmount(float value) { rewardCoinsOnWaveEndAmount /= value; MarkStatsDirty(); }
    public void DivideUnitHealOnWaveEndAmount(float value) { unitHealOnWaveEndAmount /= value; MarkStatsDirty(); }
    public void DivideUnitCoolTimeRecoveryOnWaveEndAmount(float value) { unitCoolTimeRecoveryOnWaveEndAmount /= value; MarkStatsDirty(); }
    public void DivideUnitShieldOnWaveEndAmount(float value) { unitShieldOnWaveEndAmount /= value; MarkStatsDirty(); }
    public void DivideUnitSaleRewardMultiplier(float value) { unitSaleRewardMultiplier = Mathf.FloorToInt(unitSaleRewardMultiplier / value); MarkStatsDirty(); }
    public void DivideManaRegenRate(float value) { manaRegenRate /= value; MarkStatsDirty(); }
    public void DivideHealReceivedMultiplier(float value) { gameHealReceivedMultiplier /= value; MarkStatsDirty(); }
    public void DivideEnemyDebuffDurationMultiplier(float value) { enemyDebuffDurationMultiplier /= value; MarkStatsDirty(); }
}
