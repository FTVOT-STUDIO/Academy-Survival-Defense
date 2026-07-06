using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;


public class DataManager : MonoBehaviour
{
    public static DataManager Inst;

    public PlayerData playerData = new PlayerData();
    [Tab("Main Data")]
    [Header("# Main Data")]
    public List<UnitData> unitDatas;
    public List<UnitData> sephiraUnitDatas;
    public List<FieldData> fieldDatas;
    public List<QuestData> questDatas;
    public List<ShopResourcesData> shopResourcesDatas;
    public List<JointOperationData> jointOperationBossDatas;
    [Tab("Base Data")]
    [Header("# Base Datas SO")]
    public UnitData[] baseUnitDatas;
    public UnitData baseSephiraData;
    public FieldData baseFieldData;

    string path;
    string fileName = "PlayerData.Json";

    public bool IsInitialized = false;

    void Awake()
    {
        Inst = this;

        path = Path.Combine(Application.persistentDataPath + "/");
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerManager.Inst != null && DeckManager.Inst != null && QuestManager.Inst != null && EquipmentManager.Inst != null && ShopManager.Inst != null && GameModeManager.Inst != null && DifficultyManager.Inst != null);
        LoadData();
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    public void SaveData()
    {
        if (DataLoadManager.Inst == null)
        {
            Debug.Log("<color=Red>[DataLoadManager is Not Instance]</color>");
            return;
        }

        if (!DataLoadManager.Inst.isSceneInitialized)
        {
            Debug.Log("<color=Red>[Scene is Not Load]</color>");
            return;
        }

        Debug.Log("<color=Yellow>[Save Data success]</color>");

        playerData.playerName = PlayerManager.Inst.playerName;

        playerData.gem = PlayerManager.Inst.gem;
        playerData.gold = PlayerManager.Inst.gold;
        playerData.voidTicket = PlayerManager.Inst.voidTicket;

        playerData.level = PlayerManager.Inst.level;
        playerData.levelUpAmount = PlayerManager.Inst.levelUpAmount;
        playerData.curLevelExp = PlayerManager.Inst.curLevelExp;
        playerData.maxLevelExp = PlayerManager.Inst.maxLevelExp;

        playerData.deckListIndex = DeckManager.Inst.deckListIndex;
        playerData.isTutorial = PlayerManager.Inst.isTutorial;
        playerData.isLevelUp = PlayerManager.Inst.isLevelUp;
        playerData.isJointOperationUnlocked = PlayerManager.Inst.isJointOperationUnlocked;

        playerData.questPoint = QuestManager.Inst.questPoint;

        playerData.lastLoginTime = DateTime.Now.ToString("o");

        playerData.difficultyType = PlayerManager.Inst.difficultyType;
        playerData.curDifficultyId = DifficultyManager.Inst.curDifficultyData.difficultyId;

        playerData.sUnitDatas.Clear();
        playerData.sSephiraDatas.Clear();
        playerData.sFieldDatas.Clear();
        playerData.sQuestDatas.Clear();
        playerData.deckList.Clear();
        playerData.shopData.shopItemDatas.Clear();
        playerData.shopData.curShopUnitsID.Clear();
        playerData.shopData.curShopResourcesID.Clear();

        foreach (UnitData unitData in unitDatas)
        {
            SUnitDatas unitDataToSave = new SUnitDatas();
            unitDataToSave.SyncUnitState(unitData, SUnitDatas.DataType.save);
            playerData.sUnitDatas.Add(unitDataToSave);
        }

        foreach (UnitData sephiraData in sephiraUnitDatas)
        {
            SUnitDatas sephiraDataToSave = new SUnitDatas();
            sephiraDataToSave.SyncUnitState(sephiraData, SUnitDatas.DataType.save);
            playerData.sSephiraDatas.Add(sephiraDataToSave);
        }

        foreach (FieldData fieldData in fieldDatas)
        {
            SFieldDatas fieldDataToSave = new SFieldDatas();
            fieldDataToSave.SaveInit(fieldData);
            playerData.sFieldDatas.Add(fieldDataToSave);
        }

        foreach (QuestData questData in questDatas)
        {
            SQuestDatas questDataToSave = new SQuestDatas();
            questDataToSave.SaveInit(questData);
            playerData.sQuestDatas.Add(questDataToSave);
        }

        playerData.isQuestRewardClaimed = new bool[QuestManager.Inst.questRewardButtons.Length];

        for (int i = 0; i < QuestManager.Inst.questRewardButtons.Length; i++)
        {
            playerData.isQuestRewardClaimed[i] = QuestManager.Inst.questRewardButtons[i].isQuestRewardClaimed;
        }


        foreach (DeckList deck in DeckManager.Inst.deckLists)
        {
            DeckListData deckData = new DeckListData();
            deckData.unitDataIDList = new List<int>();

            foreach (UnitData unitData in deck.unitDatas)
            {
                deckData.unitDataIDList.Add(unitData.unitID);
            }

            deckData.sephiraDataID = deck.sephiraData.unitID;
            deckData.fieldDataID = deck.fieldData.fieldID;

            playerData.deckList.Add(deckData);
        }
        playerData.currentDeckUnit.unitDataIDList = new List<int>();
        for (int i = 0; i < DeckManager.Inst.curDeckUnitDatas.Count; i++)
        {
            playerData.currentDeckUnit.unitDataIDList.Add(DeckManager.Inst.curDeckUnitDatas[i].unitID);
        }
        playerData.currentDeckUnit.sephiraDataID = DeckManager.Inst.curDeckSephiraData.unitID;
        playerData.currentDeckUnit.fieldDataID = DeckManager.Inst.curDeckFieldData.fieldID;

        if (playerData.isJointOperationUnlocked)
        {
            playerData.sJointOperationDeck.unitDataIDList = new List<int>();
            for (int i = 0; i < DeckManager.Inst.jointOperationDeckUnitDatas.Count; i++)
            {
                playerData.sJointOperationDeck.unitDataIDList.Add(DeckManager.Inst.jointOperationDeckUnitDatas[i].unitID);
            }
            playerData.sJointOperationDeck.sephiraDataID = DeckManager.Inst.jointOperationDeckSephiraData.unitID;
            playerData.sJointOperationDeck.fieldDataID = DeckManager.Inst.jointOperationDeckFieldData.fieldID;

            playerData.curJointOperatioId = GameModeManager.Inst.jointOperationsManager.curJointOperationData.operationId;

            playerData.curJointOperationDifficulty = GameModeManager.Inst.jointOperationsManager.curJointOperationDifficulty;
        }

        playerData.shopData.shopItemDatas = new List<SShopItemDatas>();

        for (int i = 0; i < ShopManager.Inst.shopItems.Length; i++)
        {
            SShopItemDatas shopItemDatas = new SShopItemDatas();
            shopItemDatas.SaveInit(ShopManager.Inst.shopItems[i]);

            playerData.shopData.shopItemDatas.Add(shopItemDatas);
            if (i != 0)
            {
                if (playerData.shopData.shopItemDatas[i].itemType == ShopItem.ItemType.unit)
                {
                    playerData.shopData.curShopUnitsID.Add(shopItemDatas.unitID);
                }
                else
                {
                    playerData.shopData.curShopResourcesID.Add(shopItemDatas.itemID);
                }
            }

        }

        playerData.curGameMode = GameModeManager.Inst.curGameMode;

        playerData.shopData.refreshCount = ShopManager.Inst.refreshCount;

        string data = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path + fileName, data);
    }

    public void LoadData()
    {
        if (File.Exists(path + fileName))
        {
            DeckManager deck = DeckManager.Inst;

            Debug.Log("<color=Yellow>[Load Data Success]</color>");

            IsInitialized = false;

            string data = File.ReadAllText(path + fileName);
            playerData = JsonUtility.FromJson<PlayerData>(data);

            PlayerManager.Inst.playerName = playerData.playerName;

            PlayerManager.Inst.gem = playerData.gem;
            PlayerManager.Inst.gold = playerData.gold;
            PlayerManager.Inst.voidTicket = playerData.voidTicket;

            PlayerManager.Inst.gemText.text = string.Format("{0:n0}", playerData.gem);
            PlayerManager.Inst.goldText.text = string.Format("{0:n0}", playerData.gold);

            PlayerManager.Inst.level = playerData.level;
            PlayerManager.Inst.levelUpAmount = playerData.levelUpAmount;
            PlayerManager.Inst.curLevelExp = playerData.curLevelExp;
            PlayerManager.Inst.maxLevelExp = playerData.maxLevelExp;

            PlayerManager.Inst.maxLevelExp = PlayerManager.Inst.levelExpTable[PlayerManager.Inst.level];
            PlayerManager.Inst.isTutorial = playerData.isTutorial;
            PlayerManager.Inst.isLevelUp = playerData.isLevelUp;
            PlayerManager.Inst.isJointOperationUnlocked = playerData.isJointOperationUnlocked;

            PlayerManager.Inst.playerLevelUpManager.gameObject.SetActive(PlayerManager.Inst.isLevelUp);
            PlayerManager.Inst.lastLoginTime = playerData.lastLoginTime;

            PlayerManager.Inst.difficultyType = playerData.difficultyType;
            if (playerData.curDifficultyId == 0)
            {
                for (int i = 0; i < DifficultyManager.Inst.difficultyData.Length; i++)
                {
                    if (DifficultyManager.Inst.difficultyData[i].difficultyType == PlayerManager.Inst.difficultyType)
                    {
                        DifficultyManager.Inst.curDifficultyData = DifficultyManager.Inst.difficultyData[i];
                        break;
                    }
                }
            }
            else
            {
                DifficultyManager.Inst.curDifficultyData = GetDifficultyDataById(playerData.curDifficultyId);
            }

            PlayerManager.Inst.InitializedPlayerInfo();
            QuestManager.Inst.questPoint = playerData.questPoint;
            DeckManager.Inst.deckListIndex = playerData.deckListIndex;
            TutorialManager.Inst.nickNameInputField.SetActive(playerData.isTutorial);

            foreach (SUnitDatas savedUnitData in playerData.sUnitDatas)
            {
                UnitData unitData = unitDatas.Find(u => u.unitID == savedUnitData.unitID);
                if (unitData != null)
                {
                    savedUnitData.SyncUnitState(unitData, SUnitDatas.DataType.load);
                }
                else
                {
                    Debug.LogWarning($"UnitData not found for ID {savedUnitData.unitID}");
                }

            }

            foreach (SUnitDatas savedSephiraData in playerData.sSephiraDatas)
            {
                UnitData sephiraData = sephiraUnitDatas.Find(u => u.unitID == savedSephiraData.unitID);
                if (sephiraData != null)
                {
                    savedSephiraData.SyncUnitState(sephiraData, SUnitDatas.DataType.load);
                }
                else
                {
                    Debug.LogWarning($"SephiraData not found for ID {savedSephiraData.unitID}");
                }
            }

            foreach (SFieldDatas savedFieldData in playerData.sFieldDatas)
            {
                FieldData fieldData = fieldDatas.Find(u => u.fieldID == savedFieldData.fieldID);
                if (fieldData != null)
                {
                    savedFieldData.LoadInit(fieldData);
                }
                else
                {
                    Debug.LogWarning($"FieldData not found for ID {savedFieldData.fieldID}");
                }
            }

            foreach (SQuestDatas savedQuestData in playerData.sQuestDatas)
            {
                QuestData questData = questDatas.Find(u => u.questID == savedQuestData.questID);
                if (questData != null)
                {
                    savedQuestData.LoadInit(questData);
                }
                else
                {
                    Debug.LogWarning($"QuestData not found for ID {savedQuestData.questID}");
                }
            }
            for (int i = 0; i < playerData.deckList.Count; i++)
            {
                DeckListData deckData = playerData.deckList[i];
                DeckManager.Inst.deckLists[i].unitDatas = new UnitData[deckData.unitDataIDList.Count];

                for (int j = 0; j < deckData.unitDataIDList.Count; j++)
                {
                    DeckManager.Inst.deckLists[i].unitDatas[j] = GetUnitDataById(deckData.unitDataIDList[j]);
                }
                DeckManager.Inst.deckLists[i].sephiraData = GetUnitDataById(deckData.sephiraDataID);
                DeckManager.Inst.deckLists[i].fieldData = GetFieldDataById(deckData.fieldDataID);
            }

            DeckManager.Inst.curDeckUnitDatas.Clear();
            for (int i = 0; i < playerData.currentDeckUnit.unitDataIDList.Count; i++)
            {
                DeckManager.Inst.curDeckUnitDatas.Add(GetUnitDataById(playerData.currentDeckUnit.unitDataIDList[i]));
            }
            DeckManager.Inst.curDeckSephiraData = GetUnitDataById(playerData.currentDeckUnit.sephiraDataID);
            DeckManager.Inst.curDeckFieldData = GetFieldDataById(playerData.currentDeckUnit.fieldDataID);

            EquipmentManager.Inst.GenerateAllUnitAndFieldLists();
            EquipmentManager.Inst.RefreshUnitContentSize(EquipmentManager.Inst.tabContentRect, unitDatas);
            DeckManager.Inst.InitializeAllDeckLists();

            DeckManager.Inst.lobbyDeckToggle.InitIndex(playerData.deckListIndex);
            DeckManager.Inst.factionBonusCalculator.

            SetFactionBonusInfo(deck.deckLists[deck.deckListIndex].unitDatas, deck.deckLists[deck.deckListIndex].sephiraData, FactionBonusCalculator.FactionBonusView.Normal);

            DifficultyManager.Inst.InitializeDifficultyToggles();
            DifficultyManager.Inst.UpdateBackgroundByDifficulty();

            if (PlayerManager.Inst.isJointOperationUnlocked)
            {
                GameModeManager.Inst.jointOperationsManager.curJointOperationDifficulty = playerData.curJointOperationDifficulty;

                DeckManager.Inst.jointOperationDeckUnitDatas.Clear();
                for (int i = 0; i < playerData.sJointOperationDeck.unitDataIDList.Count; i++)
                {
                    DeckManager.Inst.jointOperationDeckUnitDatas.Add(GetUnitDataById(playerData.sJointOperationDeck.unitDataIDList[i]));
                }
                DeckManager.Inst.jointOperationDeckSephiraData = GetUnitDataById(playerData.sJointOperationDeck.sephiraDataID);
                DeckManager.Inst.jointOperationDeckFieldData = GetFieldDataById(playerData.sJointOperationDeck.fieldDataID);

                if (playerData.curJointOperatioId == 0)
                {
                    GameModeManager.Inst.jointOperationsManager.curJointOperationData = GameModeManager.Inst.jointOperationsManager.GetRandomJointOperationData();
                }
                else
                {
                    GameModeManager.Inst.jointOperationsManager.curJointOperationData = GetJointOperationDataById(playerData.curJointOperatioId);
                }

                DeckManager.Inst.factionBonusCalculator.SetFactionBonusInfo(deck.jointOperationDeckUnitDatas.ToArray(), deck.jointOperationDeckSephiraData, FactionBonusCalculator.FactionBonusView.JointOpertaion);

                InGameUIManager.Inst.SwitchToMainOperationPanel();
                DeckManager.Inst.jointOperationsDeckManager.InitializeJointOperationUIAndDeck();
                DeckManager.Inst.InitializeJointOperationsDeck();
                GameModeManager.Inst.jointOperationsManager.InitJointOperationLobby();


            }
            else
            {
                GameModeManager.Inst.jointOperationsManager.InitContentUnlockConditionCount();
            }

            DateTime lastRefreshTime = DateTime.Parse(PlayerManager.Inst.lastLoginTime);
            DateTime todayMidnight = DateTime.Today;
            DateTime now = DateTime.Now;

            if (lastRefreshTime < todayMidnight)
            {
                ShopManager.Inst.GenerateShopItems();
                QuestManager.Inst.ResetQuestProgress();
            }
            else
            {
                for (int i = 0; i < ShopManager.Inst.shopItems.Length; i++)
                {
                    playerData.shopData.shopItemDatas[i].LoadInit(ShopManager.Inst.shopItems[i]);

                    if (i != 0)
                    {
                        if (playerData.shopData.shopItemDatas[i].itemType == ShopItem.ItemType.unit)
                        {
                            ShopManager.Inst.currentShopUnits[i - 1] = GetUnitDataById(playerData.shopData.shopItemDatas[i].unitID);
                        }
                        else
                        {
                            ShopManager.Inst.currentShopResources[i - 1] = GetResourcesDataById(playerData.shopData.shopItemDatas[i].itemID);
                        }
                    }
                }

                for (int i = 0; i < QuestManager.Inst.questRewardButtons.Length; i++)
                {
                    QuestManager.Inst.questRewardButtons[i].isQuestRewardClaimed = playerData.isQuestRewardClaimed[i];
                }

                ShopManager.Inst.refreshCount = playerData.shopData.refreshCount;
                ShopManager.Inst.RefreshItemList();
                QuestManager.Inst.SetupQuestUI();
            }

            GameModeManager.Inst.curGameMode = playerData.curGameMode;

            IsInitialized = true;

            if (SceneChanger.Inst == null)
            {
                GameObject sceneChangerObj = new GameObject("SceneChanger");
                sceneChangerObj.AddComponent<SceneChanger>();
            }
            if (SceneChanger.Inst.isRestarting)
            {
                LobbyManager.Inst.lobbyUiGroup.SetActive(false);

                switch (GameModeManager.Inst.curGameMode)
                {
                    case GameManager.GameMode.None:
                        Debug.Log("None");
                        LobbyManager.Inst.lobbyUiGroup.SetActive(true);
                        break;
                    case GameManager.GameMode.Normal:
                        Debug.Log("Normal");
                        InGameUIManager.Inst.OnStartButton_NormalMode();
                        break;
                    case GameManager.GameMode.Arcade:
                        InGameUIManager.Inst.OnStartButton_Arcade();
                        break;
                    case GameManager.GameMode.JointOperation:
                        InGameUIManager.Inst.OnStartButton_JointOpertaion();
                        break;
                }

                SceneChanger.Inst.isRestarting = false;
                SaveData();
            }
        }
        else
        {
            Debug.Log("<color=Red>[Load Data Fail]</color>");
            ClearAllData();
        }
    }

    void ClearAllData()
    {
        DeckManager deck = DeckManager.Inst;

        Debug.Log("<color=Blue>[Data All Clear]</color>");
        IsInitialized = false;
        PlayerManager.Inst.gem = 0;
        PlayerManager.Inst.gold = 0;
        PlayerManager.Inst.gemText.text = "0";
        PlayerManager.Inst.goldText.text = "0";
        PlayerManager.Inst.level = 1;
        PlayerManager.Inst.curLevelExp = 0;
        PlayerManager.Inst.voidTicket = 0;
        PlayerManager.Inst.InitializedPlayerInfo();
        DeckManager.Inst.deckListIndex = 0;

        PlayerManager.Inst.isTutorial = true;
        PlayerManager.Inst.isLevelUp = false;
        PlayerManager.Inst.isJointOperationUnlocked = false;

        PlayerManager.Inst.difficultyType = DifficultyData.DifficultyType.Normal;
        for (int i = 0; i < DifficultyManager.Inst.difficultyData.Length; i++)
        {
            if (DifficultyManager.Inst.difficultyData[i].difficultyType == PlayerManager.Inst.difficultyType)
            {
                DifficultyManager.Inst.curDifficultyData = DifficultyManager.Inst.difficultyData[i];
                break;
            }
        }

        TutorialManager.Inst.nickNameInputField.SetActive(true);

        PlayerManager.Inst.lastLoginTime = DateTime.Now.ToString("o");

        foreach (UnitData data in unitDatas)
        {
            SUnitDatas unitDatas = new SUnitDatas();
            if (data != null)
            {
                unitDatas.SyncUnitState(data, SUnitDatas.DataType.reset);
            }
        }

        foreach (UnitData data in sephiraUnitDatas)
        {
            SUnitDatas sephiraDatas = new SUnitDatas();
            if (data != null)
            {
                sephiraDatas.SyncUnitState(data, SUnitDatas.DataType.reset);
            }
        }

        foreach (FieldData data in fieldDatas)
        {
            SFieldDatas fieldDatas = new SFieldDatas();
            if (data != null)
            {
                fieldDatas.ResetData(data);
            }
        }

        foreach (QuestData data in questDatas)
        {
            SQuestDatas questDatas = new SQuestDatas();
            if (data != null)
            {
                questDatas.ResetData(data);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                DeckManager.Inst.deckLists[i].unitDatas[j] = baseUnitDatas[j];
            }
            DeckManager.Inst.deckLists[i].sephiraData = baseSephiraData;
            DeckManager.Inst.deckLists[i].fieldData = baseFieldData;
        }


        for (int i = 0; i < DeckManager.Inst.curDeckUnitDatas.Count; i++)
        {
            DeckManager.Inst.curDeckUnitDatas[i] = baseUnitDatas[i];
        }

        DeckManager.Inst.curDeckSephiraData = baseSephiraData;
        DeckManager.Inst.curDeckFieldData = baseFieldData;


        EquipmentManager.Inst.GenerateAllUnitAndFieldLists();
        DeckManager.Inst.equipmentdeckToggle.InitIndex(DeckManager.Inst.deckListIndex);

        DeckManager.Inst.lobbyDeckToggle.ToggleChangeDeckList();
        DeckManager.Inst.lobbyDeckToggle.InitIndex(DeckManager.Inst.deckListIndex);

        EquipmentManager.Inst.RefreshUnitContentSize(EquipmentManager.Inst.tabContentRect, unitDatas);

        DeckManager.Inst.InitializeAllDeckLists();

        deck.factionBonusCalculator.
        SetFactionBonusInfo(deck.deckLists[deck.deckListIndex].unitDatas, deck.deckLists[deck.deckListIndex].sephiraData, FactionBonusCalculator.FactionBonusView.Normal);

        ShopManager.Inst.GenerateShopItems();

        DifficultyManager.Inst.InitializeDifficultyToggles();
        DifficultyManager.Inst.UpdateBackgroundByDifficulty();
        GameModeManager.Inst.jointOperationsManager.InitContentUnlockConditionCount();
        GameModeManager.Inst.jointOperationsManager.curJointOperationDifficulty = JointOperationsManager.JointOperationDifficulty.Normal;
        for (int i = 0; i < ShopManager.Inst.shopItems.Length; i++)
        {
            SShopItemDatas shopItemDatas = new SShopItemDatas();
            shopItemDatas.ResetInit(ShopManager.Inst.shopItems[i]);
            playerData.shopData.shopItemDatas.Add(shopItemDatas);

            if (i != 0)
            {
                if (playerData.shopData.shopItemDatas[i].itemType == ShopItem.ItemType.unit)
                {
                    playerData.shopData.curShopUnitsID.Add(shopItemDatas.unitID);
                }
                else
                {
                    playerData.shopData.curShopResourcesID.Add(shopItemDatas.itemID);
                }
            }
        }

        GameModeManager.Inst.curGameMode = GameManager.GameMode.None;

        ShopManager.Inst.refreshCount = 0;

        ShopManager.Inst.RefreshItemList();

        QuestManager.Inst.ResetQuestProgress();

        IsInitialized = true;
        SaveData();

    }

    public void RefreshDeckList()
    {
        DeckManager deck = DeckManager.Inst;

        Debug.Log("<color=Red>[Data Missing, Refresh]</color>");

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                DeckManager.Inst.deckLists[i].unitDatas[j] = baseUnitDatas[j];
            }
            DeckManager.Inst.deckLists[i].sephiraData = baseSephiraData;
        }

        for (int i = 0; i < DeckManager.Inst.curDeckUnitDatas.Count; i++)
        {
            DeckManager.Inst.curDeckUnitDatas[i] = baseUnitDatas[i];
        }
        DeckManager.Inst.curDeckSephiraData = baseSephiraData;
        DeckManager.Inst.curDeckFieldData = baseFieldData;

        DeckManager.Inst.equipmentdeckToggle.GetComponent<DeckToggle>().ToggleChangeDeckList();
        DeckManager.Inst.equipmentdeckToggle.GetComponent<DeckToggle>().InitIndex(DeckManager.Inst.deckListIndex);
        DeckManager.Inst.lobbyDeckToggle.ToggleChangeDeckList();
        DeckManager.Inst.lobbyDeckToggle.InitIndex(DeckManager.Inst.deckListIndex);

        EquipmentManager.Inst.UpdateAllEquippedStatus();

        deck.factionBonusCalculator.
        SetFactionBonusInfo(deck.deckLists[deck.deckListIndex].unitDatas, deck.deckLists[deck.deckListIndex].sephiraData, FactionBonusCalculator.FactionBonusView.Normal);

    }

    public UnitData GetUnitDataById(int id)
    {
        List<UnitData> allUnitDatas = new List<UnitData>();
        allUnitDatas.AddRange(unitDatas);
        allUnitDatas.AddRange(sephiraUnitDatas);

        for (int i = 0; i < allUnitDatas.Count; i++)
        {
            if (id == allUnitDatas[i].unitID)
            {
                return allUnitDatas[i];
            }
        }
        return null;
    }

    public FieldData GetFieldDataById(int id)
    {
        for (int i = 0; i < fieldDatas.Count; i++)
        {
            if (id == fieldDatas[i].fieldID)
            {
                return fieldDatas[i];
            }
        }
        return null;
    }

    public ShopResourcesData GetResourcesDataById(int id)
    {
        for (int i = 0; i < shopResourcesDatas.Count; i++)
        {
            if (id == shopResourcesDatas[i].id)
            {
                return shopResourcesDatas[i];
            }
        }
        return null;
    }

    public JointOperationData GetJointOperationDataById(int id)
    {
        for (int i = 0; i < jointOperationBossDatas.Count; i++)
        {
            if (id == jointOperationBossDatas[i].operationId)
            {
                return jointOperationBossDatas[i];
            }
        }
        return null;
    }

    public DifficultyData GetDifficultyDataById(int id)
    {
        for (int i = 0; i < DifficultyManager.Inst.difficultyData.Length; i++)
        {
            if (id == DifficultyManager.Inst.difficultyData[i].difficultyId)
            {
                return DifficultyManager.Inst.difficultyData[i];
            }
        }
        return null;
    }

}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public int levelUpAmount;
    public int curLevelExp;
    public int maxLevelExp;
    public int gold;
    public int gem;
    public int voidTicket;
    public int deckListIndex;
    public bool isTutorial;
    public bool isLevelUp;
    public bool isJointOperationUnlocked;

    public bool isRestarting;

    public int questPoint;
    public bool[] isQuestRewardClaimed;

    public DifficultyData.DifficultyType difficultyType;
    public JointOperationsManager.JointOperationDifficulty curJointOperationDifficulty;
    public List<SUnitDatas> sUnitDatas = new List<SUnitDatas>();
    public List<SUnitDatas> sSephiraDatas = new List<SUnitDatas>();
    public List<SFieldDatas> sFieldDatas = new List<SFieldDatas>();
    public List<SQuestDatas> sQuestDatas = new List<SQuestDatas>();
    public List<DeckListData> deckList = new List<DeckListData>();
    public CurrentDeckUnit currentDeckUnit = new CurrentDeckUnit();
    public SJointOperationDeck sJointOperationDeck = new SJointOperationDeck();

    public ShopData shopData = new ShopData();

    public int curJointOperatioId;
    public int curDifficultyId;

    public GameManager.GameMode curGameMode;

    public string lastLoginTime;
}

