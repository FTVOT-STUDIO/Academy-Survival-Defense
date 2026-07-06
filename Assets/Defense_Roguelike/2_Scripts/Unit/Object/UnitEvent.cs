using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IUnitStatusFlag
{
    bool IsParasitic { get; set; }
    bool IsSyntaxError { get; set; }
    bool IsSilenced { get; set; }
    bool IsInvulnerable { get; set; }

    bool IsUnitLocked { get; set; }
    bool IsIgnored { get; set; }
    bool IsMergeRandomized { get; set; }

    float ParasiticRemainingTime { get; set; }
    float SyntaxErrorRemainingTime { get; set; }
    float SilencedRemainingTime { get; set; }
    float InvulnerableRemainingTime { get; set; }
}
public class UnitEvent : MonoBehaviour, IUnitStatusFlag
{
    Unit unit;

    [Header("# Special Shield")]
    public int specialShieldCount;

    [Header("# Debuff Flag")]
    public bool IsParasitic { get; set; }
    public bool IsSyntaxError { get; set; }
    public bool IsSilenced { get; set; }
    [field: SerializeField] public bool IsInvulnerable { get; set; }

    public bool IsUnitLocked { get; set; }
    public bool IsIgnored { get; set; }
    public bool IsMergeRandomized { get; set; }

    public float ParasiticRemainingTime { get; set; }
    public float SyntaxErrorRemainingTime { get; set; }
    public float SilencedRemainingTime { get; set; }
    public float InvulnerableRemainingTime { get; set; }

    private Coroutine parasiticCoroutine;
    private Coroutine syntaxErrorCoroutine;
    private Coroutine silencedCoroutine;

    public GameObject specialShield;
    public GameObject invulnerableShield;

    float hitChanceDecrease;
    float damageDecrease;

    public int invulnerableCount;

    public Action unitEventHandler;
    public Action OnStatChanged;
    public Action OnCriticalHit;
    public Action OnAttackHit;
    public Action OnBuffChanged;
    public Action OnDeath;

