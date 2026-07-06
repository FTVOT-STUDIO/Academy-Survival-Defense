using UnityEngine;

public class JointOperationUnitList : MonoBehaviour
{
    public UnitList unitList;
    private bool isEquipment = false;
    [SerializeField] GameObject equipmentImage;

    private void Awake()
    {
        unitList = GetComponent<UnitList>();
    }

    public void ChangeUnit()
    {
        if (isEquipment)
            return;
        DeckManager deck = DeckManager.Inst;

        DeckManager deckManager = DeckManager.Inst;
        deckManager.jointOperationDeckUnitDatas[deckManager.jointOperationsDeckManager.curUnitSlotIndex] = unitList.unitData;

        deckManager.jointOperationsDeckManager.unitSlots[deckManager.jointOperationsDeckManager.curUnitSlotIndex].jointOperationUnitList.ActiveEquipmentImage(false);

        deckManager.jointOperationsDeckManager.unitSlots[deckManager.jointOperationsDeckManager.curUnitSlotIndex].Init();
        deckManager.jointOperationsDeckManager.unitSlots[deckManager.jointOperationsDeckManager.curUnitSlotIndex].jointOperationUnitList = this;

        ActiveEquipmentImage(true);

        deck.factionBonusCalculator.
        SetFactionBonusInfo(deck.jointOperationDeckUnitDatas.ToArray(), deck.jointOperationDeckSephiraData, FactionBonusCalculator.FactionBonusView.JointOpertaion);
    }

    public void ActiveEquipmentImage(bool isActive)
    {
        isEquipment = isActive;
        equipmentImage.SetActive(isActive);
    }
}