[System.Serializable]
public class DeckListData
{
    public List<int> unitDataIDList = new List<int>();
    public int sephiraDataID;
    public int fieldDataID;
}

[System.Serializable]
public class CurrentDeckUnit
{
    public List<int> unitDataIDList = new List<int>();
    public int sephiraDataID;
    public int fieldDataID;
}

[System.Serializable]
public class ShopData
{
    public List<SShopItemDatas> shopItemDatas = new List<SShopItemDatas>();
    public List<int> curShopUnitsID = new List<int>();
    public List<int> curShopResourcesID = new List<int>();
    public int refreshCount;
}

[System.Serializable]
public class SUnitDatas
{
    public enum DataType { save, load, reset }
    public DataType dataType;

    // public UnitData unitData;
    public int unitID;
    public string unitName;
    public double damageAmout;
    public float skillAmount;
    public float maxHealth;
    public float maxShield;
    public float defenseAmount;
    public float criticalPer;
    public float criticalDamage;
    public float attackSpeed;
    public float range;
    public float coolTime;
    public int cardLevel;
    public int curLevelExp;
    public int maxLevelExp;
    public bool isUnlock;
    public bool isEquipped;

    public void SyncUnitState(UnitData unitData, DataType dataType)
    {
        switch (dataType)
        {
            case DataType.save:
                SaveUnitData(unitData);
                break;
            case DataType.load:
                LoadUnitData(unitData);
                break;
            case DataType.reset:
                ResetUnitData(unitData);
                break;
        }
    }

