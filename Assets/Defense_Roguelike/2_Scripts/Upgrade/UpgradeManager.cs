using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] UpgradeData[] upgradeDatas;
    [SerializeField] GameObject upgradeListPrefab;

    [SerializeField] Transform contentParent;
    [SerializeField] List<UpgradeList> upgradeLists = new List<UpgradeList>();

    void Awake()
    {
        for (int i = 0; i < upgradeDatas.Length; i++)
        {
            int index = i;
            GameObject upgradeList = Instantiate(upgradeListPrefab, contentParent);
            upgradeList.GetComponent<UpgradeList>().Init(upgradeDatas[index]);
            upgradeList.GetComponent<Button>().onClick.AddListener(() => ApplyUpgrade(upgradeList.GetComponent<UpgradeList>()));
            upgradeLists.Add(upgradeList.GetComponent<UpgradeList>());
        }

        upgradeLists = upgradeLists.OrderBy(u => u.id).ToList();

        for (int i = 0; i < upgradeLists.Count; i++)
        {
            upgradeLists[i].transform.SetSiblingIndex(i);
        }
    }

    public void ApplyUpgrade(UpgradeList list)
    {
        if (list.level < list.maxLevel)
        {
            bool canUpgrade = false;

            switch (list.upgradeData.resourceData.resourcesType)
            {
                case ResourcesData.ResourcesType.coin:
                    if (GameManager.Inst.coin >= list.upgradeCost)
                    {
                        GameManager.Inst.ModifyCoin(-list.upgradeCost);
                        list.upgradeCost += 100;
                        canUpgrade = true;
                    }
                    break;
                case ResourcesData.ResourcesType.darkMatteCoin:
                    if (GameManager.Inst.darkMatteCoin >= list.upgradeCost)
                    {
                        GameManager.Inst.ModifyDarkMatteCoin(-list.upgradeCost);
                        list.upgradeCost += 1;
                        canUpgrade = true;
                    }
                    break;
            }

            if (canUpgrade)
            {
                list.level++;
                list.upgradeData.DefineUpgradeStat();
                list.costText.text = list.level == list.maxLevel ? "최대레벨" : list.upgradeCost.ToString();
                list.costText.color = list.level == list.maxLevel ? new Color(0, 1, 0.8f) : Color.white;
                list.levelText.text = "Lv." + list.level;
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.InGameUpgrade);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.select4);
            }

        }
    }
}
