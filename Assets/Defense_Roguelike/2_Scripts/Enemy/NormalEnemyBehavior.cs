using System.Linq;
using UnityEngine;

public class NormalEnemyBehavior : IEnemyBehavior
{
    Enemy enemy;

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.hpBarObject.SetActive(true);
        int difficultyIndex = (int)GameManager.Inst.curGameDifficulty;

        enemy.isLive = true;
        enemy.anim.runtimeAnimatorController = enemy.enemyData.animatorController[difficultyIndex];

        enemy.speed = enemy.enemyData.speed[difficultyIndex];
        enemy.enemySkill = enemy.enemyData.skillData;
        enemy.maxHealth = enemy.enemyData.maxHealth[difficultyIndex] * GameManager.Inst.wave * GameManager.Inst.currentGameMode.GetIncreaseStatsPerWave();

        enemy.damage = enemy.enemyData.damage[difficultyIndex] * Mathf.Pow(1.12f, GameManager.Inst.wave);
        enemy.health = enemy.maxHealth;
        enemy.maxShield = enemy.enemyData.maxShield[difficultyIndex] + enemy.maxHealth;
        enemy.defenseAmount = enemy.enemyData.defenseAmount[difficultyIndex];
        enemy.attackSpeed = enemy.enemyData.attackSpeed[difficultyIndex];
        float slowRatio = Mathf.Clamp01(2f - GameStatManager.Inst.GameSlowValue);
        enemy.speed = Mathf.Max(enemy.enemyData.speed[difficultyIndex] * (1 + 0.001f * GameManager.Inst.wave) * enemy.baseSpeed * slowRatio, 0.1f);

        enemy.attributeResistance.Clear();
        enemy.attributeWeakness.Clear();
        for (int i = 0; i < enemy.enemyData.attributeStats[difficultyIndex].resistances.Length; i++)
        {
            enemy.attributeResistance.Add(new AttributeModifierData());
            enemy.attributeResistance[i].data = enemy.enemyData.attributeStats[difficultyIndex].resistances[i].data;
            enemy.attributeResistance[i].value = enemy.enemyData.attributeStats[difficultyIndex].resistances[i].value;
        }
        for (int i = 0; i < enemy.enemyData.attributeStats[difficultyIndex].weaknesses.Length; i++)
        {
            enemy.attributeWeakness.Add(new AttributeModifierData());
            enemy.attributeWeakness[i].data = enemy.enemyData.attributeStats[difficultyIndex].weaknesses[i].data;
            enemy.attributeWeakness[i].value = enemy.enemyData.attributeStats[difficultyIndex].weaknesses[i].value;
        }

        enemy.gameObject.layer = 10;

        enemy.startSortingOrder = 20;
        enemy.sp.sortingOrder = enemy.startSortingOrder;

        enemy.SetHitColliderSizeAndOffset(0.7f, 0.7f, 0, 0.2f);
        enemy.SetUnitDetecColliderSizeAndOffset(0.5f, 0.5f, 0, 0.2f);
        enemy.SetSephiraDetecColliderSizeAndOffset(0.5f, 0.5f, 0, 0.2f);
        enemy.SetHpBarLocateAndScale(0.6f, 0.5f, new Vector3(1, 1, 1));
        enemy.SetBuffAndDebuffParentScale(1);
        enemy.enemyAttributeWeaknessManager.Init();
    }

    public void GrantDeathReward()
    {
        FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(1);
        GameManager.Inst.GetExp(1);
    }

    public bool SetHpBarActive()
    {
        return true;
    }

    public void PerformAttack()
    {
        enemy.enemyWeapon.OnAttack();
    }

    public void UpdateHealthBar()
    {
        enemy.UpdateHealthBar();
    }

    public void UpdateShieldBar()
    {
        enemy.UpdateShieldBar();
    }

    public void UseSkill()
    {

    }

    public void MoveWait()
    {
    }
}
