using UnityEngine;
using UnityEngine.Serialization;

public class TabManager : MonoBehaviour
{
    public GameObject[] tabs;
    public GameObject[] toggles;

    public void OnClickTabChange(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(index == i);
        }
    }
}
