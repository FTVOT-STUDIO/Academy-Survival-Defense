using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JointOperationBossHpSlider : MonoBehaviour
{
    [SerializeField] Slider bossHpSlider;
    [SerializeField] Slider bossShieldSlider;
    [SerializeField] TMP_Text bossHealthText;
    [SerializeField] TMP_Text bossNameText;

    public void Init()
    {
        Enemy enemy = GameModeManager.Inst.jointOperationsManager.bossEnemy;
        UpdateHealthSlider(enemy);
        enemy.enemyEvent.OnDamaged += (enemy) => UpdateHealthSlider(enemy);
    }

    private void UpdateHealthSlider(Enemy enemy)
    {
        bossNameText.text = enemy.enemyData.enemyName;
        float currentHealth = Mathf.Max(0f, (float)enemy.health);
        bossHealthText.text = $"{currentHealth:N0}/{enemy.maxHealth:N0}";
        bossHpSlider.value = (float)enemy.health / (float)enemy.maxHealth;

        bossShieldSlider.value = (float)enemy.shield / (float)enemy.maxShield;
    }
}
