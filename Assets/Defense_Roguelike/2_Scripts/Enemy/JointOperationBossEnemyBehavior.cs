using System.Linq;
using DG.Tweening;
using UnityEngine;

public class JointOperationBossEnemyBehavior : IEnemyBehavior
{
    Enemy enemy;

    public void GrantDeathReward()
    {
        int exp = 5;
        int darkMatteCoin = enemy.enemyData.darkMatteCoinPerDifficulty[(int)GameManager.Inst.curGameDifficulty];
        int abllityGain = 5;

        GameManager.Inst.GetExp(exp);
        GameManager.Inst.ModifyDarkMatteCoin(darkMatteCoin);
        FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(abllityGain);

        GameManager.Inst.isEliteActive = false;
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.EnemyDead);
    }

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.enemyMove.enabled = false;
        enemy.transform.position = new Vector3(enemy.transform.position.x, -1.5f, enemy.transform.position.z);
        enemy.hpBarObject.SetActive(false);

        enemy.anim.runtimeAnimatorController = enemy.enemyData.animatorController[0];
        enemy.speed = enemy.enemyData.speed[0];
        enemy.enemySkill = enemy.enemyData.skillData;
        enemy.maxHealth = enemy.enemyData.maxHealth[0];
        enemy.health = enemy.maxHealth;
        enemy.damage = enemy.enemyData.damage[0] * (1 + 0.1f * GameManager.Inst.wave);
        enemy.maxShield = enemy.enemyData.maxShield[0] + enemy.maxHealth;
        enemy.defenseAmount = enemy.enemyData.defenseAmount[0];

        enemy.speed = enemy.enemyData.speed[0];
        enemy.enemySkill = enemy.enemyData.skillData;

        enemy.isLive = false; // Appaer Waiting Time
        enemy.EnableCollider(false);

        enemy.anim.SetTrigger("Appear");

        if (enemy.enemySkill is IEnemyOnEnablePassive passive)
            passive.ApplyEnemyOnEnablePassive(enemy, enemy);

        enemy.attributeResistance.Clear();
        enemy.attributeWeakness.Clear();
        for (int i = 0; i < enemy.enemyData.attributeStats[0].resistances.Length; i++)
        {
            enemy.attributeResistance.Add(new AttributeModifierData());
            enemy.attributeResistance[i].data = enemy.enemyData.attributeStats[0].resistances[i].data;
            enemy.attributeResistance[i].value = enemy.enemyData.attributeStats[0].resistances[i].value;
        }
        for (int i = 0; i < enemy.enemyData.attributeStats[0].weaknesses.Length; i++)
        {
            enemy.attributeWeakness.Add(new AttributeModifierData());
            enemy.attributeWeakness[i].data = enemy.enemyData.attributeStats[0].weaknesses[i].data;
            enemy.attributeWeakness[i].value = enemy.enemyData.attributeStats[0].weaknesses[i].value;
        }


        enemy.gameObject.layer = 11;

        enemy.startSortingOrder = -4;
        enemy.sp.sortingOrder = enemy.startSortingOrder;

        enemy.SetHitColliderSizeAndOffset(8, 1, 0, 0.5f);
        enemy.SetUnitDetecColliderSizeAndOffset(9, 1, 0, -1f);
        enemy.SetSephiraDetecColliderSizeAndOffset(1, 1, 0, 2);

        enemy.SetHpBarLocateAndScale(0.8f, 3, new Vector3(3f, 3f, 3f));
        GameModeManager.Inst.jointOperationsManager.bossEnemy = enemy;
        enemy.SetBuffAndDebuffParentScale(5f);

        InGameUIManager.Inst.jointOperationBossHpSlider.gameObject.SetActive(true);
    }

    public void MoveWait()
    {
        GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, Color.red);
    }

    public void PerformAttack()
    {
        enemy.enemyWeapon.OnAttack();
    }

    public bool SetHpBarActive()
    {
        return false;
    }

    public void UpdateHealthBar()
    {

    }

    public void UpdateShieldBar()
    {

    }

    public void UseSkill()
    {
        enemy.enemyWeapon.OnSkill();
    }
}
