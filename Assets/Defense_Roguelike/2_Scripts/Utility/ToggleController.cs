using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] GameObject[] graphics;

    void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleChanged);
        OnToggleChanged(toggle.isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].SetActive(isOn);
        }
    }
}
