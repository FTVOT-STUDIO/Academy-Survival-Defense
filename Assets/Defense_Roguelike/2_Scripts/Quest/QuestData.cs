using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "QuestData/QuestData")]
public class QuestData : ScriptableObject
{
    public enum QuestType { Login, GamePlay, UnitSpawn, EnemyKill, GameVictory, ShopBuyItem, BossKill, GetSkillCard, OpenBox, UnitMerge, UseDarkMatte, UseAnomaly }
    public int questID;
    public QuestType questType;
    public string questName = "None";
    public int progress = 0;
    public int maxProgress = 1;
    public int questPoint = 1;
    public bool isClear = false;
}
