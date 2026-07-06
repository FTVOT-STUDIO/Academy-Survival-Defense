using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossInfoUiManger : MonoBehaviour
{
    [SerializeField] Transform bossSlotParent;
    [SerializeField] NextBossPreivewUI nextBossPreivewUI;
    //public List<EnemyData> enemyDatas = new List<EnemyData>();

    void OnEnable()
    {
        // enemyDatas.Clear();
        EnemySpawnManager.Inst.NextBoss();
        nextBossPreivewUI.Init();
        //enemyDatas.AddRange(EnemySpawnManager.Inst.nextBossData.Select(boss => boss.enemyData));


    }
}
