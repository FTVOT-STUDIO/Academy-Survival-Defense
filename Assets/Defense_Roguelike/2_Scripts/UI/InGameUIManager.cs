using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Inst;
    [Tab("In Game HUD")]
    [Header("# In Game HUD")]
    public GameObject gameIntroPanel;
    public GameObject inGameHUD;
    public GameObject eventUiGroup;
    public TMP_Text coinText;
    public TMP_Text darkMatteCoinText;
    public TMP_Text spawnUnitCountText;
    public Text abilityCountText;
    public Animator abilityCountAnim;
    public GameObject abilityButtonParent;
    public Slider abilitySlider;
    public RectTransform coinRect;
    public RectTransform darkMatteCoinRect;
    public Slider jointOperationBossHpSlider;
    public FactionBonusUIContoroller inGamefactionBonusController;
    [Space]
    public TMP_Text waveText;
    public TMP_Text waveAlertText;
    public Animator waveTextAnim;
    public TMP_Text levelText;
    public TMP_Text expValueText;
    public Slider expSlider;
    public TMP_Text sephiraHealthText;
    [Space]
    public LevelUpPanelManger levelUpPanelManger;
    public GameObject anomalyPanel;
    public GameObject gameStatePanel;
    public GameObject bossAppearPanel;
    public GameObject waveChangedPanel;
    [Space]
    public GameObject levelUpPanel;
    public GameObject ResultUIPanel;
    [Space]
    public GameObject nextBossPanel;
    public GameObject bossIntroUI;
    public GameObject bossClearPanel;
    [Space]
    public UnitInfoUI unitInfoUI;
    public StateCardUIManager stateCardUIManager;
    public GameObject anomalyButton;
    public Toggle speedUpToggle;
    [SerializeField] GameObject speedToggleStateGroup;
    [SerializeField] Image speedToggleIcon;
    [SerializeField] TMP_Text curGameSpeedText;
    [Space]
    public GameObject topGroup;
    public GameObject bottomGroup;
    public GameObject bordor;
    [Space]
    public GameObject operationConditionPanel;
    public GameObject operationMainPanel;
    public GameObject abilityReadyEffect;
    [Header("# Canvas")]
    [Tab("Canvas")]
    public GameObject inGameCanvas;
    public enum CanvasType { lobby, inGame, Gacha }
    [SerializeField] private Camera mainCamera;
    [SerializeField] public Camera uiCamera;
    public TMP_Text skillCardCountText;
    public TMP_Text skillCardCostText;


    void Awake()
    {
        Inst = this;
    }

    public void ToggleGameplayPanels(bool active)
    {
        anomalyPanel.SetActive(active);
        gameStatePanel.SetActive(active);
        levelUpPanelManger.gameObject.SetActive(active);

    }

    public void ToggleMainHUD(bool active)
    {
        topGroup.SetActive(active);
        bottomGroup.SetActive(active);
        InGameEventManager.Inst.OnMainHUDToggled?.Invoke();
    }

    private void StartGame()
    {
        LobbyManager.Inst.lobbyUiGroup.SetActive(false);
        inGameCanvas.SetActive(true);
        gameIntroPanel.SetActive(true);
        inGameHUD.SetActive(true);
        GameManager.Inst.fieldManager.gameObject.SetActive(true);
        GameManager.Inst.mapManager.Init();
        GameManager.Inst.spawner.SetActive(true);
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.GamePlay, 1);
        AudioManager.Inst.StopBgm();
        DataManager.Inst.SaveData();

        GameManager.Inst.InitializedGameState();

        for (int i = 0; i < GachaManager.Inst.rewardListContent.transform.childCount; i++)
        {
            Destroy(GachaManager.Inst.rewardListContent.transform.GetChild(i).gameObject);
        }

        PoolManager.Inst.Clear();
        EffectManager.Inst.Clear();
        UnitCombatStatManager.Inst.Init();
        GameManager.Inst.fieldManager.fieldSephira.SetUp();
    }

    private void SetGameMode(GameManager.GameMode gamemode)
    {
        GameManager.Inst.gameMode = gamemode;
        GameModeManager.Inst.curGameMode = gamemode;
        switch (gamemode)
        {
            case GameManager.GameMode.Normal:

                GameManager.Inst.curGameDifficulty = PlayerManager.Inst.difficultyType;

                DeckManager.Inst.SetUpDeck(DeckManager.Inst.curDeckUnitDatas, DeckManager.Inst.curDeckSephiraData, DeckManager.Inst.curDeckFieldData);

                switch (GameManager.Inst.curGameDifficulty)
                {
                    case DifficultyData.DifficultyType.Normal:
                        GameManager.Inst.currentGameMode = new NormalMode();
                        break;
                    case DifficultyData.DifficultyType.Hard:
                        GameManager.Inst.currentGameMode = new HardMode();
                        break;
                }

                DeckManager.Inst.ApplyCurrentDeckToGameDeck(DeckManager.Inst.curDeckUnitDatas, DeckManager.Inst.curDeckSephiraData, DeckManager.Inst.curDeckFieldData, DeckManager.Inst.curDeckFactionBonus);
                ;
                break;
            case GameManager.GameMode.JointOperation:
                DeckManager.Inst.SetUpDeck(DeckManager.Inst.jointOperationDeckUnitDatas, DeckManager.Inst.jointOperationDeckSephiraData, DeckManager.Inst.jointOperationDeckFieldData);

                GameManager.Inst.curGameDifficulty = DifficultyData.DifficultyType.Normal;
                switch (GameModeManager.Inst.jointOperationsManager.curJointOperationData.operationType)
                {
                    case JointOperationData.OpertaionType.Metatron:
                        GameManager.Inst.currentGameMode = new JointOperationMetatronMode();
                        break;
                }

                DeckManager.Inst.ApplyCurrentDeckToGameDeck(DeckManager.Inst.jointOperationDeckUnitDatas, DeckManager.Inst.jointOperationDeckSephiraData, DeckManager.Inst.jointOperationDeckFieldData, DeckManager.Inst.jointOpertaionDeckFactionBonus);
                break;
        }
        GameManager.Inst.currentGameMode.Initialize(GameManager.Inst);
        DataManager.Inst.SaveData();
    }

    public void OnStartButton_NormalMode()
    {
        SetGameMode(GameManager.GameMode.Normal);
        StartGame();
    }
    public void OnStartButton_Arcade()
    {
        SetGameMode(GameManager.GameMode.Arcade);
        StartGame();
    }
    public void OnStartButton_JointOpertaion()
    {
        SetGameMode(GameManager.GameMode.JointOperation);
        StartGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        SceneChanger.Inst.isRestarting = true;
        SceneChanger.Inst.ChangeScene(SceneChanger.SceneType.RestartGame);
        DataManager.Inst.SaveData();
    }

    public void BackToLobby()
    {
        Time.timeScale = 1;
        SceneChanger.Inst.ChangeScene(SceneChanger.SceneType.RestartGame);
        DataManager.Inst.SaveData();
    }

    public void SpawnCard()
    {
        if (GameManager.Inst.darkMatteCoin >= GameFlowManager.Inst.spentCardSpawnCost && !levelUpPanel.activeSelf)
        {
            GameManager.Inst.ModifyDarkMatteCoin(-GameFlowManager.Inst.spentCardSpawnCost);
            GameFlowManager.Inst.spentCardSpawnCost++;

            GameManager.Inst.skillCardManager.GetSkillCard(1);

            skillCardCostText.text = "x" + GameFlowManager.Inst.spentCardSpawnCost.ToString();
        }
    }

    public void TryBonusCardDrop()
    {
        if (Random.value <= 0.05f)
        {
            GameManager.Inst.skillCardManager.selectCardCount++;
            levelUpPanel.SetActive(true);
            skillCardCountText.text = "x" + GameFlowManager.Inst.spentCardSpawnCost.ToString();
            GameManager.Inst.Pause();
            skillCardCountText.text = "선택 가능한 카드 : " + GameManager.Inst.skillCardManager.selectCardCount + " 개";

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.cheers);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Special);
        }
    }

    public void ShowResourceGainEffect(Transform targetPos, int amount, ResourcesData.ResourcesType resourceType)
    {
        GameObject resourcePrefab = PoolManager.Inst.Get(PoolManager.PrefabType.InGameResourceVisualizer, targetPos.position);

        InGameResourceVisualizer resourceVisualizer = resourcePrefab.GetComponentInChildren<InGameResourceVisualizer>();
        FloatingText floatingText = resourcePrefab.GetComponent<FloatingText>();

        resourceVisualizer.Init(resourceType);
        floatingText.InitializeText("+" + amount, 45, Color.white);
    }

    public void ShowResourceGainEffect(RectTransform targetRect, int amount, ResourcesData.ResourcesType resourceType)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetRect.position);

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5f));

        GameObject resourcePrefab = PoolManager.Inst.Get(
            PoolManager.PrefabType.InGameResourceVisualizer,
            worldPos
        );

        InGameResourceVisualizer resourceVisualizer =
            resourcePrefab.GetComponentInChildren<InGameResourceVisualizer>();

        FloatingText floatingText =
            resourcePrefab.GetComponent<FloatingText>();

        resourceVisualizer.Init(resourceType);
        floatingText.InitializeText("+" + amount, 45, Color.white);
    }

    public void ChangeCount(ref int count, int delta, TMP_Text uiText, string prefix = "", string suffix = "")
    {
        count += delta;
        if (count < 0) count = 0;

        uiText.text = prefix + count.ToString() + suffix;
    }

    public void SwitchToMainOperationPanel()
    {
        operationConditionPanel.SetActive(false);
        operationMainPanel.SetActive(true);
    }

    public void OnSpeedUpToggle()
    {
        if (speedUpToggle.isOn)
        {
            speedToggleIcon.color = new Color(0, 1, 0.9f);
            curGameSpeedText.color = new Color(0, 1, 0.9f);
            curGameSpeedText.text = "2.0X";
            Time.timeScale = 2;
            // AudioManager.Inst.PlaySfx(AudioManager.Sfx.speedToggleOn);
        }

        else
        {
            speedToggleIcon.color = Color.white;
            curGameSpeedText.color = Color.white;
            curGameSpeedText.text = "1.0X";
            Time.timeScale = 1;
            // AudioManager.Inst.PlaySfx(AudioManager.Sfx.speedToggleOff);
        }
    }

    public void OnAbiltyShoot()
    {
        FieldManager.Inst.fieldSephira.OnAbllity();
    }

    public void InitJointOperationBossHpBar()
    {
        jointOperationBossHpSlider.gameObject.SetActive(true);
        jointOperationBossHpSlider.GetComponent<JointOperationBossHpSlider>().Init();
    }

}
