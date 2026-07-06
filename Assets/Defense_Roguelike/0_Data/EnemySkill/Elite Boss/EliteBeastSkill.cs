using UnityEngine;

[CreateAssetMenu(fileName = "EliteBeastSkill", menuName = "Enemy Skill Data/EliteBeastSkill")]
public class EliteBeastSkill : ScriptableObject, IEnemyOnEnablePassive, IEnemyDeathPassive, IEnemyAttackPassive
{
    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.EliteEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SpawnEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Wings);

        enemy.baseSpeed *= 1.2f;
        enemy.attackSpeed *= 0.5f;
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