    void SaveUnitData(UnitData unitData)
    {
        unitID = unitData.unitID;
        unitName = unitData.unitName;
        damageAmout = unitData.damageAmount;
        skillAmount = unitData.skillAmount;
        maxHealth = unitData.maxHealth;
        maxShield = unitData.maxShield;
        defenseAmount = unitData.defenseAmount;
        criticalPer = unitData.criticalPer;
        criticalDamage = unitData.criticalDamage;
        attackSpeed = unitData.attackSpeed;
        range = unitData.range;
        coolTime = unitData.coolTime;

        cardLevel = unitData.cardLevel;
        curLevelExp = unitData.curLevelExp;
        maxLevelExp = unitData.maxLevelExp;

        isUnlock = unitData.isUnlock;
        isEquipped = unitData.isEquipped;

        unitData.InitUnitState();
    }

    void LoadUnitData(UnitData unitData)
    {
        unitData.unitID = unitID;
        unitData.unitName = unitName;
        unitData.damageAmount = unitData.baseDamage;
        unitData.skillAmount = unitData.baseSkillAmount;
        unitData.maxHealth = unitData.baseHealth;
        unitData.maxShield = unitData.baseShield;
        unitData.defenseAmount = unitData.baseDefence;
        unitData.criticalPer = unitData.baseCriticalPer;
        unitData.criticalDamage = unitData.baseCriticalDamage;
        unitData.attackSpeed = unitData.baseAttackSpeed;
        unitData.range = unitData.GetRange();
        unitData.coolTime = unitData.baseCoolTime;

        unitData.cardLevel = cardLevel;
        unitData.curLevelExp = curLevelExp;
        unitData.maxLevelExp = maxLevelExp;

        unitData.isUnlock = isUnlock;
        unitData.isEquipped = isEquipped;

        unitData.InitUnitState();
    }

