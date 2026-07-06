using UnityEngine;

public class JointOperationUnitSlot : MonoBehaviour
{
    public UnitList unitList;
    [HideInInspector] public JointOperationUnitList jointOperationUnitList;

    [SerializeField] int slotIndex;

    public void Init()
    {
        unitList.Init(DeckManager.Inst.jointOperationDeckUnitDatas[slotIndex]);
    }

    public void ChangeSlotIndex()
    {
        DeckManager.Inst.jointOperationsDeckManager.curUnitSlotIndex = slotIndex;
    }
}
