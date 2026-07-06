using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VInspector;

public class DeckManager : MonoBehaviour
{
    public enum DeckType { Normal, Arcade, JointOpertaion }
    public static DeckManager Inst;
    [System.Serializable]
    public class CurrentInGameDeck
    {
        public List<UnitData> unitdatas;
        public UnitData sephiraUnit;
        public FieldData fieldData;
    }

    [Tab("Deck List")]
    [Header("# Deck List")]
    public int deckListIndex;
    public DeckList[] deckLists;
    [Tab("Current Deck")]
    [Header("# Current Deck")]
    public List<UnitData> curDeckUnitDatas;
    public UnitData curDeckSephiraData;
    public FieldData curDeckFieldData;
    public List<Faction> curDeckFactionBonus = new List<Faction>();
    [Space]
    [Header("# Current Game Deck")]
    public List<UnitData> curGameDeckUnitDatas;
    public UnitData curGameDeckSephiraData;
    public FieldData curGameDeckFieldData;
    public List<Faction> curGameDeckFactionBonus = new List<Faction>();
    public CurrentInGameDeck currentInGameDeck;
    [Tab("Joint Operation")]
    [Header("# Joint Operation")]
    public JointOperationsDeckManager jointOperationsDeckManager;
    public UnitList[] jointOperationsUnitList;
    public UnitList jointOperationsSephiraList;
    public FieldList jointOperationsFieldList;
    public List<UnitData> jointOperationDeckUnitDatas;
    public UnitData jointOperationDeckSephiraData;
    public FieldData jointOperationDeckFieldData;
    public List<Faction> jointOpertaionDeckFactionBonus = new List<Faction>();
    [Tab("DeckList Group")]
    [Header("# DeckList Group")]
    public UnitList[] equipmentUnitLists;
    public UnitList equipmentSephiraLists;
    public FieldList equipmentFieldLists;
    public Transform selectDeckListGroup;
    [Space]
    public GameObject equipmentUnitDeckList;
    public GameObject equipmentSephiraDeckList;
    public GameObject equipmentFieldDeckList;
    public GameObject lobbyUnitDeckList;
    public GameObject lobbySephiraDeckList;
    public GameObject lobbyFieldDeckList;
    [Tab("Select State")]
    [Header("# Select State")]
    public bool isSelected;
    public UnitData selectData;
    public GameObject selectUI;
    public GameObject selectUnitParent;
    [Space]
    [Header("# Faction")]
    public FactionBonusCalculator factionBonusCalculator;
    public FactionBonusUIContoroller normalDeckFactionController;
    public FactionBonusUIContoroller jointOpertaionDeckFactionController;
    [Tab("DeckToggle Groups")]
    [Header("# DeckToggle Groups")]
    public DeckToggle equipmentdeckToggle;
    public DeckToggle lobbyDeckToggle;
    Action initializeAllDeckLists;

    void Awake()
    {
        Inst = this;

        deckLists = new DeckList[5];
        curDeckUnitDatas = new List<UnitData>();

        for (int i = 0; i < deckLists.Length; i++)
        {
            deckLists[i] = new DeckList();
            deckLists[i].unitDatas = new UnitData[5];

            curDeckUnitDatas.Add(null);
        }

        jointOperationDeckUnitDatas = new List<UnitData>();

        for (int i = 0; i < 8; i++)
        {
            jointOperationDeckUnitDatas.Add(null);
        }

        initializeAllDeckLists += InitializeEquipmentUnitDeckList;
        initializeAllDeckLists += InitializeEquipmentSephiraDeckList;
        initializeAllDeckLists += InitializeEquipmentFieldDeck;
    }

    public void InitializeAllDeckLists()
    {
        initializeAllDeckLists.Invoke();
        lobbyDeckToggle.InitDeckListState();
        equipmentdeckToggle.InitDeckListState();
    }

    public void InitializeEquipmentUnitDeckList()
    {
        UnitList[] unitLists = equipmentUnitDeckList.GetComponentsInChildren<UnitList>();

        for (int i = 0; i < unitLists.Length; i++)
        {
            unitLists[i].Init(deckLists[deckListIndex].unitDatas[unitLists[i].dataIndex]);
        }
    }


    public void InitializeEquipmentSephiraDeckList()
    {
        equipmentSephiraDeckList.GetComponentInChildren<UnitList>().Init(deckLists[deckListIndex].sephiraData);
    }

    public void InitializeEquipmentFieldDeck()
    {
        equipmentFieldDeckList.GetComponent<FieldList>().Init(curDeckFieldData);
    }

    public void InitializeJointOperationsDeck()
    {
        for (int i = 0; i < jointOperationsUnitList.Length; i++)
        {
            jointOperationsUnitList[i].Init(jointOperationDeckUnitDatas[i]);
        }
        jointOperationsSephiraList.Init(jointOperationDeckSephiraData);
        jointOperationsFieldList.Init(jointOperationDeckFieldData);
    }

    public void SetUpDeck(List<UnitData> unitDatas, UnitData sephiraData, FieldData fieldData)
    {
        currentInGameDeck.unitdatas = unitDatas;
        currentInGameDeck.sephiraUnit = sephiraData;
        currentInGameDeck.fieldData = fieldData;
    }

    public void GenerateTriggerUnitManager()
    {
        for (int i = 0; i < currentInGameDeck.unitdatas.Count; i++)
        {
            if (currentInGameDeck.unitdatas[i].unitSkill is IUnitManagerFactory unitManager)
            {
                unitManager.CreateUnitManager();
            }
        }

        if (currentInGameDeck.sephiraUnit.unitSkill is IUnitManagerFactory sephiraManager)
        {
            sephiraManager.CreateUnitManager();
        }
    }

    public void ApplyCurrentDeckToGameDeck(List<UnitData> unitDatas, UnitData sephiraData, FieldData fieldData, List<Faction> factionBonus)
    {
        curGameDeckUnitDatas = unitDatas;
        curGameDeckSephiraData = sephiraData;
        curGameDeckFieldData = fieldData;
        curGameDeckFactionBonus = factionBonus;
    }
}

[System.Serializable]
public class DeckList
{
    public UnitData[] unitDatas;
    public UnitData sephiraData;
    public FieldData fieldData;
}