    void ResetUnitData(UnitData unitData)
    {
        unitData.damageAmount = unitData.baseDamage;
        unitData.skillAmount = unitData.baseSkillAmount;
        unitData.maxHealth = unitData.baseHealth;
        unitData.maxShield = unitData.baseShield;
        unitData.defenseAmount = unitData.baseDefence;
        unitData.criticalPer = unitData.baseCriticalPer;
        unitData.criticalDamage = unitData.baseCriticalDamage;
        unitData.attackSpeed = unitData.baseAttackSpeed;
        unitData.range = unitData.GetRange();
        unitData.coolTime = unitData.baseCoolTime;

        unitData.cardLevel = unitData.baseCardLevel;
        unitData.curLevelExp = unitData.baseCurLevelExp;
        unitData.maxLevelExp = unitData.baseMaxLevelExp;

        unitData.isUnlock = unitData.baseIsUnlock;
        unitData.isEquipped = unitData.baseIsEquipped;

        unitData.InitUnitState();
    }
}

[System.Serializable]
public class SFieldDatas
{
    public int fieldID;
    public string fieldName;

    public int fieldLevel;

    public float value;
    public bool isUnlock;
    public bool isEquipped;

    public void SaveInit(FieldData fieldData)
    {
        fieldID = fieldData.fieldID;
        value = fieldData.value;
        fieldName = fieldData.fieldName;
        fieldLevel = fieldData.fieldLevel;
        isUnlock = fieldData.isUnlock;
        isEquipped = fieldData.isEquipped;
    }