    public Action<Unit> onUnitPlacedAction;
    public Action<Unit> onUnitRemovedAction;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void ApplyDamageStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseDamage *= 1 + value;
        }
        else
        {
            unit.BaseDamage /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplySkillAmountStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseSkillValue *= 1 + value;
        }
        else
        {
            unit.BaseSkillValue /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyHealthStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseMaxHealth *= 1 + value;
        }
        else
        {
            unit.BaseMaxHealth /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyShieldStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseMaxShield *= 1 + value;
        }
        else
        {
            unit.BaseMaxShield /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyAttackSpeedStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseAttackSpeed *= 1 + value;
        }
        else
        {
            unit.BaseAttackSpeed /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyRangeStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseRange *= 1 + value;
        }
        else
        {
            unit.BaseRange /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyCoolTimeState(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseCoolTime += value;
        }
        else
        {
            unit.BaseCoolTime -= value;
        }
        unit.RecalculateStats();
    }

    public void ApplyCriticalPerStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseCriticalPer *= 1 + value;
        }
        else
        {
            unit.BaseCriticalPer /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyCiriticalDamageStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseCriticalDamage *= 1 + value;
        }
        else
        {
            unit.BaseCriticalDamage /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplyDefenseStats(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseDefense *= 1 + value;
        }
        else
        {
            unit.BaseDefense /= 1 + value;
        }
        unit.RecalculateStats();
    }

    public void ApplySkillTriggerChanceState(float value, bool isApply, bool isDirty = true)
    {
        if (IsIgnored)
            return;
        if (isApply)
        {
            unit.BaseSkillTriggerChance += value;
        }
        else
        {
            unit.BaseSkillTriggerChance -= value;
        }
        unit.RecalculateStats();
    }

    public void ApplyInvulnerable(float duration)
    {
        StartCoroutine(SetInvulnerableCoroutine(duration));
    }

    IEnumerator SetInvulnerableCoroutine(float duration)
    {
        InvulnerableRemainingTime = duration;
        IsInvulnerable = true;
        invulnerableShield = PoolManager.Inst.Get(PoolManager.PrefabType.InvulnerableShield, unit.transform.position);

        invulnerableShield.GetComponentInChildren<SpriteRenderer>().sortingOrder = unit.sp.sortingOrder + 1;
        invulnerableShield.GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = unit.sp.sortingOrder + 1;

        invulnerableShield.transform.SetParent(unit.buffPrefabParent);

        invulnerableCount++;

        while (InvulnerableRemainingTime > 0f)
        {
            InvulnerableRemainingTime -= Time.deltaTime;
            yield return null;
        }
        invulnerableCount = Mathf.Max(0, invulnerableCount - 1);

        if (invulnerableCount == 0)
        {
            IsInvulnerable = false;
            invulnerableShield.transform.SetParent(PoolManager.Inst.transform);
            invulnerableShield.SetActive(false);
            invulnerableShield = null;
        }
    }

    public void ApplyGetHeal(float value, float healingMultiplier, float healingReceivedMultiplier, UnitData owner = null)
    {
        if (IsIgnored)
            return;

        value *= healingMultiplier;
        value *= healingReceivedMultiplier;
        value = Mathf.Max(0, value);

        if (owner != null)
            UnitCombatStatManager.Inst.AddHeal(owner, value);


        EventFloatingText("+" + value.ToString("F0"), 5, new Color(0, 0.9f, 0.2f));
        unit.health = Mathf.Min(unit.health + value, unit.maxHealth);
        unit.RecalculateStats();
    }

    public void ApplyGetCoolTime(float value)
    {
        if (IsIgnored)
            return;

        float finalValue = value * unit.manaReceivedMultiplier;

        EventFloatingText(finalValue.ToString("F1"), 5, new Color(0, 0.8f, 1));
        unit.unitWeapon.skillTime = Mathf.Min(unit.unitWeapon.skillTime + finalValue, unit.coolTime);
        EffectManager.Inst.Get(EffectManager.PrefabType.ChokmahManaRegenEffect, gameObject.transform.position);
        unit.RecalculateStats();
    }


    public void ApplyGetShield(float value, float shieldMultiplier, float shieldReceivedMultiplier)
    {
        if (IsIgnored)
            return;

        value *= shieldMultiplier;
        value *= shieldReceivedMultiplier;

        unit.shield = Mathf.Min(unit.shield + value, unit.maxShield);
        unit.RecalculateStats();
    }

    public void TakeDamageInstant(float damage)
    {
        unit.health -= damage;
        UnitCombatStatManager.Inst.AddTakenDamage(unit.unitData, damage);
        if (unit.health <= 0)
        {
            if (unit.unitSkill is IOnUnitDeathOverride onUnitDeathOverride)
            {
                onUnitDeathOverride.ApplyOnUnitDeathOverride(this);
            }
            else
            {
                unit.Dead();
                return;
            }
        }
        unit.UpdateHealthBar();
    }

    public void ApplyRandomizeUnit()
    {
        if (DecrementSpecialShield() || IsInvulnerable || IsIgnored)
            return;

        unit.unitDrag.ResetLoaction();
        int randomIndex = Random.Range(0, DeckManager.Inst.curGameDeckUnitDatas.Count);
        if (unit.unitSkill is IOnDisablePassive onDisablePassive)
        {
            onDisablePassive.ApplyOnDisablePassive(this);
        }
        if (unit.unitSkill is IOnDisableSkill onDisableSkill)
        {
            onDisableSkill.ApplyOnDisableSkill(this);
        }
        if (unit.unitSkill is ITriggerAfterTransform unitAfterTransform)
        {
            unitAfterTransform.OnTriggerAfterTransform();
        }

        unit.ResetUnitBuff();
        unit.unitData = DeckManager.Inst.curGameDeckUnitDatas[randomIndex];
        unit.Init(unit.starLevel, true, true);
        GameObject randomizeEffect = EffectManager.Inst.Get(EffectManager.PrefabType.RandomizeEffect, unit.transform.position);
        randomizeEffect.GetComponent<ObjectLifecycleManager>().SetParent(unit.debuffPrefabParent);
        randomizeEffect.GetComponent<ObjectLifecycleManager>().SetDelay(1);
        unit.anim.SetTrigger("Attack");
    }

    public void ApplyParasiticUnit(float damage, float duration, float interval)
    {
        if (DecrementSpecialShield() || IsInvulnerable || IsIgnored)
            return;

        if (unit.isLive && gameObject.activeInHierarchy)
        {
            if (IsParasitic)
                return;
            parasiticCoroutine = StartCoroutine(ParasiticUnitCoroutine(damage, duration, interval));
        }
    }

    public void ApplySyntaxErrorUnit(float hitChanceDecrease, float damageDecrease, float duration)
    {
        if (DecrementSpecialShield() || IsInvulnerable || IsIgnored)
            return;

        if (unit.isLive && gameObject.activeInHierarchy)
        {
            if (IsSyntaxError)
                return;
            syntaxErrorCoroutine = StartCoroutine(SyntaxErrorUnitCoroutine(hitChanceDecrease, damageDecrease, duration));
        }
    }

    public void ApplySilencedUnit(float duration)
    {
        if (DecrementSpecialShield() || IsInvulnerable || IsIgnored)
            return;

        if (unit.isLive && gameObject.activeInHierarchy)
        {
            if (IsIgnored)
                return;
            silencedCoroutine = StartCoroutine(SilencedUnitCoroutine(duration));
        }

    }

    public void ApplyLevelUpUnit(int amount, Transform effectPos = null)
    {
        if (IsIgnored)
            return;
        unit.unitLevel.AddLevel(amount);
        unit.RecalculateStats();

        EffectManager.Inst.Get(EffectManager.PrefabType.StarLevelUpEffect, effectPos ? effectPos.position : unit.transform.position);
    }

    public void ApplyLevelDownUnit(int amount, Transform effectPos = null)
    {
        if (IsInvulnerable || IsIgnored)
            return;
        unit.unitLevel.RemoveLevel(amount);
        unit.RecalculateStats();

        EffectManager.Inst.Get(EffectManager.PrefabType.StarLevelDownEffect, effectPos ? effectPos.position : unit.transform.position);
    }

    public void ApplyHitChanceStats(float value, bool isApply, bool isDirty = true)
    {
        if (isApply)
            unit.BaseHitChance *= value;
        else
            unit.BaseHitChance /= value;
        unit.RecalculateStats();
    }

    public void ApplyDurationHealToUnit(float healAmount, float healingMultiplier, float healingReceivedMultiplier, float time, float tick, UnitData owner = null)
    {
        StartCoroutine(DurationHealRoutine(healAmount, healingMultiplier, healingReceivedMultiplier, time, tick, owner));
    }

    public void IncrementSpecialShield()
    {
        if (IsIgnored)
            return;
        specialShieldCount++;

        if (specialShield == null)
        {
            specialShield = PoolManager.Inst.Get(PoolManager.PrefabType.SpecialShield, unit.transform.position);
            specialShield.GetComponent<DOTweenAnimation>().DORestartById("Show");
        }

        specialShield.GetComponentInChildren<SpriteRenderer>().sortingOrder = unit.sp.sortingOrder + 1;
        specialShield.GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = unit.sp.sortingOrder + 1;

        specialShield.GetComponentInChildren<TextMeshPro>().text = specialShieldCount.ToString();
        specialShield.GetComponentsInChildren<DOTweenAnimation>()[1].DORestartById("Show");

        specialShield.transform.SetParent(unit.buffPrefabParent);
        // specialShield.SetActive(true);
    }

    public bool DecrementSpecialShield()
    {
        if (IsIgnored)
            return false;

        if (specialShieldCount > 0)
        {
            specialShieldCount = Mathf.Max(specialShieldCount - 1, 0);

            specialShield.GetComponentInChildren<TextMeshPro>().text = specialShieldCount.ToString();
            specialShield.GetComponentsInChildren<DOTweenAnimation>()[1].DORestartById("Show");

            if (specialShieldCount == 0)
            {
                specialShield.transform.SetParent(PoolManager.Inst.transform);
                specialShield.SetActive(false);
                specialShield = null;
                EffectManager.Inst.Get(EffectManager.PrefabType.SpecialShieldHideEffect, transform.position);
            }
            return true;
        }

        return false;
    }

    public void ApplyKillEffect()
    {
        if (IsIgnored)
            return;

        if (WeaponManager.Inst.isKillShieldEnabled)
        {
            ApplyGetShield(unit.maxShield * WeaponManager.Inst.shieldAmount, GameStatManager.Inst.GameShieldMultiplier, GameStatManager.Inst.GameShieldReceivedMultiplier);
        }

        if (WeaponManager.Inst.isHealthDrainEnabled)
        {
            ApplyGetHeal(unit.maxHealth * WeaponManager.Inst.drainvalue, GameStatManager.Inst.GameHealingMultiplier, GameStatManager.Inst.GameHealingReceivedMultiplier);
        }
    }

    public void InstantSaleUnit(Unit unit)
    {
        InGameUIManager.Inst.unitInfoUI.ApplySaleReward(unit);
        unit.UnitDeActive();
    }

    IEnumerator ParasiticUnitCoroutine(float damage, float duration, float interval)
    {
        GameObject effects = EffectManager.Inst.Get(EffectManager.PrefabType.ParasiticEffect, unit.transform.position);
        ObjectLifecycleManager effectLifeCycle = effects.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(unit.debuffPrefabParent);
        effectLifeCycle.GetComponent<ObjectLifecycleManager>().Init(ObjectLifecycleManager.ObjectType.Effect);
        effects.transform.localPosition = new Vector3(0, 0.3f, 0);

        IsParasitic = true;

        float elapsed = 0;
        ParasiticRemainingTime = duration;

        while (elapsed < duration)
        {
            unit.OnDamage(damage);
            if (unit.health <= 0)
            {
                effectLifeCycle.GetComponent<ObjectLifecycleManager>().DeActive();


                List<Unit> availableUnits = new List<Unit>();

                foreach (var fieldUnit in FieldManager.Inst.fieldUnits)
                {
                    if (fieldUnit != null && !fieldUnit.GetComponent<UnitEvent>().IsParasitic)
                    {
                        availableUnits.Add(fieldUnit.GetComponent<Unit>());
                    }
                }

                if (availableUnits.Count > 0)
                {
                    Unit selectedUnit = availableUnits[Random.Range(0, availableUnits.Count)];
                    UnitEvent unitEvent = selectedUnit.GetComponent<UnitEvent>();
                    unitEvent.ApplyParasiticUnit(damage, duration, interval);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill0);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill1);
                    // AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXSkill3);
                }

            }
            elapsed += interval;
            ParasiticRemainingTime -= interval;
            yield return new WaitForSeconds(interval);
        }

        IsParasitic = false;
        effectLifeCycle.DeActive();
    }

    IEnumerator SyntaxErrorUnitCoroutine(float hitChanceDecrease, float damageDecrease, float duration)
    {
        SyntaxErrorRemainingTime = duration;

        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.SyntaxErrorEffect, unit.transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(unit.debuffPrefabParent);
        effectLifeCycle.GetComponent<ObjectLifecycleManager>().Init(ObjectLifecycleManager.ObjectType.Effect);
        effect.transform.localPosition = new Vector3(0, 0.3f, 0);

        ApplySyntaxErrorDebuff(hitChanceDecrease, damageDecrease);

        while (SyntaxErrorRemainingTime > 0f)
        {
            SyntaxErrorRemainingTime -= Time.deltaTime;
            yield return null;
        }

        RemoveSyntaxErrorDebuff();

        effectLifeCycle.DeActive();
    }

    void ApplySyntaxErrorDebuff(float hitChanceDecrease, float damageDecrease)
    {
        this.hitChanceDecrease = hitChanceDecrease;
        this.damageDecrease = damageDecrease;

        unit.BaseHitChance *= hitChanceDecrease;
        unit.BaseDamage *= damageDecrease;

        IsSyntaxError = true;

        unit.RecalculateStats();
    }

    void RemoveSyntaxErrorDebuff()
    {
        unit.BaseHitChance /= hitChanceDecrease;
        unit.BaseDamage /= damageDecrease;
        IsSyntaxError = false;

        hitChanceDecrease = 0;
        damageDecrease = 0;

        unit.RecalculateStats();
    }

    IEnumerator DurationHealRoutine(float healAmount, float healingMultiplier, float healingReceivedMultiplier, float time, float tick, UnitData owner = null)
    {
        ObjectLifecycleManager effect = EffectManager.Inst.Get(EffectManager.PrefabType.DurationHealEffect, transform.position).GetComponent<ObjectLifecycleManager>();
        effect.SetParent(unit.buffPrefabParent);
        effect.Init(ObjectLifecycleManager.ObjectType.Effect);
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            ApplyGetHeal(healAmount, healingMultiplier, healingReceivedMultiplier, owner);
            yield return new WaitForSeconds(tick);

            elapsedTime += tick;
        }

        effect.DeActive();
    }

    IEnumerator SilencedUnitCoroutine(float duration)
    {
        SilencedRemainingTime = duration;
        IsSilenced = true;
        ObjectLifecycleManager effect = EffectManager.Inst.Get(EffectManager.PrefabType.SlienceEffect, transform.position).GetComponent<ObjectLifecycleManager>();
        effect.SetParent(unit.debuffPrefabParent);
        effect.Init(ObjectLifecycleManager.ObjectType.Effect);

        if (unit.unitData.type == Type.Support && unit.unitData.unitSupportType == UnitSupportType.Mult)
        {
            unit.unitSupportBuffController.supportBuffRangeCircle.SetActive(false);
            unit.unitSupportBuffController.ResetBuffFieldSlot();
            unit.unitSupportBuffController.DisableMultSupportBuffEffect();
        }

        while (SilencedRemainingTime > 0f)
        {
            SilencedRemainingTime -= Time.deltaTime;
            yield return null;
        }

        effect.DeActive();
        IsSilenced = false;

        if (unit.unitData.type == Type.Support && unit.unitData.unitSupportType == UnitSupportType.Mult)
        {
            unit.unitSupportBuffController.SetSupportRangeCircle(true);
            unit.unitSupportBuffController.ApplySupportBuffToNearbyFields();
        }
    }

    public void ClearBuff(Unit supportUnit, Unit targetUnit)
    {
        if (supportUnit.unitSkill is IResetSingleSupportSkill resetSupportSkill)
        {
            Debug.Log("버프 헤제" + supportUnit + targetUnit, gameObject);
            resetSupportSkill.ResetSupport(supportUnit, targetUnit);
        }
    }

    void EventFloatingText(string text, float size, Color color)
    {
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-0.5f, 0.5f),
            UnityEngine.Random.Range(-0.5f, 0.5f),
            0f
        );
        Vector3 spawnPos = transform.position + randomOffset;

        FloatingText floatingText = PoolManager.Inst
            .Get(PoolManager.PrefabType.FloatingText, spawnPos)
            .GetComponent<FloatingText>();

        floatingText.InitializeText(text, size, color);
    }


    public void HandleBuffClear()
    {
        GameObject mergeUnit = unit.unitDrag.mergeUnit;

        if (unit.unitDrag.mergeUnit == null || unit.unitScanner.buffUnit == null)
            return;
        if (unit.GetComponent<Unit>().unitData.type == Type.Support)
        {
            Debug.Log("서폿", gameObject);

            if (unit.isBuffed && mergeUnit.GetComponent<Unit>().isBuffed)
            {
                Debug.LogWarning("모든 버프 유닛 제거");
                unit.unitEvent.ClearBuff(unit, unit.unitScanner.buffUnit.GetComponent<Unit>());
                unit.unitEvent.ClearBuff(mergeUnit.GetComponent<Unit>(), mergeUnit.GetComponent<Unit>().unitScanner.buffUnit.GetComponent<Unit>());
            }
            else if (unit.isBuffed)
            {
                Debug.LogWarning("버프 유닛 제거");
                unit.unitEvent.ClearBuff(unit, unit.unitScanner.buffUnit.GetComponent<Unit>());
            }
            else if (mergeUnit.GetComponent<Unit>().isBuffed)
            {
                Debug.LogWarning("합친 버프 유닛 제거");
                unit.unitEvent.ClearBuff(mergeUnit.GetComponent<Unit>(), mergeUnit.GetComponent<Unit>().unitScanner.buffUnit.GetComponent<Unit>());
            }
            // 매게 변수로 해당 버프를 주고 있는 서폿을 넣어야함 
        }
        else
        {
            Debug.Log("서폿 아님", gameObject);
            if (unit.isBuffed && mergeUnit.GetComponent<Unit>().isBuffed)
            {
                Debug.LogWarning("모든 버프 유닛 제거");
                unit.unitEvent.ClearBuff(unit.unitScanner.buffUnit.GetComponent<Unit>(), unit);
                unit.unitEvent.ClearBuff(mergeUnit.GetComponent<Unit>().unitScanner.buffUnit.GetComponent<Unit>(), mergeUnit.GetComponent<Unit>());
            }
            else if (unit.isBuffed)
            {
                Debug.LogWarning("버프 유닛 제거");
                unit.unitEvent.ClearBuff(unit.unitScanner.buffUnit.GetComponent<Unit>(), unit);
            }
            else if (mergeUnit.GetComponent<Unit>().isBuffed)
            {
                Debug.LogWarning("합친 버프 유닛 제거");
                unit.unitEvent.ClearBuff(mergeUnit.GetComponent<Unit>().unitScanner.buffUnit.GetComponent<Unit>(), mergeUnit.GetComponent<Unit>());
            }
        }
    }

    public void ActivateDebuffClear()
    {
        IsParasitic = false;
        IsSilenced = false;
        IsSyntaxError = false;

        ParasiticRemainingTime = 0;
        SyntaxErrorRemainingTime = 0;
        SilencedRemainingTime = 0;

        if (parasiticCoroutine != null)
        {
            StopCoroutine(parasiticCoroutine);
            parasiticCoroutine = null;
        }

        if (syntaxErrorCoroutine != null)
        {
            RemoveSyntaxErrorDebuff();
            StopCoroutine(syntaxErrorCoroutine);
            syntaxErrorCoroutine = null;
        }

        if (silencedCoroutine != null)
        {
            StopCoroutine(silencedCoroutine);
            silencedCoroutine = null;
        }

        if (unit.debuffPrefabParent != null)
        {
            List<ObjectLifecycleManager> lifecycleManagers = new List<ObjectLifecycleManager>();
            for (int i = 0; i < unit.debuffPrefabParent.childCount; i++)
            {
                Transform child = unit.debuffPrefabParent.GetChild(i);
                ObjectLifecycleManager lifecycleManager = child.GetComponent<ObjectLifecycleManager>();
                if (lifecycleManager != null)
                {
                    lifecycleManagers.Add(lifecycleManager);
                }
            }

            foreach (var lifecycleManager in lifecycleManagers)
            {
                lifecycleManager.DeActive();
            }
        }
    }

    public void ResetUnitEvent()
    {
        hitChanceDecrease = 1;
        damageDecrease = 1;

        ParasiticRemainingTime = 0;
        SyntaxErrorRemainingTime = 0;
        SilencedRemainingTime = 0;
        InvulnerableRemainingTime = 0;

        //specialShield.SetActive(false);
        IsUnitLocked = false;
        IsParasitic = false;
        IsSyntaxError = false;
        IsSilenced = false;
        IsInvulnerable = false;
        IsMergeRandomized = false;
        IsIgnored = false;
        specialShieldCount = 0;

        specialShield = null;
        invulnerableShield = null;

        invulnerableCount = 0;

        parasiticCoroutine = null;
        syntaxErrorCoroutine = null;
        silencedCoroutine = null;

        unitEventHandler = null;
        OnStatChanged = null;
        OnCriticalHit = null;
        OnAttackHit = null;
        OnBuffChanged = null;
        OnDeath = null;

        onUnitPlacedAction = null;
        onUnitRemovedAction = null;

        if (unit.unitData.type == Type.Support && unit.unitData.unitSupportType == UnitSupportType.Mult)
        {
            onUnitPlacedAction = (placedUnit) =>
            {
                if (unit.unitSkill is ISupportApplyOnce supportAttackOnce)
                {
                    supportAttackOnce.ApplySupportApplyOnce(placedUnit);
                }
            };

            onUnitRemovedAction = (placedUnit) =>
            {
                if (unit.unitSkill is IResetMultSupportSkill resetMultSupportSkill)
                {
                    resetMultSupportSkill.ResetSupport(placedUnit);
                }
            };
        }


        ObjectLifecycleManager[] buffObjs = unit.buffPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager buffObj in buffObjs)
        {
            if (buffObj != null)
                buffObj.DeActive();
        }

        ObjectLifecycleManager[] debuffObjs = unit.debuffPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager debuffObj in debuffObjs)
        {
            if (debuffObj != null)
                debuffObj.DeActive();
        }

        ObjectLifecycleManager[] weaponObjs = unit.unitWeaponPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager weaponObj in weaponObjs)
        {
            if (weaponObj != null)
                weaponObj.DeActive();
        }

        StopAllCoroutines();
    }
}

