using UnityEngine;

public interface IEnemyBehavior
{
    void Initialize(Enemy enemy);
    void UpdateHealthBar();
    void UpdateShieldBar();
    void GrantDeathReward();
    void PerformAttack();
    void UseSkill();
    bool SetHpBarActive();
    void MoveWait();
}
