using UnityEngine;

public class JointOperationFieldSlot : MonoBehaviour
{
    public FieldList fieldList;
    [HideInInspector] public JointOperationFieldList jointOperationFieldList;

    public void Init()
    {
        fieldList.Init(DeckManager.Inst.jointOperationDeckFieldData);
    }
}
