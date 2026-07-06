using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesData", menuName = "ShopResourcesItem/ResourcesData")]
public class ShopResourcesData : ScriptableObject
{
    public enum CurrencyType { gold, gem }
    public ResourcesData.ResourcesType itemType;
    public CurrencyType currencyType;

    public bool isFree = false;
    public Sprite portrait;
    public string itemName;
    public int id;
    public int itemCount;
    public int price;
    [Space]
    public ResourcesData resourcesData;
    public ResourcesData costResourceData;

}
