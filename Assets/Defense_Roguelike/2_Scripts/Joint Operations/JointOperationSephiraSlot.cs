using UnityEngine;

public class JointOperationSephiraSlot : MonoBehaviour
{
    public UnitList unitList;
    [HideInInspector] public JointOperationSephiraList jointOperationSephiraList;

    public void Init()
    {
        unitList.Init(DeckManager.Inst.jointOperationDeckSephiraData);
    }
}
