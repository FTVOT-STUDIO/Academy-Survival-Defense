using UnityEngine;

public class JointOperationFieldList : MonoBehaviour
{
    public FieldList fieldList;
    private bool isEquipment = false;
    [SerializeField] GameObject equipmentImage;

    private void Awake()
    {
        fieldList = GetComponent<FieldList>();
    }

    public void ChangeField()
    {
        if (isEquipment)
            return;

        DeckManager deckManager = DeckManager.Inst;
        deckManager.jointOperationDeckFieldData = fieldList.fieldData;

        deckManager.jointOperationsDeckManager.fieldSlot.jointOperationFieldList.ActiveEquipmentImage(false);

        deckManager.jointOperationsDeckManager.fieldSlot.Init();
        deckManager.jointOperationsDeckManager.fieldSlot.jointOperationFieldList = this;

        ActiveEquipmentImage(true);
    }

    public void ActiveEquipmentImage(bool isActive)
    {
        isEquipment = isActive;
        equipmentImage.SetActive(isActive);
    }
}
