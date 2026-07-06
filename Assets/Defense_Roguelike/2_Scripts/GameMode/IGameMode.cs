using System.Collections;

public interface IGameMode
{
    void Initialize(GameManager manager);
    void OnWaveUpdate();
    void OnLevelUp();
    void Update();
    void LateUpdate();
    void SetSpawnTime(int wave);
    void GrantResources();
    bool CanSpawnEnemy();
    bool CanOfferElite();
    bool CanSpawnBoss();
    bool IsGameVictory();
    void InitializeOnFirstStart();
    void SetUpNextBoss(EnemySpawnManager enemySpawnManager);
    void SetUpBgm();
    void OnBossKilled();
    bool CheckEmergencyCondition();
    float GetIncreaseStatsPerWave();
    float DelayNextWave();
    IEnumerator BossWaveRoutine();
}