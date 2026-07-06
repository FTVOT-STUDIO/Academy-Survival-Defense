using UnityEngine;

[CreateAssetMenu(fileName = "EliteLighthouseSkill", menuName = "Enemy Skill Data/EliteLighthouseSkill")]
public class EliteLighthouseSkill : ScriptableObject, IEnemyOnEnablePassive, IEnemyDeathPassive, IEnemyAttackPassive
{
    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.EliteEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SpawnEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Siren);

        foreach (var monster in EnemySpawnManager.Inst.GetSpawnedEnemies())
        {
            monster.baseSpeed *= 2f;
            monster.attackSpeed *= 0.5f;
            monster.enemyEvent.ApplyGetShield(monster.health * 0.2f);
        }
    }



    public void ApplyEnemyDeathPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {

    }

    public void ApplyEnemyAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Sword);
        EffectManager.Inst.Get(EffectManager.PrefabType.EnemyAttackEffect, enemy.enemyScanner.nearestTarget.transform.position);
    }
}
