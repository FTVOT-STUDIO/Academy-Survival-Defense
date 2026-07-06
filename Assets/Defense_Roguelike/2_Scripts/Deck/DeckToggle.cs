using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class DeckToggle : MonoBehaviour
{
    public enum DeckToggleType { Lobby, InGame }
    [SerializeField] DeckToggleType deckToggleType;

    public Toggle[] toggles;

    public GameObject[] unitLists;
    public GameObject sephiraList;
    public GameObject fieldList;

    void OnEnable()
    {
        if (deckToggleType == DeckToggleType.Lobby)
            StartCoroutine(DelayedInitToggle());
    }

    IEnumerator DelayedInitToggle()
    {
        yield return new WaitUntil(() => DataManager.Inst != null && DataManager.Inst.IsInitialized != false);
        ChangeDeckList(DeckManager.Inst.deckListIndex);
        InitDeckListState();

        toggles[DeckManager.Inst.deckListIndex].SetIsOnWithoutNotify(true);

    }

    public void InitIndex(int index)
    {
        toggles[index].SetIsOnWithoutNotify(true);
    }

    public void ChangeDeckList(int index)
    {
        for (int i = 0; i < DeckManager.Inst.curDeckUnitDatas.Count; i++)
        {
            DeckManager.Inst.curDeckUnitDatas[i].isEquipped = false;
            DeckManager.Inst.curDeckUnitDatas[i] = DeckManager.Inst.deckLists[index].unitDatas[i];
            DeckManager.Inst.curDeckUnitDatas[i].isEquipped = true;
        }

        DeckManager.Inst.curDeckSephiraData.isEquipped = false;
        DeckManager.Inst.curDeckSephiraData = DeckManager.Inst.deckLists[index].sephiraData;
        DeckManager.Inst.curDeckSephiraData.isEquipped = true;

        DeckManager.Inst.curDeckFieldData.isEquipped = false;
        DeckManager.Inst.curDeckFieldData = DeckManager.Inst.deckLists[index].fieldData;
        DeckManager.Inst.curDeckFieldData.isEquipped = true;

        DeckManager.Inst.deckListIndex = index;

        //InitIndex(index);

        ToggleChangeDeckList();
    }

    public void ToggleChangeDeckList()
    {
        DeckManager deck = DeckManager.Inst;

        EquipmentManager.Inst.UpdateAllEquippedStatus();
        deck.factionBonusCalculator.
        SetFactionBonusInfo(deck.deckLists[deck.deckListIndex].unitDatas, deck.deckLists[deck.deckListIndex].sephiraData, FactionBonusCalculator.FactionBonusView.Normal);
    }

    public void InitDeckListState()
    {
        for (int i = 0; i < unitLists.Length; i++)
        {
            UnitList unitList = unitLists[i].GetComponent<UnitList>();

            unitList.unitData = DeckManager.Inst.curDeckUnitDatas[i];
            unitList.Init(DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].unitDatas[unitList.dataIndex]);
        }

        if (sephiraList != null && DeckManager.Inst.curDeckSephiraData != null)
        {
            UnitList sephira = sephiraList.GetComponent<UnitList>();

            sephira.unitData = DeckManager.Inst.curDeckSephiraData;
            sephira.Init(DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].sephiraData);
        }

        if (fieldList != null && DeckManager.Inst.curDeckFieldData != null)
        {
            FieldList field = fieldList.GetComponent<FieldList>();

            field.fieldData = DeckManager.Inst.curDeckFieldData;
            field.Init(DeckManager.Inst.curDeckFieldData);
        }
    }
}

