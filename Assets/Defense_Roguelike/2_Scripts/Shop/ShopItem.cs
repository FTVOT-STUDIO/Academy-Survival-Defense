using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum ItemType { gold, gem, voidTicket, unit }
    public ItemType itemType;

    [HideInInspector] public UnitData unitData;
    [HideInInspector] public ShopResourcesData shopResourcesData;

    [Header("# Basic Item Info")]
    public TMP_Text itemName;
    public int itemID;
    public Image background;
    public Image portrait;
    public TMP_Text classText;
    public Image classBackground;
    public GameObject purchaseBlocker;
    public bool isPurchased;
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;
    [Header("# Level Up Info")]
    public Slider nextLevelUpSlider;
    public Image nextLevelUpSliderFill;
    public TMP_Text levelText;
    public TMP_Text nextLevelUpSliderText;
    public GameObject upgradeIcon;

    [Header("# Cost and Item Info")]
    [HideInInspector] public int price;
    [HideInInspector] public int itemGetCount;
    public GameObject freePriceText;
    public GameObject newUnitGetItemText;
    public Image priceImgae;
    public TMP_Text priceText;
    public TMP_Text ItemCountText;

    public void Init(ItemType type)
    {
        itemType = type;

        purchaseBlocker.SetActive(isPurchased);
        GetComponent<Button>().interactable = !isPurchased;

        switch (type)
        {
            case ItemType.gold:
            case ItemType.gem:
            case ItemType.voidTicket:
                if (shopResourcesData.isFree)
                {
                    itemGetCount = shopResourcesData.itemCount * PlayerManager.Inst.level;
                }
                else
                {
                    itemGetCount = shopResourcesData.itemCount;

                    priceImgae.sprite = shopResourcesData.costResourceData.icon;
                    price = shopResourcesData.price;
                    priceText.text = price.ToString();

                    if (shopResourcesData.currencyType == ShopResourcesData.CurrencyType.gold)
                        priceText.color = PlayerManager.Inst.gold < price ? Color.red : Color.white;
                    else
                        priceText.color = PlayerManager.Inst.gem < price ? Color.red : Color.white;
                }

                background.color = new Color(0, 1, 0.9f);
                classText.text = "재화";
                classBackground.color = Color.grey;

                nextLevelUpSlider.gameObject.SetActive(false);
                levelText.gameObject.SetActive(false);
                newUnitGetItemText.SetActive(false);

                freePriceText.SetActive(shopResourcesData.isFree);
                priceImgae.gameObject.SetActive(!shopResourcesData.isFree);

                itemName.text = shopResourcesData.resourcesData.resourceName;
                portrait.sprite = shopResourcesData.resourcesData.icon;

                ItemCountText.text = "X" + itemGetCount.ToString();
                break;
            case ItemType.unit:
                itemName.text = unitData.unitName;
                background.color = new Color(0.7f, 0.7f, 1f);
                portrait.sprite = unitData.portrait;
                RectTransform portraitRect = portrait.GetComponent<RectTransform>();
                portraitRect.sizeDelta = new Vector2(portraitWidth * unitData.portraitScaleFactor, portraitHeight * unitData.portraitScaleFactor);
                portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * unitData.portraitPosXFactor, (1 + portraitPosY) * unitData.portraitPosYFactor);

                levelText.text = "Lv." + unitData.cardLevel;
                classText.text = unitData.classToKorean[unitData.unitClass];
                classBackground.color = unitData.classColorType[unitData.unitClass];

                nextLevelUpSlider.gameObject.SetActive(true);

                if (unitData.cardLevel == 15)
                {
                    nextLevelUpSlider.value = 1;
                    nextLevelUpSliderFill.color = Color.red;
                    nextLevelUpSliderText.text = "Max";
                    upgradeIcon.SetActive(false);
                }
                else
                {
                    nextLevelUpSlider.value = (float)unitData.curLevelExp / unitData.maxLevelExp;
                    nextLevelUpSliderFill.color = new Color(0.3f, 1, 0.5f);
                    nextLevelUpSliderText.text = unitData.curLevelExp + "/" + unitData.maxLevelExp;
                    upgradeIcon.SetActive(unitData.curLevelExp >= unitData.maxLevelExp);
                }
                freePriceText.SetActive(false);
                newUnitGetItemText.SetActive(!unitData.isUnlock);

                priceImgae.gameObject.SetActive(true);
                priceImgae.sprite = unitData.buyResourcesData.icon;
                (itemGetCount, price) = unitData.unitItemValues[unitData.unitClass];
                itemGetCount *= PlayerManager.Inst.level;

                priceText.text = price.ToString();

                ItemCountText.text = "X" + itemGetCount;
                if (unitData.buyResourcesData.resourcesType == ResourcesData.ResourcesType.gold)
                    priceText.color = PlayerManager.Inst.gold < price ? Color.red : Color.white;
                else
                    priceText.color = PlayerManager.Inst.gem < price ? Color.red : Color.white;

                break;
        }
    }

}

