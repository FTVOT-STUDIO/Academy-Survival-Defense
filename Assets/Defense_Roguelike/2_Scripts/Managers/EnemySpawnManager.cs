using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
[System.Serializable]
public class NextBossData
{
    public EnemyData enemyData;
    public bool onSilhouette;
}

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Inst;

    public List<Enemy> spawnedEnemies = new List<Enemy>();
    [SerializeField] GameObject normalWayPointsPrefab;
    [SerializeField] GameObject jointOpertaionWayPointsPrefab;
    public Transform[] spawnPointParent;
    public Transform[] spawnPoint;
    public int enemyCount = 0;
    [Space]
    public EnemyData[] enemyDatas;
    public EnemyData[] eliteDats;
    public EnemyData[] bossDatas;
    public List<NextBossData> nextBossData;
    public bool isPathLoop;
    public int remainingBosses;

    float timer;

    void Awake()
    {
        Inst = this;
    }

    void OnEnable()
    {
        Transform curWayPointPrefab;
        switch (GameManager.Inst.gameMode)
        {
            case GameManager.GameMode.Normal:
                curWayPointPrefab = normalWayPointsPrefab.transform;
                break;
            case GameManager.GameMode.JointOperation:
                curWayPointPrefab = jointOpertaionWayPointsPrefab.transform;
                break;
            default:
                curWayPointPrefab = normalWayPointsPrefab.transform;
                break;
        }
        curWayPointPrefab.gameObject.SetActive(true);

        List<Transform> wayPointTransforms = new List<Transform>();
        List<Transform> wayPointParents = new List<Transform>();

        foreach (Transform child in curWayPointPrefab.transform)
        {
            if (child.CompareTag("WayPoint"))
            {
                wayPointParents.Add(child);

                foreach (Transform wayPointChild in child)
                {
                    wayPointTransforms.Add(wayPointChild.GetChild(1));
                }
            }
        }

        spawnPoint = wayPointTransforms.ToArray();
        spawnPointParent = wayPointParents.ToArray();
        spawnedEnemies.Clear();
    }

    void Update()
    {
        if (GameManager.Inst.currentGameMode.CanSpawnEnemy())
            return;

        timer += Time.deltaTime;

        if (timer > GameManager.Inst.spawnTime)
        {
            timer = 0;
            Spawn();

        }
    }

    public void OnClickEliteSpawn()
    {
        if (GameManager.Inst.isEliteActive)
            return;

        GameManager.Inst.isEliteActive = true;

        Transform spawnPosition = spawnPoint[Random.Range(0, spawnPoint.Length)];

        Transform spawnPosParent = spawnPosition.parent;

        GameObject enemy = PoolManager.Inst.Get(PoolManager.PrefabType.enemy0, spawnPosition.position);

        Enemy enemyScript = enemy.GetComponent<Enemy>();

        enemyScript.enemyData = eliteDats[Random.Range(0, eliteDats.Length)];

        List<Transform> spawnPointChild = new List<Transform>();

        for (int i = 1; i < spawnPosParent.childCount; i++)
        {
            spawnPointChild.Add(spawnPosParent.GetChild(i));
        }

        enemyScript.enemyMove.wayPos = spawnPointChild.ToArray();

        EffectManager.Inst.Get(EffectManager.PrefabType.EnemySpawnEffect, enemy.transform.position);
        GameFlowManager.Inst.ChangeSpentAnomaly(1);

        enemyScript.Init();

        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.UseAnomaly, 1);

        InGameUIManager.Inst.anomalyButton.SetActive(false);
    }

    public void NextBoss()
    {
        nextBossData.Clear();
        GameManager.Inst.currentGameMode.SetUpNextBoss(this);
    }

    public void AddBoss(EnemyData enemyData, bool silhouetteOn)
    {
        NextBossData data = new()
        {
            enemyData = enemyData,
            onSilhouette = silhouetteOn
        };
        nextBossData.Add(data);
        remainingBosses++;
    }

    public int WaveBossCount()
    {
        int Count;
        if (GameManager.Inst.wave <= 1)
        {
            Count = 1;
        }
        else
        {
            Count = 2;
        }
        return Count;
    }

    void Spawn()
    {
        Transform spawnPosition = spawnPoint[Random.Range(0, spawnPoint.Length)];

        Transform spawnPosParent = spawnPosition.parent;

        GameObject enemy = PoolManager.Inst.Get(PoolManager.PrefabType.enemy0, spawnPosition.position);

        Enemy enemyScript = enemy.GetComponent<Enemy>();

        enemyScript.enemyData = enemyDatas[Random.Range(0, enemyDatas.Length)];

        List<Transform> spawnPointChild = new List<Transform>();

        for (int i = 1; i < spawnPosParent.childCount; i++)
        {
            spawnPointChild.Add(spawnPosParent.GetChild(i));
        }

        enemyScript.enemyMove.wayPos = spawnPointChild.ToArray();

        enemyScript.Init();

    }

    public void StartBossSpawnSequence()
    {
        StartCoroutine(SpawnBossCoroutine());
    }


    IEnumerator SpawnBossCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        if (nextBossData.Count == 1)
        {
            GenerateBoss(0);
        }
        else
        {
            for (int i = 0; i < nextBossData.Count; i++)
            {
                GenerateBoss(i);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void GenerateBoss(int index)
    {
        int ranIndex = Random.Range(0, spawnPoint.Length);
        Transform spawnPosition = spawnPoint[ranIndex];

        Transform spawnPosParent = spawnPosition.parent;

        GameObject enemy = PoolManager.Inst.Get(PoolManager.PrefabType.enemy0, spawnPosition.position);

        Enemy enemyScript = enemy.GetComponent<Enemy>();

        enemyScript.enemyData = nextBossData[index].enemyData;

        List<Transform> spawnPointChild = new List<Transform>();

        for (int j = 1; j < spawnPosParent.childCount; j++)
        {
            spawnPointChild.Add(spawnPosParent.GetChild(j));
        }

        enemyScript.enemyMove.wayPos = spawnPointChild.ToArray();
        enemyScript.spawnIndex = index;
        enemyScript.Init();
    }

    public EnemyData RandomBoss()
    {
        return bossDatas[UnityEngine.Random.Range(0, bossDatas.Length)];
    }

    public List<Enemy> GetSpawnedEnemies()
    {
        return spawnedEnemies;
    }
}
