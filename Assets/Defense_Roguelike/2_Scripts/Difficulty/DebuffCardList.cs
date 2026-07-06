using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuffCardList : MonoBehaviour
{
    [SerializeField] TMP_Text dubuffNameText;
    [SerializeField] TMP_Text dubuffDescText;
    [SerializeField] Image debuffIconImage;

    public void Init(HardModeDebuffData hardModeDebuffData)
    {
        dubuffNameText.text = hardModeDebuffData.debuffName;
        dubuffDescText.text = hardModeDebuffData.debuffDesc;

        debuffIconImage.sprite = hardModeDebuffData.icon;
    }

}
