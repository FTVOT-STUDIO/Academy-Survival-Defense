using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    public ResourcesData.ResourcesType resourcesType;
    [SerializeField] UnitData unitData;
    [SerializeField] ResourcesData[] resourcesDatas;

    [SerializeField] TMP_Text itemValue;
    [SerializeField] Image itemPortrait;
    [SerializeField] Image itemBackground;
    [SerializeField] Slider upgradeSlider;
    [SerializeField] TMP_Text upgradeSliderText;
    [SerializeField] GameObject upgradeIcon;
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;

    public void Init(ResourcesData.ResourcesType type, int value)
    {
        InitInternal(type, value, null);
    }

    public void Init(UnitData data, int value)
    {
        InitInternal(ResourcesData.ResourcesType.none, value, data);
    }

    private void InitInternal(ResourcesData.ResourcesType type, int value, UnitData data)
    {
        resourcesType = type;
        unitData = data;

        itemValue.text = "x" + value;

        bool isUnit = data != null;
        upgradeSlider.gameObject.SetActive(isUnit);

        if (isUnit)
        {
            itemPortrait.sprite = data.portrait;
            itemBackground.color = data.classColorType[data.unitClass];
            RectTransform portraitRect = itemPortrait.GetComponent<RectTransform>();
            portraitRect.sizeDelta = new Vector2(portraitWidth * data.portraitScaleFactor, portraitHeight * data.portraitScaleFactor);
            portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * data.portraitPosXFactor, (1 + portraitPosY) * data.portraitPosYFactor);
            upgradeIcon.SetActive(data.curLevelExp >= data.maxLevelExp);
            upgradeSliderText.text = $"{data.curLevelExp}/{data.maxLevelExp}";
            upgradeSlider.value = (float)data.curLevelExp / data.maxLevelExp;
        }
        else
        {
            var resource = resourcesDatas.FirstOrDefault(r => r.resourcesType == type);
            itemPortrait.sprite = resource.icon;
            itemBackground.color = new Color(0.3f, 0.25f, 0.4f);
            upgradeIcon.SetActive(false);
            upgradeSliderText.text = "";
            upgradeSlider.value = 0;
        }
    }
}
