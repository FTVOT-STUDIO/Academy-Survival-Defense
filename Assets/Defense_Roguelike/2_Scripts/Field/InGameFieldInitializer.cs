using UnityEngine;

public class InGameFieldInitializer : MonoBehaviour
{
    public FieldSlot[] fieldSlots;
    public Transform sephiraSlot;
    public SpriteRenderer backField;

    public void InitializeAllSlotMarkers()
    {
        for (int i = 0; i < fieldSlots.Length; i++)
        {
            fieldSlots[i].InitMarker();
        }
    }
}
