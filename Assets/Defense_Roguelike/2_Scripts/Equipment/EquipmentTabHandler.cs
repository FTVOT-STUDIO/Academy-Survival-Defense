using UnityEngine;
using UnityEngine.UI;

public class EquipmentTabHandler : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] Toggle[] toggles;
    void OnEnable()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                equipmentManager.OnClickResizeContent(i);
                break;
            }
        }
    }

}