    public void LoadInit(FieldData fieldData)
    {
        fieldData.InitFieldState();
        fieldData.fieldID = fieldID;
        fieldData.value = value;
        fieldData.fieldName = fieldName;
        fieldData.fieldLevel = fieldLevel;
        fieldData.isUnlock = isUnlock;
        fieldData.isEquipped = isEquipped;
    }

    public void ResetData(FieldData fieldData)
    {
        fieldData.value = fieldData.baseValue;
        fieldData.fieldLevel = fieldData.baseLevel;
        fieldData.isUnlock = fieldData.baseIsUnlock;
        fieldData.isEquipped = fieldData.baseIsEquipped;
    }
}

[System.Serializable]
public class SShopItemDatas
{
    public ShopItem.ItemType itemType;

    public int unitID;
    public int itemID;

    public bool isPurchased;

    public void SaveInit(ShopItem shopItem)
    {
        itemType = shopItem.itemType;
        switch (itemType)
        {
            case ShopItem.ItemType.unit:
                unitID = shopItem.unitData.unitID;
                break;
            case ShopItem.ItemType.gold:
            case ShopItem.ItemType.gem:
            case ShopItem.ItemType.voidTicket:
                itemID = shopItem.shopResourcesData.id;
                break;
        }

        isPurchased = shopItem.isPurchased;

    }

