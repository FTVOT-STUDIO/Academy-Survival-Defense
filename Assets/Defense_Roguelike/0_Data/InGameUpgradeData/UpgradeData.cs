using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "UpgradeData/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public enum UpgradeType { damage, health, attackSpeed, range, criticalPer, criticalDamage, CardSpawnProbabilityUpgrade }
    [Header("# Upgrade Info")]
    public UpgradeType upgradeType;
    public int id;
    public Sprite icon;
    public string upgradeName;
    public float value;
    [Space]
    [Header("# Upgrade Cost & Lv")]
    public int upgradeCost;
    public int level = 0;
    public int maxLevel;
    [Space]
    [Header("# Upgrade Resource Type")]
    public ResourcesData resourceData;

    public void DefineUpgradeStat()
    {
        switch (upgradeType)
        {
            case UpgradeType.damage:
                GameStatManager.Inst.AddGameDamage(value);
                break;
            case UpgradeType.health:
                GameStatManager.Inst.AddGameMaxHealth(value);
                break;
            case UpgradeType.attackSpeed:
                GameStatManager.Inst.AddGameAttackSpeed(value);
                break;
            case UpgradeType.range:
                GameStatManager.Inst.AddGameRange(value);
                break;
            case UpgradeType.criticalPer:
                GameStatManager.Inst.AddGameCriticalPer(value);
                break;
            case UpgradeType.criticalDamage:
                GameStatManager.Inst.AddGameCriticalDamage(value);
                break;
            case UpgradeType.CardSpawnProbabilityUpgrade:
                GameFlowManager.Inst.ChangeCardSpawnLevel((int)value);
                break;
        }
    }
}
