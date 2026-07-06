using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UpgradeList : MonoBehaviour
{
    public int id;
    public int upgradeCost;
    public int level;
    public int maxLevel;
    public UpgradeData upgradeData;
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [Space]
    [SerializeField] Image costIconImage;
    public TMP_Text costText;
    public TMP_Text levelText;



    public void Init(UpgradeData data)
    {
        upgradeData = data;
        id = upgradeData.id;
        upgradeCost = upgradeData.upgradeCost;
        maxLevel = upgradeData.maxLevel;
        iconImage.sprite = upgradeData.icon;
        nameText.text = upgradeData.upgradeName;

        costIconImage.sprite = upgradeData.resourceData.icon;
        costText.text = upgradeData.upgradeCost.ToString();
        levelText.text = "Lv." + upgradeData.level;
    }

}
