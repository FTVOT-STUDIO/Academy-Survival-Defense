using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopResourcesItem : MonoBehaviour
{
    [HideInInspector] public ShopResourcesData shopResourcesData;
    public Image portrait;
    public Image currencyImage;
    public TMP_Text itemNameText;
    public TMP_Text itemCountText;
    public TMP_Text priceText;

    public void Init()
    {
        portrait.sprite = shopResourcesData.portrait;
        itemNameText.text = shopResourcesData.itemName;
        itemCountText.text = shopResourcesData.itemCount.ToString();
        priceText.text = shopResourcesData.price.ToString();

        switch (shopResourcesData.currencyType)
        {
            case ShopResourcesData.CurrencyType.gold:
                currencyImage.sprite = shopResourcesData.costResourceData.icon;
                break;
            case ShopResourcesData.CurrencyType.gem:
                currencyImage.sprite = shopResourcesData.costResourceData.icon;
                break;
        }
    }
}
