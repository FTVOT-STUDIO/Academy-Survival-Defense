using TMPro;
using UnityEngine;

public class UnitSkillDescriptionTooltip : MonoBehaviour
{
    [SerializeField] TMP_Text skillNameText;
    [SerializeField] TMP_Text skillDescText;
    [SerializeField] TMP_Text skillTypeText;

    public void Init(string name, string desc, string type)
    {
        skillNameText.text = name;
        skillDescText.text = desc;
        skillTypeText.text = type;
    }
}
