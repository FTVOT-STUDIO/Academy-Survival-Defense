using UnityEngine;

[CreateAssetMenu(fileName = "EliteBossSkill", menuName = "Enemy Skill Data/EliteBossSkill")]
public class EliteKnightSkill : ScriptableObject, IEnemyOnEnablePassive, IEnemyDeathPassive, IEnemyAttackPassive
{

    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.EliteEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SpawnEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Sword2);

        enemy.damage *= 1.2f;
        enemy.health *= 1.2f;
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
