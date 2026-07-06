using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitSkillDescriptionToggle : MonoBehaviour
{
    public SkillActivationType.ActivationType activationType;
    [SerializeField] UnitSkillDescriptionTooltip descriptionTooltip;
    [Space]
    [SerializeField] string skillName;
    [SerializeField] string skillDesc;
    [SerializeField] string skillType;
    [SerializeField] Image icon;
    public UnitData unitdata;
    [SerializeField] private Toggle toggle;

    void Awake()
    {
        toggle.onValueChanged.AddListener(InitializeDescriptionTooltip);
    }

    public void Init(UnitData data, int index)
    {
        unitdata = data;
        activationType = data.skillActivationInfo[index].skillActivationType.activationType;
        skillName = data.skillActivationInfo[index].skillName;
        skillDesc = data.ConvertHighlightText(data.skillActivationInfo[index].skillDesc);
        skillType = data.skillActivationInfo[index].skillActivationType.ActivationTypeToKorean[activationType];
        icon.sprite = data.skillActivationInfo[index].skillActivationType.icon;

        descriptionTooltip.Init(skillName, skillDesc, skillType);
    }

    private void InitializeDescriptionTooltip(bool isOn)
    {
        descriptionTooltip.Init(skillName, skillDesc, skillType);
    }

}
