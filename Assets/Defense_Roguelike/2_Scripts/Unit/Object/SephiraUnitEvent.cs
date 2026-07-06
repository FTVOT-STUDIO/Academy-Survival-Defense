using System;
using DG.Tweening;
using UnityEngine;

public class SephiraUnitEvent : MonoBehaviour
{
    SephiraUnit sephiraUnit;
    public Action OnStatChanged;
    public Action OnCriticalHit;
    public Action OnAttackHit;
    public Action OnAbilityCharged;
    public bool isAbilityReady = false;

    void Awake()
    {
        sephiraUnit = GetComponent<SephiraUnit>();
        OnAbilityCharged += PlayAbilityReadyEffect;
    }

    public void MultiplyAbilityStats(float percent)
    {
        if (sephiraUnit.isSkillActive)
            return;

        sephiraUnit.abllityCount = Mathf.Min(sephiraUnit.abllityCount + sephiraUnit.maxAbllityCount * percent, sephiraUnit.maxAbllityCount);
        sephiraUnit.RecalculateStats();
        OnAbilityCharged?.Invoke();
    }

    public void AddDamage(float amount)
    {
        sephiraUnit.baseDamage += amount;
        sephiraUnit.RecalculateStats();
    }

    public void AddAbllityStats(float amount)
    {
        if (sephiraUnit.isSkillActive)
            return;
        sephiraUnit.abllityCount = Mathf.Min(sephiraUnit.abllityCount + amount, sephiraUnit.maxAbllityCount);
        sephiraUnit.RecalculateStats();
        OnAbilityCharged?.Invoke();
    }

    public void MultiplyAbilityStatsDecrease(float value)
    {
        if (sephiraUnit.isSkillActive)
            return;
        sephiraUnit.baseAbilityCount = Mathf.Clamp01(sephiraUnit.baseAbilityCount * (1f - value));
        sephiraUnit.RecalculateStats();
    }

    public void ApplyHeal(float amount, float healingMultiplier, float healingReceivedMultiplier)
    {
        amount *= healingMultiplier;
        amount *= healingReceivedMultiplier;
        sephiraUnit.health = Mathf.Min(sephiraUnit.health + amount, sephiraUnit.maxHealth);
        sephiraUnit.RecalculateStats();
        EffectManager.Inst.Get(EffectManager.PrefabType.SephiraHealEffect, sephiraUnit.transform.position);
    }

    public void ApplyGetShield(float value, float shieldMultiplier, float shieldReceivedMultiplier)
    {
        value *= shieldMultiplier;
        value *= shieldReceivedMultiplier;

        sephiraUnit.shield = Mathf.Min(sephiraUnit.shield + value, sephiraUnit.maxShield);
        GameStatManager.Inst.MarkStatsDirty();
    }

    private void PlayAbilityReadyEffect()
    {
        if (!isAbilityReady && sephiraUnit.abllityCount >= sephiraUnit.maxAbllityCount)
        {
            isAbilityReady = true;
            InGameUIManager.Inst.abilityReadyEffect.SetActive(true);
            Transform target = InGameUIManager.Inst.abilityButtonParent.transform;
            target.localScale = Vector3.one * 1.15f;

            target
               .DOScale(Vector3.one, 0.3f)
               .SetEase(Ease.InOutBack)
               .SetAutoKill(true);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.AbilityReady1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.AbilityReady2);
            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0, 1f, 0.7f), 0.2f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ApplyGetShield(10, sephiraUnit.shieldMultiplier, sephiraUnit.shieldReceivedMultiplier);
        }
    }
}
