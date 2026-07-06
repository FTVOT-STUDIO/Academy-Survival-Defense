using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HardMode : IGameMode
{
    private GameManager manager;

    public void Initialize(GameManager manager)
    {
        EffectManager.Inst.light2D.color = new Color(1, 0.6f, 0.7f);
        this.manager = manager;
        CinemachineCameraManager.SetCameraPosition(10f, -3f);
    }

    public void InitializeOnFirstStart()
    {
        FieldManager.Inst.fieldSephira.maxAbllityCount *= 2;
        GameStatManager.Inst.SetEnemyHpRatioDamageReduction(0.3f);
    }

    public void GrantResources()
    {
        manager.ModifyCoin(2000);
        manager.ModifyDarkMatteCoin(10);
    }

    public void Update()
    {
        if (!manager.isGameStart || manager.isBossActive || manager.isIntroDelay || manager.isPause)
            return;

        manager.timer -= Time.deltaTime;

        if (manager.timer <= 0)
        {
            manager.wave++;
            manager.isBossSpawnable = false;
            manager.timer = manager.waveDuration;
            manager.OnWaveChangedInvoke();
            manager.isAnomaly = false;

            InGameUIManager.Inst.waveChangedPanel.SetActive(!CanSpawnBoss());

            manager.HandleBossWave();
            manager.HandleEliteSpawn();
            SetSpawnTime(manager.wave);
        }
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
        float spawnTime;
        if (wave < 5)
            spawnTime = 1.5f;       // 1~4
        else if (wave < 10)
            spawnTime = 1.1f;       // 5~9
        else if (wave < 20)
            spawnTime = 0.9f;       // 10~19
        else if (wave < 30)
            spawnTime = 0.4f;       // 20~29
        else if (wave < 40)
            spawnTime = 0.2f;       // 30~39
        else if (wave < 50)
            spawnTime = 0.2f;       // 40~49
        else if (wave < 60)
            spawnTime = 0.1f;       // 50~59
        else
            spawnTime = 0.1f;       // 60 이상 고정

        manager.spawnTime = spawnTime;
    }

    public IEnumerator BossWaveRoutine()
    {
        manager.timer = 60;
        manager.isBossSpawnable = true;
        manager.isBossActive = true;
        InGameUIManager.Inst.nextBossPanel.SetActive(false);
        InGameUIManager.Inst.bossAppearPanel.SetActive(true);
        EnemySpawnManager.Inst.StartBossSpawnSequence();

        yield return new WaitForSeconds(1.5f);

        while (manager.timer > 0)
        {
            if (manager.isBossActive)
            {
                manager.timer -= Time.deltaTime;
                yield return null;
            }
            else
            {
                yield break;
            }

        }
        if (manager.timer <= 0 && manager.isBossActive)
        {
            manager.fieldManager.fieldSephira.Ondamage(999999999);
        }
    }

    public bool IsGameVictory()
    {
        return manager.wave >= 80;
    }

    public bool CanSpawnEnemy()
    {
        return !GameManager.Inst.isGameStart || GameManager.Inst.isIntroDelay || GameManager.Inst.isPause
        || manager.isBossActive;
    }

    public bool CanOfferElite()
    {
        return manager.wave > 0 && manager.wave % 3 == 0;
    }

    public bool CanSpawnBoss()
    {
        return manager.wave > 0 && manager.wave % 10 == 0;
    }

    public void SetUpNextBoss(EnemySpawnManager spawnManager)
    {
        float wave = manager.wave;
        switch (wave)
        {
            case 0:
            case 10:
                spawnManager.AddBoss(spawnManager.RandomBoss(), false);
                break;
            case 20:
            case 30:
                spawnManager.AddBoss(spawnManager.RandomBoss(), true);
                break;
            case 40:
                spawnManager.AddBoss(spawnManager.RandomBoss(), false);
                spawnManager.AddBoss(spawnManager.RandomBoss(), true);
                break;
            case 50:
            case 60:
            case 70:
                spawnManager.AddBoss(spawnManager.RandomBoss(), true);
                spawnManager.AddBoss(spawnManager.RandomBoss(), true);
                break;
        }
    }

    public void SetUpBgm()
    {
        AudioManager.Inst.PlayBgm(AudioManager.Bgm.Hard);
    }

    public float GetIncreaseStatsPerWave()
    {
        float wave = manager.wave;
        if (wave <= 5)
            return 4;         // 1~4
        else if (wave <= 10)
            return 6;         // 5~9
        else if (wave <= 20)
            return 10;        // 10~19
        else if (wave <= 30)
            return 20;        // 20~29
        else if (wave <= 40)
            return 60;       // 30~39
        else if (wave <= 50)
            return 200f;       // 40~49
        else if (wave <= 60)
            return 1000f;      // 50~59
        else if (wave <= 70)
            return 2000f;      // 60~69
        else if (wave <= 80)
            return 6000f;      // 70~79
        else
            return 10000;       // 80 이상
    }


    public bool CheckEmergencyCondition()
    {
        return manager.isBossActive && manager.timer <= 30;
    }

    public void OnBossKilled()
    {
        if (EnemySpawnManager.Inst.remainingBosses <= 0)
        {
            InGameUIManager.Inst.bossClearPanel.SetActive(true);
            manager.StartBossKilledFlow(2.5f);
        }
    }

    public float DelayNextWave()
    {
        return 2;
    }

}
