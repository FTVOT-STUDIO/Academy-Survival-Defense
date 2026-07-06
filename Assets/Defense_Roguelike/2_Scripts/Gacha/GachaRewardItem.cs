using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GachaRewardItem : MonoBehaviour
{
    public enum ItemType { gold, gem, unit }
    [HideInInspector] public UnitData unitdata;
    [HideInInspector] public ResourcesData resourcesData;

    public Image BackGround;
    public Image itemPortrait;
    public TMP_Text itemName;
    public TMP_Text itemClass;
    public TMP_Text itemValue;

    [Space]
    public Slider unitSlider;
    public GameObject upgradeIcon;
    public GameObject newGetUnitTextObj;
    public TMP_Text unitLevel;
    public TMP_Text unitLevelSldierText;
    public Image levelUpSliderFill;
    [Space]
    public Slider goodsSlider;
    public TMP_Text goodsValue;
    public Image goodsSliderPortrait;
    [Space]
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;

    public void Init(ItemType itemType)
    {
        goodsSlider.gameObject.SetActive(false);
        unitSlider.gameObject.SetActive(false);
        newGetUnitTextObj.SetActive(false);

        // RectTransform rect = itemPortrait.GetComponent<RectTransform>();
        // rect.sizeDelta = new Vector2(430f, 430f);
        RectTransform portraitRect = itemPortrait.GetComponent<RectTransform>();

        switch (itemType)
        {
            case ItemType.gold:
            case ItemType.gem:

                portraitRect.sizeDelta = new Vector2(portraitWidth, portraitHeight);
                portraitRect.anchoredPosition = new Vector2(portraitPosX, portraitPosY);

                BackGround.color = Color.gray;

                itemPortrait.sprite = resourcesData.icon;
                goodsSliderPortrait.sprite = resourcesData.icon;

                itemName.text = resourcesData.resourceName;
                itemClass.text = "재화";

                goodsValue.text = string.Format("{0:n0}", PlayerManager.Inst.gold);

                goodsSlider.gameObject.SetActive(true);
                break;
            case ItemType.unit:
                BackGround.color = unitdata.classColorType[unitdata.unitClass];

                itemPortrait.sprite = unitdata.portrait;

                portraitRect.sizeDelta = new Vector2(portraitWidth * unitdata.portraitScaleFactor, portraitHeight * unitdata.portraitScaleFactor);
                portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * unitdata.portraitPosXFactor, (1 + portraitPosY) * unitdata.portraitPosYFactor);

                itemName.text = unitdata.unitName;
                itemClass.text = unitdata.classToKorean[unitdata.unitClass];
                unitLevel.text = unitdata.cardLevel.ToString();

                if (GachaManager.Inst.isSkip)
                    GachaManager.Inst.unitRewardIndex--;

                int index = 0;
                for (int i = GachaManager.Inst.rewardList.Count - GachaManager.Inst.unitRewardIndex; i < GachaManager.Inst.rewardList.Count; i++)
                {
                    if (GachaManager.Inst.rewardList[i] == unitdata)
                    {
                        index++;
                    }
                }

                int curExp = unitdata.curLevelExp - GachaManager.Inst.unitExpList[GachaManager.Inst.rewardList.Count - GachaManager.Inst.unitRewardIndex - 1] * index;
                unitLevelSldierText.text = unitdata.cardLevel < 15 ? unitLevelSldierText.text = curExp + "/" + unitdata.maxLevelExp : "MAX";
                upgradeIcon.SetActive(!(unitdata.cardLevel == 15 || unitdata.curLevelExp < unitdata.maxLevelExp));

                levelUpSliderFill.color = unitdata.cardLevel < 15 ? new Color(0.3f, 1, 0.5f) : Color.red;

                unitSlider.value = unitdata.cardLevel < 15 ? (float)unitdata.curLevelExp / unitdata.maxLevelExp : 1;

                foreach (UnitData datas in GachaManager.Inst.unlockNewUnitList)
                {
                    if (datas == unitdata)
                    {
                        newGetUnitTextObj.SetActive(true);
                        break;
                    }
                }

                unitSlider.gameObject.SetActive(true);
                break;
        }
    }

}
