using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class JointOperationMetatronMode : IGameMode
{
    private GameManager manager;
    public bool isWaveChange;
    public void Initialize(GameManager manager)
    {
        this.manager = manager;

        CinemachineCameraManager.SetCameraPosition(12f, -1.8f);
        GameManager.Inst.spawnManager.isPathLoop = true;

    }

    public void GrantResources()
    {
        manager.ModifyCoin(7000);
        manager.ModifyDarkMatteCoin(30);
    }
    private bool isWaveChangeProcessing;

    public void Update()
    {
        if (!manager.isGameStart || manager.isPause)
            return;

        if (isWaveChangeProcessing)
            return;

        if (manager.isIntroDelay)
            return;

        manager.timer -= Time.deltaTime;

        if (manager.timer <= 0f)
        {
            manager.StartCoroutine(ProcessWaveChangeRoutine());
        }
    }

    private IEnumerator ProcessWaveChangeRoutine()
    {
        isWaveChangeProcessing = true;

        if (manager.wave != 0 && manager.wave % 10 == 0)
        {
            manager.StartBossKilledFlow(0);
            yield return null;
            while (manager.isIntroDelay)
            {
                yield return null;
            }
        }

        manager.wave++;
        manager.isBossSpawnable = false;
        manager.timer = manager.waveDuration;
        manager.OnWaveChangedInvoke();
        manager.isAnomaly = false;

        InGameUIManager.Inst.waveChangedPanel.SetActive(!CanSpawnBoss());

        manager.HandleBossWave();
        manager.HandleEliteSpawn();
        SetSpawnTime(manager.wave);
        isWaveChangeProcessing = false;
    }

    public void LateUpdate()
    {

    }

    public void OnLevelUp()
    {

    }

    public void OnWaveUpdate()
    {

    }

    public void SetSpawnTime(int wave)
    {
        manager.spawnTime = 0.7f;
    }

    public IEnumerator BossWaveRoutine()
    {
        manager.isBossSpawnable = true;
        manager.isBossActive = true;
        InGameUIManager.Inst.nextBossPanel.SetActive(false);
        InGameUIManager.Inst.bossAppearPanel.SetActive(true);
        EnemySpawnManager.Inst.StartBossSpawnSequence();
        yield return null;
    }

    public bool IsGameVictory()
    {
        return !GameModeManager.Inst.jointOperationsManager.bossEnemy.isLive;
    }

    public bool CanSpawnEnemy()
    {
        return !manager.isGameStart || manager.isPause;
    }

    public bool CanOfferElite()
    {
        return manager.wave > 0 && manager.wave % 2 == 0;
    }

    public bool CanSpawnBoss()
    {
        return manager.wave > 0 && manager.wave % 10 == 0;
    }

    public void SetUpNextBoss(EnemySpawnManager spawnManager)
    {
        spawnManager.AddBoss(spawnManager.RandomBoss(), true);
        spawnManager.AddBoss(spawnManager.RandomBoss(), true);
    }

    public void InitializeOnFirstStart()
    {
        GameModeManager.Inst.jointOperationsManager.GenerateBoss();
        GameStatManager.Inst.SetEnemyHpRatioDamageReduction(0.999f);
    }

    public void SetUpBgm()
    {
        AudioManager.Inst.PlayBgm(AudioManager.Bgm.Metatron);
    }

    public float GetIncreaseStatsPerWave()
    {
        float wave = manager.wave;
        if (wave < 5) return 1f;       // 1~4
        if (wave < 10) return 30f;     // 5~9
        if (wave < 20) return 80f;     // 10~19
        if (wave < 30) return 300f;    // 20~29
        if (wave < 40) return 500f;    // 30~39
        if (wave < 50) return 1000f;   // 40~49
        if (wave < 60) return 3000f;   // 50~59
        if (wave < 70) return 5000f;   // 60~69
        if (wave < 80) return 10000f;  // 70~79

        return 10000f;                 // 80 이상 (0.3f → 10000f 로 맞춰줌)
    }

    public bool CheckEmergencyCondition()
    {
        return false;
    }

    public void OnBossKilled()
    {

    }

    public float DelayNextWave()
    {
        return 2;
    }

}
