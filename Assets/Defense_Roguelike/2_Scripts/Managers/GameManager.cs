using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using VInspector;
using Random = UnityEngine.Random;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum GameState { Lobby, InGame, Loading }
    public enum GameMode { None, Normal, Arcade, JointOperation }
    public IGameMode currentGameMode;
    public static GameManager Inst;
    [Tab("Main")]
    [Header("# In-Game Info")]
    public bool isDebugging;
    public int exp;
    public int coin;
    public int darkMatteCoin;
    public int level;
    public int[] expArray;
    public int unitSpawnCost;
    public bool isGameStart;
    public DifficultyData.DifficultyType curGameDifficulty;
    public GameState gameState;
    public GameMode gameMode;
    public FieldManager fieldManager;
    public SkillCardManager skillCardManager;
    public GameObject spawner;
    [HideInInspector] public bool isVictory;
    [HideInInspector] public bool isDefeat;
    public event Action OnDelayNextWave;

    [Tab("Wave")]
    [Header("# Wave")]
    public int wave;
    [Space(15)]
    [Header("# Timer")]
    public float spawnTime;
    public float waveDuration;
    public float timer;
    public bool isPause;
    [HideInInspector] public bool isEliteActive;
    [HideInInspector] public bool isAnomaly;
    public bool isIntroDelay;
    [HideInInspector] public bool isBossActive;
    [HideInInspector] public bool isBossSpawnable;
    [HideInInspector] public event Action<int> OnWaveChanged;
    [HideInInspector] public event Action OnLevelUp;
    public EnemySpawnManager spawnManager;
    public UnitInputController unitInputController;

    [Tab("VFX")]
    [Header("# VFX")]
    [SerializeField] WarningVinetteController warningVinetteController;
    [Tab("ETC")]
    [Header("# ETC")]
    public CheatCodeManager cheatCodeManager;
    public UnitSpawn unitSpawn;
    public Transform unitManagerGroup;
    public MapManager mapManager;
    private Coroutine delayNextWaveCoroutine;

    void Awake()
    {
        Inst = this;
        wave = 0;
        timer = waveDuration;

        isEliteActive = false;
        isVictory = false;
        isDefeat = false;

        Time.timeScale = 1;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        OnLevelUp += ShowLevelUpPanel;
        OnWaveChanged += SetWave;
    }

    void Start()
    {
        if (isDebugging)
        {
            expArray = new int[1];
            expArray[0] = 999999;
            return;
        }
        int maxLevel = 100;
        expArray = new int[maxLevel];

        for (int i = 0; i < maxLevel; i++)
        {
            expArray[i] = GetRequiredExp(i);
        }
    }

    int GetRequiredExp(int level)
    {
        return level * (level + 1) / 2;
    }


    void Update()
    {
        currentGameMode?.Update();
    }

    void LateUpdate()
    {
        if (isGameStart)
        {
            if (currentGameMode.CheckEmergencyCondition() || fieldManager.fieldSephira.health <= fieldManager.fieldSephira.maxHealth * 0.3f)
                warningVinetteController.StartBlink();
            else
                warningVinetteController.StopBlink();
        }
    }

    public void ToggleIntroDelay(bool isOn)
    {
        isIntroDelay = isOn;
    }
    public void GameStart()
    {
        if (!isGameStart)
        {
            isGameStart = true;
            isPause = false;
            Time.timeScale = 1;
            currentGameMode.GrantResources();
            currentGameMode.InitializeOnFirstStart();
            fieldManager.fieldSephira.UpdateAbilityBar();
            GameStatManager.Inst.MarkStatsDirty();

            for (int i = 0; i < DeckManager.Inst.curGameDeckFactionBonus.Count; i++)
            {
                DeckManager.Inst.factionBonusCalculator.ApplyFactionBonusStat(DeckManager.Inst.curGameDeckFactionBonus[i]);
            }

            DeckManager.Inst.GenerateTriggerUnitManager();
            if (DeckManager.Inst.curDeckFieldData.fieldSkill is IFieldOnEnablePassive fieldOnEnablePassive)
            {
                fieldOnEnablePassive.ApplyFieldOnEnablePassive(DeckManager.Inst.currentInGameDeck.fieldData, this);
            }
            currentGameMode.SetUpBgm();
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select2);


        }
        else
        {
            isGameStart = true;
            isPause = false;
        }
        DataManager.Inst.SaveData();
    }

    public void InitializedGameState()
    {
        coin = 0;
        darkMatteCoin = 0;
        unitSpawnCost = 10;
        level = 0;
        unitSpawn.freeSpawnCount = 0;
        GameFlowManager.Inst.InitializedGameFlow();
        GameStatManager.Inst.InitializedGameState();
        InGameEventManager.Inst.Initialized();
        InGameUIManager.Inst.levelText.text = "Lv." + level.ToString();
        InGameUIManager.Inst.expValueText.text = exp + "/" + expArray[level];
        InGameUIManager.Inst.expSlider.value = 0;
        SetWave(0);
    }

    public void SetWave(int wave)
    {
        this.wave = wave;
        InGameUIManager.Inst.waveText.text = "WAVE " + wave;
        InGameUIManager.Inst.waveAlertText.text = "WAVE " + wave;
        InGameUIManager.Inst.waveTextAnim.SetTrigger("POP");
    }
    public void OnWaveChangedInvoke()
    {
        OnWaveChanged?.Invoke(wave);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.OnWaveChanged);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select2);
    }

    public void HandleBossWave()
    {
        if (currentGameMode.CanSpawnBoss())
        {
            if (isBossSpawnable)
                return;

            // 보스 웨이브 코루틴 실행
            StartCoroutine(currentGameMode.BossWaveRoutine());
        }
    }

    public void HandleEliteSpawn()
    {
        if (currentGameMode.CanOfferElite() && !isEliteActive && !isAnomaly)
        {
            if (isEliteActive)
                return;

            isAnomaly = true;
            InGameUIManager.Inst.anomalyButton.SetActive(true);
            InGameUIManager.Inst.anomalyButton.transform.localScale = Vector3.one;
            InGameUIManager.Inst.anomalyButton.GetComponent<DOTweenAnimation>().DORestartById("Show");
        }
    }

    public void OnBossKilled()
    {
        EnemySpawnManager.Inst.remainingBosses--;
        currentGameMode.OnBossKilled();
    }

    public void StartBossKilledFlow(float delay)
    {
        StartCoroutine(OnBossKilledCoroutine(delay));
    }

    private IEnumerator OnBossKilledCoroutine(float delay)
    {
        ToggleIntroDelay(true);
        FieldManager.Inst.ResetFieldUnitLocate();
        yield return null;

        isBossActive = false;

        timer = delay;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (currentGameMode.IsGameVictory())
        {
            OnGameVictory();
            yield break;
        }
        InGameUIManager.Inst.bossIntroUI.SetActive(true);
    }

    public void GetExp(int value)
    {
        int count = 0;
        bool isLevelUp = false;
        exp += Mathf.RoundToInt(value * GameStatManager.Inst.GameExpValue);

        while (exp >= expArray[Mathf.Min(level, expArray.Length - 1)])
        {
            isLevelUp = true;
            count++;
            int lastLevel = level;
            int needExp = expArray[Mathf.Min(level, expArray.Length - 1)];

            exp -= needExp;
            level++;

            skillCardManager.selectCardCount++;
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.LevelUpPanel);

            InGameUIManager.Inst.levelText.text = "Lv." + level.ToString();

            ModifyCoin((int)(200 * (1 + level * 0.1f)) * (level - lastLevel));
        }

        if (isLevelUp)
            OnLevelUp.Invoke();

        float curExp = exp;
        float maxExp = expArray[Mathf.Min(level, expArray.Length - 1)];
        InGameUIManager.Inst.expSlider.value = curExp / maxExp;
        InGameUIManager.Inst.expValueText.text = curExp + "/" + expArray[level];
    }

    private void ShowLevelUpPanel()
    {
        if (DebuffManager.Inst.blockLevelUpUI && Random.value < 0.5f)
        {
            skillCardManager.selectCardCount = 0;
            return;
        }
        skillCardManager.GetSkillCard();
    }

    public void Resume()
    {
        for (int i = 0; i < FieldManager.Inst.fieldUnits.Length; i++)
        {
            if (FieldManager.Inst.fieldUnits[i] != null)
            {
                FieldManager.Inst.fieldUnits[i].GetComponent<Unit>().isLive = true;
            }
        }
        isPause = false;
        Time.timeScale = InGameUIManager.Inst.speedUpToggle.isOn ? 2 : 1;
    }

    public void Pause()
    {
        for (int i = 0; i < FieldManager.Inst.fieldUnits.Length; i++)
        {
            if (FieldManager.Inst.fieldUnits[i] != null)
            {
                FieldManager.Inst.fieldUnits[i].GetComponent<Unit>().isLive = false;
            }
        }

        isPause = true;

        Time.timeScale = 0;

    }

    public void DelayNextWave()
    {
        float delay = currentGameMode.DelayNextWave();

        if (delayNextWaveCoroutine != null)
        {
            StopCoroutine(delayNextWaveCoroutine);
            delayNextWaveCoroutine = null;
        }

        delayNextWaveCoroutine = StartCoroutine(DelayNextWaveCoroutine(delay));
    }

    private IEnumerator DelayNextWaveCoroutine(float delay)
    {
        if (!isGameStart)
            InGameUIManager.Inst.bordor.SetActive(true);

        timer = delay;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        ToggleIntroDelay(false);

        if (!isGameStart)
            InGameUIManager.Inst.bordor.SetActive(false);

        GameStart();
        OnDelayNextWave?.Invoke();

        delayNextWaveCoroutine = null;
    }

    public void UnitUpgradeStats()
    {
        for (int i = 0; i < FieldManager.Inst.fieldUnits.Length; i++)
        {
            if (FieldManager.Inst.fieldUnits[i] != null)
            {
                FieldManager.Inst.fieldUnits[i].GetComponent<Unit>().RecalculateStats();
            }
        }

        FieldManager.Inst.fieldSephira.RecalculateStats();
    }

    public void ModifyCoin(int amount)
    {
        int previousGold = coin;
        if (amount >= 0)
            amount *= GameFlowManager.Inst.coinGainMultiplier;
        coin += amount;


        if (amount > 0)
        {
            InGameUIManager.Inst.ShowResourceGainEffect(InGameUIManager.Inst.coinRect, amount, ResourcesData.ResourcesType.coin);
        }
        else
        {
            GameFlowManager.Inst.ChangeSpentCoin(Mathf.Abs(amount));
        }

        NumberCounter.StartCounting(this, coin, previousGold, 0.3f, InGameUIManager.Inst.coinText);
        InGameUIManager.Inst.coinText.GetComponentInParent<PopUpUI>(true).PopUpText(InGameUIManager.Inst.coinText.transform.parent.gameObject);
        InGameEventManager.Inst.GetCoin?.Invoke();

        InGameUIManager.Inst.coinText.color = coin < 0 ? Color.red : Color.white;
    }

    public void ModifyDarkMatteCoin(int amount)
    {
        int previousDarkMatteCoin = darkMatteCoin;
        if (amount >= 0)
            amount *= GameFlowManager.Inst.darkMatteGainMultiplier;
        darkMatteCoin += amount;

        if (darkMatteCoin < 0)
            darkMatteCoin = 0;

        if (amount > 0)
        {
            InGameUIManager.Inst.ShowResourceGainEffect(InGameUIManager.Inst.darkMatteCoinRect, amount, ResourcesData.ResourcesType.darkMatteCoin);
        }
        else
        {
            GameFlowManager.Inst.ChangeSpentDarkMatteCoin(Mathf.Abs(amount));
        }


        NumberCounter.StartCounting(this, darkMatteCoin, previousDarkMatteCoin, 0.3f, InGameUIManager.Inst.darkMatteCoinText);
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.UseDarkMatte, amount);
        InGameUIManager.Inst.darkMatteCoinText.GetComponentInParent<PopUpUI>(true).PopUpText(InGameUIManager.Inst.darkMatteCoinText.transform.parent.gameObject);
    }
    public void OnGameVictory()
    {
        isVictory = true;
        InGameUIManager.Inst.ResultUIPanel.SetActive(true);
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.GameVictory, 1);
    }
}