    public void LoadInit(ShopItem shopItem)
    {
        shopItem.itemType = itemType;

        switch (itemType)
        {
            case ShopItem.ItemType.unit:
                shopItem.unitData = DataManager.Inst.GetUnitDataById(unitID);
                shopItem.unitData.unitID = unitID;
                break;
            case ShopItem.ItemType.gold:
            case ShopItem.ItemType.gem:
            case ShopItem.ItemType.voidTicket:
                shopItem.shopResourcesData = DataManager.Inst.GetResourcesDataById(itemID);
                shopItem.shopResourcesData.id = itemID;
                break;
        }

        shopItem.isPurchased = isPurchased;
    }

    public void ResetInit(ShopItem shopItem)
    {
        shopItem.isPurchased = false;
    }
}

[System.Serializable]
public class SQuestDatas
{
    public string questName;
    public int questID;
    public int progress;
    public bool isClear;

    public void SaveInit(QuestData questData)
    {
        questName = questData.questName;
        questID = questData.questID;
        progress = questData.progress;
        isClear = questData.isClear;
    }

    public void LoadInit(QuestData questData)
    {
        questData.questName = questName;
        questData.questID = questID;
        questData.progress = progress;
        questData.isClear = isClear;
    }

    public void ResetData(QuestData questData)
    {
        questData.progress = 0;
        questData.isClear = false;
    }
}

[System.Serializable]
public class SJointOperationDeck
{
    public List<int> unitDataIDList = new List<int>();
    public int sephiraDataID;
    public int fieldDataID;
}