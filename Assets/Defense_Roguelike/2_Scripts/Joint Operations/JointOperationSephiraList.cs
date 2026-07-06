using UnityEngine;

public class JointOperationSephiraList : MonoBehaviour
{
    public UnitList unitList;
    private bool isEquipment = false;
    [SerializeField] GameObject equipmentImage;

    private void Awake()
    {
        unitList = GetComponent<UnitList>();
    }

    public void ChangeSephira()
    {
        if (isEquipment)
            return;
        DeckManager deck = DeckManager.Inst;
        DeckManager deckManager = DeckManager.Inst;
        deckManager.jointOperationDeckSephiraData = unitList.unitData;

        deckManager.jointOperationsDeckManager.sephiraSlot.jointOperationSephiraList.ActiveEquipmentImage(false);

        deckManager.jointOperationsDeckManager.sephiraSlot.Init();
        deckManager.jointOperationsDeckManager.sephiraSlot.jointOperationSephiraList = this; ;

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
