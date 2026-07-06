using UnityEngine;

[CreateAssetMenu(fileName = "GamchicothBossSkill", menuName = "Enemy Skill Data/GamchicothBossSkill")]
public class GamchicothBossSkill : ScriptableObject, IEnemyCoolTimeAttackPassive, IEnemyOnEnablePassive, IWaitForAnimationEvent
{
    public void ApplyEnemyCoolTimeAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        enemy.enemyEvent.ApplyGetShield(enemy.maxHealth * 0.1f);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothSkill0);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothSkill1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothSkill2);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothSkill3);
    }

    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        enemy.maxShield = enemy.maxHealth * 2;
        enemy.enemyEvent.ApplyGetShield(enemy.maxShield);

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothAppear0);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothAppear1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothAppear2);
    }

    public void ApplyWaitForAnimationEvent(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        GameObject shieldObj = PoolManager.Inst.Get(PoolManager.PrefabType.GamchicothShield, enemy.transform.position);
        shieldObj.transform.SetParent(enemy.weaponPrefabParent);

        shieldObj.GetComponentsInChildren<SortingOrderController>()[0].Init(enemy.sp);
        shieldObj.GetComponentsInChildren<SortingOrderController>()[1].Init(enemy.sp);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GamchicothAppear3);
    }
}
