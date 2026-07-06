using System.Linq;
using UnityEngine;

public class BossEnemyBehavior : IEnemyBehavior
{
    Enemy enemy;

    public void GrantDeathReward()
    {
        int exp = 10;
        int darkMatteCoin = enemy.enemyData.darkMatteCoinPerDifficulty[(int)GameManager.Inst.curGameDifficulty];
        int abllityGain = 10;

        GameManager.Inst.GetExp(exp);
        GameManager.Inst.ModifyDarkMatteCoin(darkMatteCoin);
        FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(abllityGain);

        GameManager.Inst.OnBossKilled();
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.BossKill, 1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.EnemyDead);
    }

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;

        int difficultyIndex = (int)GameManager.Inst.curGameDifficulty;

        enemy.anim.runtimeAnimatorController = enemy.enemyData.animatorController[0];
        enemy.speed = enemy.enemyData.speed[difficultyIndex];
        enemy.enemySkill = enemy.enemyData.skillData;
        enemy.maxHealth = enemy.enemyData.maxHealth[difficultyIndex] * GameManager.Inst.wave * GameManager.Inst.currentGameMode.GetIncreaseStatsPerWave();
        enemy.damage = enemy.enemyData.damage[difficultyIndex] * (1 + 0.1f * GameManager.Inst.wave);
        enemy.health = enemy.maxHealth;
        enemy.maxShield = enemy.enemyData.maxShield[difficultyIndex] + enemy.maxHealth;
        enemy.defenseAmount = enemy.enemyData.defenseAmount[difficultyIndex];

        enemy.isLive = false; // Appaer Waiting Time
        enemy.EnableCollider(false);

        if (EnemySpawnManager.Inst.nextBossData[enemy.spawnIndex].onSilhouette)
            enemy.anim.SetTrigger("Appear_Silhouette");
        else
            enemy.anim.SetTrigger("Appear");

        if (enemy.enemySkill is IEnemyOnEnablePassive passive)
            passive.ApplyEnemyOnEnablePassive(enemy, enemy);

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

        enemy.gameObject.layer = 11;
        enemy.startSortingOrder = 22;
        enemy.sp.sortingOrder = enemy.startSortingOrder;

        enemy.SetHitColliderSizeAndOffset(1, 3.5f, 0, 1.5f);
        enemy.SetUnitDetecColliderSizeAndOffset(1, 1.5f, 0, 1.5f);
        enemy.SetSephiraDetecColliderSizeAndOffset(1, 1.5f, 0, 1.5f);
        enemy.SetHpBarLocateAndScale(0.8f, 4, new Vector3(3f, 3f, 3f));
        enemy.SetBuffAndDebuffParentScale(3);
        enemy.enemyAttributeWeaknessManager.Init();
    }

    public void PerformAttack()
    {
        enemy.enemyWeapon.OnAttack();
    }

    public void UpdateHealthBar()
    {
        enemy.UpdateHealthBar();
    }

    public bool SetHpBarActive()
    {
        return true;
    }

    public void UpdateShieldBar()
    {
        enemy.UpdateShieldBar();
    }

    public void UseSkill()
    {
        enemy.enemyWeapon.OnSkill();
    }

    public void MoveWait()
    {
    }
}
