using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JointOperationsManager : MonoBehaviour
{
    public enum JointOperationDifficulty { Normal, Hard, Extream, Insane, Apocalypse, Oblivion }
    public JointOperationDifficulty curJointOperationDifficulty;
    public JointOperationData[] jointOperationDatas;
    [SerializeField] public JointOperationData curJointOperationData;
    [SerializeField] public Enemy bossEnemy;
    public JointOperationBossHpSlider jointOperationBossHpSlider;
    [SerializeField] JointOperationsLobbyManager jointOperationsLobbyManager;
    [SerializeField] TMP_Text contentUnlockConditionCountText;

    public void GenerateBoss()
    {
        Enemy enemy = PoolManager.Inst.Get(PoolManager.PrefabType.enemy0, transform.position).GetComponent<Enemy>();
        bossEnemy = enemy;
        enemy.enemyData = curJointOperationData.jointOperationBossData;
        enemy.Init();
        jointOperationBossHpSlider.Init();
    }

    public void InitContentUnlockConditionCount()
    {
        contentUnlockConditionCountText.text = "보유한 유닛 : " + EquipmentManager.Inst.unlockUnitTabItems.Count + " / 8 \n" + "보유한 세피라 : " + EquipmentManager.Inst.unlockSephiraTabItems.Count + " / 3 \n" + "보유한 필드 : " + EquipmentManager.Inst.unlockFieldTabItems.Count + " / 2";
    }

    public void InitJointOperationLobby()
    {
        jointOperationsLobbyManager.Init(curJointOperationData);
    }

    public JointOperationData GetRandomJointOperationData()
    {
        return jointOperationDatas[UnityEngine.Random.Range(0, jointOperationDatas.Length)];
    }

    public Dictionary<JointOperationDifficulty, string> GetDifficultyKorean = new Dictionary<JointOperationDifficulty, string>()
    {
        {JointOperationDifficulty.Normal,"노말"},
        {JointOperationDifficulty.Hard,"하드"},
        {JointOperationDifficulty.Extream,"익스트림"},
        {JointOperationDifficulty.Insane,"인세인"},
        {JointOperationDifficulty.Apocalypse,"아포칼립스"},
        {JointOperationDifficulty.Oblivion,"오블리비온"},
    };

    public float GetJointOperationDifficultyMultiplier()
    {
        switch (curJointOperationDifficulty)
        {
            case JointOperationDifficulty.Normal: return 1.0f;
            case JointOperationDifficulty.Hard: return 2f;
            case JointOperationDifficulty.Insane: return 5f;
            case JointOperationDifficulty.Extream: return 10f;
            case JointOperationDifficulty.Apocalypse: return 20f;
            case JointOperationDifficulty.Oblivion: return 50f;
            default: return 1.0f;
        }
    }
}
