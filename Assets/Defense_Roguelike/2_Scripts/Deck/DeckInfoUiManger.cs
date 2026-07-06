using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeckInfoUiManger : MonoBehaviour
{
    [SerializeField] Transform unitSlotParent;
    [SerializeField] GameObject sephiraSlot;
    [SerializeField] GridLayoutGroup unitInfoSlotGrid;

    public GameObject unitSlotPrefab;

    public UnitData sephiraData;
    public List<UnitData> unitDatas = new List<UnitData>();

    void Start()
    {
        GameManager.Inst.ToggleIntroDelay(true);
        sephiraData = DeckManager.Inst.currentInGameDeck.sephiraUnit;
        sephiraSlot.GetComponent<UnitIntroUI>().SetUp(sephiraData);
        unitDatas.Clear();

        unitDatas = DeckManager.Inst.currentInGameDeck.unitdatas;

        for (int i = 0; i < unitDatas.Count; i++)
        {
            GameObject slotObj = Instantiate(unitSlotPrefab, unitSlotParent);
            UnitIntroUI slot = slotObj.GetComponent<UnitIntroUI>();
            slot.SetUp(unitDatas[i]);
        }
        if (unitDatas.Count > 5)
            unitInfoSlotGrid.constraintCount = 4;
        else
            unitInfoSlotGrid.constraintCount = 5;
    }

    public void ReturnTransform()
    {
        Debug.Log("!!!!!!!!!!");
        for (int i = 0; i < unitDatas.Count; i++)
        {
            unitDatas[i].GetComponent<ObjectLifecycleManager>().DeActive();
        }
    }
}
