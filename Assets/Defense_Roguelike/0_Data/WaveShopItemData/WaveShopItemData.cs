using UnityEngine;

[CreateAssetMenu(fileName = "WaveShopItemData", menuName = "WaveShopItemData/WaveShopItemData")]
public class WaveShopItemData : ScriptableObject
{
    public enum ItemType { };
    public ItemType itemType;
    [Space(20)]
    public string itemName;
    [TextArea]
    public string itmeDesc;
    [Space]
    public int itemPrice;
    public ResourcesData resourcesType;


}
