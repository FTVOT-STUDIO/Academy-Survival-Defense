using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Unit : MonoBehaviour, IUnitBaseStat
{
    [Tab("Core State")]
    [Header("# Core State")]
    public UnitData unitData;
    public ScriptableObject skillSO;
    public IUnitSkill unitSkill;
    public int starLevel;
    public double damageAmout;
    public float skillAmount;
    public float defenseAmount;
    public float criticalPer;
    public float criticalDamage;

    public float health;
    public float maxHealth;
    public float shield;
    public float maxShield;
    public float attackSpeed;
    public float range;
    public float coolTime;
    public float hitChance;
    public float skillTriggerChance;
    public float areaDamage;
    public float singleTargetDamage;
    public float evasionValue;
    public float defensePenetration;
    public float attributeResistancePenetration;
    public float attributeDamageMultiplier;

    public float healingMultiplier;
    public float healingReceivedMultiplier;

    public float shieldReceivedMultiplier;
    public float shieldMultiplier;

    public float normalAttackMultiplier;
    public float skillMultiplier;
    public float abilityMultiplier;

    public float manaReceivedMultiplier;

    public float damageTakenIncrease;

    [Tab("Base State")]
    public float BaseDamage { get; set; }
    public float BaseSkillValue { get; set; }
    public float BaseDefense { get; set; }
    public float BaseMaxHealth { get; set; }
    public float BaseMaxShield { get; set; }
    public float BaseCoolTime { get; set; }
    public float BaseRange { get; set; }
    public float BaseAttackSpeed { get; set; }

    public float BaseCriticalPer { get; set; }
    public float BaseCriticalDamage { get; set; }
    public float BaseHitChance { get; set; }
    public float BaseSkillTriggerChance { get; set; }
    public float BaseAreaDamage { get; set; }
    public float BaseSingleTargetDamage { get; set; }
    public float BaseEvasion { get; set; }
    public float BaseDefensePenetration { get; set; }
    public float BaseAttributeResistancePenetration { get; set; }
    public float BaseAttributeDamageMultiplier { get; set; }

    public float BaseHealingMultiplier { get; set; }
    public float BaseHealingReceivedMultiplier { get; set; }
    public float BaseShieldMultiplier { get; set; }
    public float BaseShieldReceivedMultiplier { get; set; }

    public float BaseNormalAttackMultiplier { get; set; }
    public float BaseSkillMultiplier { get; set; }
    public float BaseAbilityMultiplier { get; set; }

    public float BaseManaReceivedMultiplier { get; set; }

    public float BaseDamageTakenIncrease { get; set; }

    [Tab("Unit State")]
    [Header("# Unit State")]
    [Space]
    public bool isLive;
    public bool isBuffed;
    public bool isTrans = false;
    public bool isDirty = false;
    [Space]
    [Header("# Hp & Shield")]
    [SerializeField] GameObject hpBar;
    [SerializeField] GameObject manaBar;
    [SerializeField] GameObject shieldBar;
    [SerializeField] GameObject attackSpeedBar;
    public HpBarColorController hpBarColorController;
    [Header("# State Groups")]
    public GameObject visualGroup;
    public GameObject buffIcon;
    public Transform buffPrefabParent;
    public Transform debuffPrefabParent;
    public Transform unitWeaponPrefabParent;
    [SerializeField] Collider2D hitBoxColl;
    [SerializeField] Collider2D inputColl;

    [HideInInspector] public int hitCount;
    [HideInInspector] public UnitScanner unitScanner;
    [HideInInspector] public UnitWeapon unitWeapon;
    [HideInInspector] public UnitLevel unitLevel;
    [HideInInspector] public UnitDrag unitDrag;
    [HideInInspector] public UnitInfo unitInfo;
    [HideInInspector] public UnitEvent unitEvent;
    [HideInInspector] public UnitSupportBuffController unitSupportBuffController;
    [HideInInspector] public bool isHit;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer sp;
    [HideInInspector] public GameObject effect;
    [HideInInspector] public SpriteRenderer shadow;
    [HideInInspector] public TrailRenderer trail;
    [HideInInspector] public MaterialChanger materialChanger;

    void Awake()
    {
        sp = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        unitScanner = GetComponent<UnitScanner>();
        unitWeapon = GetComponent<UnitWeapon>();
        unitDrag = GetComponent<UnitDrag>();
        unitLevel = GetComponent<UnitLevel>();
        unitEvent = GetComponent<UnitEvent>();
        unitInfo = GetComponentInChildren<UnitInfo>(true);
        unitSupportBuffController = GetComponent<UnitSupportBuffController>();
        materialChanger = GetComponentInChildren<MaterialChanger>();
    }

    void OnEnable()
    {
        GameStatManager.Inst.OnStatsChanged += UpdateStats;
        GameStatManager.Inst.OnStatsChanged += RefreshSupportBuffIfDirty;
    }
    void OnDisable()
    {
        GameStatManager.Inst.OnStatsChanged -= UpdateStats;
        GameStatManager.Inst.OnStatsChanged -= RefreshSupportBuffIfDirty;
        ResetUnitState();
    }

    public void Init(int startLevel = 1, bool shouldExecuteBatch = true, bool callOnBeforeTrans = false)
    {
        if (gameObject == FieldManager.Inst.selectUnit)
        {
            unitInfo.ToggleUnitUI(false, gameObject);
            unitDrag.isClicked = false;
        }

        ResetUnitBuff();

        skillSO = unitData.unitSkill;
        if (skillSO is IUnitSkillBase skillBase)
        {
            unitSkill = skillBase.GenerateRuntimeSkill();
            unitSkill.Initialize(this, this);
        }
        starLevel = startLevel;
        isLive = true;

        BaseDamage = 1;
        BaseSkillValue = 1;
        BaseDefense = 1;
        BaseCriticalPer = 1;
        BaseCriticalDamage = 1;
        BaseMaxHealth = 1;
        BaseMaxShield = 1;
        BaseCoolTime = 1;
        BaseRange = 1;
        BaseAttackSpeed = 1;
        BaseHitChance = 1;
        BaseSkillTriggerChance = 0;
        BaseAreaDamage = 1;
        BaseSingleTargetDamage = 1;
        BaseEvasion = 0;
        BaseDefensePenetration = 0;

        BaseAttributeResistancePenetration = 0;
        BaseAttributeDamageMultiplier = 1;

        BaseHealingMultiplier = 1;
        BaseHealingReceivedMultiplier = 1;
        BaseShieldMultiplier = 1;
        BaseShieldReceivedMultiplier = 1;

        BaseNormalAttackMultiplier = 1;
        BaseSkillMultiplier = 1;
        BaseAbilityMultiplier = 1;

        BaseManaReceivedMultiplier = 1;

        BaseDamageTakenIncrease = 1;

        hitCount = 0;
        hitChance = 100;

        damageAmout = unitData.damageAmount * GameStatManager.Inst.GameDamage;
        skillAmount = unitData.skillAmount * GameStatManager.Inst.GameSkillAmount;

        criticalPer = unitData.criticalPer * GameStatManager.Inst.GameCriticalPer;
        criticalDamage = unitData.criticalDamage * GameStatManager.Inst.GameCriticalDamage;

        attackSpeed = unitData.attackSpeed / GameStatManager.Inst.GameAttackSpeed;

        range = unitData.GetRange() * GameStatManager.Inst.GameRange;
        coolTime = unitData.coolTime / GameStatManager.Inst.GameCoolTime;

        maxHealth = unitData.maxHealth * GameStatManager.Inst.GameMaxHealth;
        health = maxHealth;

        maxShield = (unitData.maxShield * GameStatManager.Inst.GameMaxShield) + maxHealth;
        shield = 0;

        skillTriggerChance = 0;
        areaDamage = 1;
        singleTargetDamage = 1;
        evasionValue = 0;
        defensePenetration = 0;
        attributeResistancePenetration = 0;
        attributeDamageMultiplier = 1;

        healingMultiplier = 1;
        healingReceivedMultiplier = 1;
        shieldMultiplier = 1;
        shieldReceivedMultiplier = 1;

        normalAttackMultiplier = 1;
        skillMultiplier = 1;
        abilityMultiplier = 1;

        manaReceivedMultiplier = 1;

        damageTakenIncrease = 1;

        unitWeapon.attackTime = 0;
        unitWeapon.skillTime = 0;

        anim.runtimeAnimatorController = unitData.animController;
        gameObject.name = unitData.name;

        unitDrag.curPos = transform.position;
        unitDrag.curIndex = unitDrag.GetUnitIndex(gameObject);
        sp.transform.localPosition = Vector3.zero;
        sp.transform.localScale = Vector3.one;
        sp.transform.rotation = Quaternion.identity;

        visualGroup.transform.localScale = Vector3.one;

        ResetUnitState();

        switch (unitData.type)
        {
            case Type.Healer:
                unitScanner.targetLayer = LayerMask.GetMask("Unit");
                break;
            case Type.Support:
                unitScanner.targetLayer = LayerMask.GetMask("Unit");
                break;
            default:
                unitScanner.targetLayer = LayerMask.GetMask("Enemy Unit Detction");
                break;
        }
        unitSupportBuffController.UpdateSupportRangeCircle();

        unitLevel.SetLevel(startLevel, !shouldExecuteBatch);

        unitWeapon.DeleteSkillCoroutine();

        if (shouldExecuteBatch)
            FieldManager.Inst.UnitBatch(gameObject, unitDrag.curIndex, true);

        if (unitSkill is IOnEnablePassive onEnablePassive)
        {
            onEnablePassive.ApplyOnEnablePassive(this);
        }
        if (unitSkill is IOnEnableSkill onEnableSkill)
        {
            onEnableSkill.ApplyOnEnableSkill(this);
        }

        if (callOnBeforeTrans && unitSkill is ITriggerBeforeTransform unitBeforeTransform)
        {
            unitBeforeTransform.OnTriggerBeforeTransform();
        }

        RecalculateStats();
    }

    public void TransInit(UnitData transData, int starLevel, bool callOnBeforeTrans = false)
    {
        if (unitSkill is IOnDisablePassive onDisablePassive)
        {
            onDisablePassive.ApplyOnDisablePassive(this);
        }
        if (unitSkill is IOnDisableSkill onDisableSkill)
        {
            onDisableSkill.ApplyOnDisableSkill(this);
        }

        unitData = transData;
        Init(starLevel, false, callOnBeforeTrans);
    }

    public void ResetUnitState()
    {
        isHit = false;
        isTrans = false;
        unitDrag.curUnit = null;
        //unitDrag.exiUnit = null;
        unitDrag.isDragged = false;
        unitDrag.isClicked = false;
        unitDrag.isInputLocked = false;
        unitDrag.isPlaced = true;
        unitDrag.isPressed = false;
        unitDrag.isDragLocked = false;
        unitWeapon.isSkillActive = false;
        unitWeapon.sameTargetHitCount = 0;

        unitWeapon.RemoveAllEventHandler();

        unitEvent.ResetUnitEvent();

        unitScanner.buffUnit = null;
        unitScanner.nearestUnit = null;
        unitScanner.lastNearestUnit = null;
        unitScanner.nearestTarget = null;
        unitScanner.lastNearestTarget = null;
        unitScanner.lowestHealthTarget = null;
        unitScanner.lastLowestHealthTarget = null;
        unitScanner.result = null;
        FieldManager.Inst.ResetFieldState();
    }

    public void ResetUnitBuff()
    {
        if (unitData.type == Type.Support)
        {
            if (unitScanner.buffUnit != null)
            {
                unitEvent.ClearBuff(this, unitScanner.buffUnit.GetComponent<Unit>());
            }
        }
        else
        {
            if (unitScanner.buffUnit != null)
            {
                unitEvent.ClearBuff(unitScanner.buffUnit.GetComponent<Unit>(), this);
            }
        }

        unitSupportBuffController.ResetBuffFieldSlot();
        unitSupportBuffController.DisableMultSupportBuffEffect();
    }

    void Update()
    {
        if (!isLive || unitEvent.IsIgnored)
            return;

        if (unitSkill is IUpdatePassive updatePassive)
        {
            updatePassive.ApplyUpdatePassive(this);
        }
    }

    void LateUpdate()
    {
        if (!isLive || unitEvent.IsIgnored)
            return;


    }

    private void RefreshSupportBuffIfDirty()
    {
        if (unitSkill is MultSupportSkillBase multSupportSkillBase)
        {
            multSupportSkillBase.ApplySupportBuffUpdate(this);
            isDirty = false;
        }

        if (unitSkill is SingleSupportSkillBase singleSupportSkillBase && unitScanner.nearestUnit != null)
        {
            singleSupportSkillBase.ApplySupportBuffUpdate(this, unitScanner.GetCurrentBuffedUnit());
            isDirty = false;
        }
    }

    private void UpdateStats()
    {
        isDirty = true;
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        float coinMultiplier = Mathf.Max(GameStatManager.Inst.CoinDamage * (GameManager.Inst.coin / 100f), 0f);

        float damageMultiplier = (unitSkill is IDamageModifier dmgMod) ? dmgMod.GetDamageMultiplier() : 1f;
        float skillValueMultiplier = (unitSkill is ISkillModifier sklMod) ? sklMod.GetSkillMultiplier() : 1f;
        float defenseMultiplier = (unitSkill is IDefenseModifier defMod) ? defMod.GetDefenseMultiplier() : 1f;
        float healthMultiplier = (unitSkill is IMaxHealthModifier hpMod) ? hpMod.GetMaxHealthMultiplier() : 1f;
        float shieldMultiplier = (unitSkill is IMaxShieldModifier shieldMod) ? shieldMod.GetMaxShieldMultiplier() : 1f;
        float attackSpeedMultiplier = (unitSkill is IAttackSpeedModifier atkSpdMod) ? atkSpdMod.GetAttackSpeedMultiplier() : 1f;
        float coolTimeMultiplier = (unitSkill is ICoolTimeModifier ctMod) ? ctMod.GetCoolTimeMultiplier() : 1f;
        float rangeMultiplier = (unitSkill is IRangeModifier rngMod) ? rngMod.GetRangeMultiplier() : 1f;
        float criticalPerMultiplier = (unitSkill is ICriticalPerModifier cpmMod) ? cpmMod.GetCriticalPerMultiplier() : 1f;
        float criticalDamageMultiplier = (unitSkill is ICriticalDamageModifier cdmMod) ? cdmMod.GetCriticalDamageMultiplier() : 1f;

        damageAmout = Math.Max(
            ((unitData.damageAmount * BaseDamage * GameStatManager.Inst.GameDamage) + coinMultiplier) * damageMultiplier,
            0);

        skillAmount = Mathf.Max(unitData.skillAmount * BaseSkillValue * GameStatManager.Inst.GameSkillAmount * skillMultiplier, 0.0001f);

        defenseAmount = Mathf.Clamp(unitData.defenseAmount * BaseDefense * GameStatManager.Inst.GameDefense * defenseMultiplier, 1, 90);

        maxHealth = Mathf.Max(unitData.maxHealth * BaseMaxHealth * GameStatManager.Inst.GameMaxHealth * healthMultiplier, 0.0001f);
        maxShield = unitData.maxShield * BaseMaxShield * GameStatManager.Inst.GameMaxShield * shieldMultiplier;

        if (health > maxHealth)
        {
            health = maxHealth;

        }
        if (shield > maxShield)
        {
            shield = maxShield;
        }

        attackSpeed = Mathf.Max(unitData.attackSpeed / BaseAttackSpeed / GameStatManager.Inst.GameAttackSpeed / attackSpeedMultiplier, 0.03f);

        coolTime = Mathf.Max(unitData.coolTime / BaseCoolTime / GameStatManager.Inst.GameCoolTime / coolTimeMultiplier, 1);

        range = Mathf.Clamp(BaseRange * unitData.GetRange() * GameStatManager.Inst.GameRange * rangeMultiplier, 0.7f, 8);

        hitChance = Mathf.Clamp(100 * BaseHitChance, 0, 100);
        skillTriggerChance = Mathf.Clamp(BaseSkillTriggerChance + GameStatManager.Inst.GameSkillTriggerChance, 0, 1);

        criticalPer = Mathf.Max(unitData.criticalPer * BaseCriticalPer * GameStatManager.Inst.GameCriticalPer * criticalPerMultiplier, 0);
        criticalDamage = Mathf.Max(unitData.criticalDamage * BaseCriticalDamage * GameStatManager.Inst.GameCriticalDamage * criticalDamageMultiplier, 1);
        evasionValue = Mathf.Clamp(BaseEvasion + GameStatManager.Inst.GameEvasionValue, 0, 1);

        defensePenetration = Mathf.Clamp
        (BaseDefensePenetration + GameStatManager.Inst.GameDefensePenetration, 0, 1);

        attributeResistancePenetration = Mathf.Clamp(BaseAttributeResistancePenetration + GameStatManager.Inst.GameAttributeResistancePenetration, 0, 1);

        normalAttackMultiplier = Mathf.Max(BaseNormalAttackMultiplier * GameStatManager.Inst.GameNormalAttackMultiplier, 0);
        skillMultiplier = Mathf.Max(BaseSkillMultiplier * GameStatManager.Inst.GameSkillMultiplier, 0);
        abilityMultiplier = Mathf.Max(BaseAbilityMultiplier * GameStatManager.Inst.GameAbilityMultiplier, 0);

        manaReceivedMultiplier = Mathf.Max(BaseManaReceivedMultiplier * GameStatManager.Inst.GameManaReceivedMultiplier, 0);

        damageTakenIncrease = Mathf.Max(BaseDamageTakenIncrease * GameStatManager.Inst.GameDamageTakenIncrease, 1);


        areaDamage = Mathf.Max(
    BaseAreaDamage * GameStatManager.Inst.GameAreaDamage,
    0.0001f);

        singleTargetDamage = Mathf.Max(
            BaseSingleTargetDamage * GameStatManager.Inst.GameSingleTargetDamagefloat,
            0.0001f);

        AttributeDatabase attributeDatabase = AttributeDatabase.Inst;

        attributeDamageMultiplier = Mathf.Max(
         BaseAttributeDamageMultiplier * GameStatManager.Inst.GameAttributeDamageMultiplier * attributeDatabase.GetAttributeMultiplier(unitData.attribute.attributeType, GameStatManager.Inst.gameAttributeDamageBonus),
          0);

        healingMultiplier = Mathf.Max(
            BaseHealingMultiplier * GameStatManager.Inst.GameHealingMultiplier,
            0.0001f);

        healingReceivedMultiplier = Mathf.Max(
            BaseHealingReceivedMultiplier * GameStatManager.Inst.GameHealingReceivedMultiplier,
            0.0001f);

        shieldMultiplier = Mathf.Max(
            BaseShieldMultiplier * GameStatManager.Inst.GameShieldMultiplier,
            0.0001f);

        shieldReceivedMultiplier = Mathf.Max(
            BaseShieldReceivedMultiplier * GameStatManager.Inst.GameShieldReceivedMultiplier,
            0.0001f);

        unitEvent.OnStatChanged?.Invoke();

        UpdateHealthBar();
        UpdateShieldBar();
        UpdateManaBar();
        UpdateAttackSpeedBar();
        hpBarColorController.UpdateHealthBarColor(health / maxHealth);
    }

    public void UpdateHealthBar()
    {
        double hpRatioD = maxHealth > 0 ? health / maxHealth : 0.0;
        hpRatioD = Math.Clamp(hpRatioD, 0.0, 1.0);

        hpBar.transform.localScale = new Vector3((float)hpRatioD, 1f, 1f);
    }

    public void UpdateShieldBar()
    {
        double shieldRatioD = maxShield > 0 ? shield / maxShield : 0.0;
        shieldRatioD = Math.Clamp(shieldRatioD, 0.0, 1.0);

        shieldBar.transform.localScale = new Vector3((float)shieldRatioD, 1f, 1f);
    }

    public void UpdateAttackSpeedBar()
    {
        float attackSpeedRatio = 1 - Mathf.Clamp01(unitWeapon.attackTime / attackSpeed);
        attackSpeedBar.transform.localScale = new Vector3(attackSpeedRatio, 0.3f, 1);
    }

    public void UpdateManaBar()
    {
        float manaRatio = Mathf.Clamp01(unitWeapon.skillTime / coolTime);
        manaBar.transform.localScale = new Vector3(manaRatio, 1, 1);
    }

    public void OnDamage(float damage, Enemy enemy = null)
    {
        if (unitEvent.IsInvulnerable || unitEvent.IsIgnored || !isLive)
            return;

        damage *= 1 - defenseAmount / 100f;
        if (shield > 0)
            damage *= 0.5f;

        damage *= damageTakenIncrease;

        if (!InGameEventManager.Inst.isDamageShared)
        {
            StartCoroutine(DamageCoroutine(damage, enemy));
            return;
        }
        List<Unit> shareUnits = FieldManager.Inst.GetFieldBatchUnits();

        float myDamage = damage * InGameEventManager.Inst.myShareRatio;
        StartCoroutine(DamageCoroutine(myDamage, enemy));

        float remainingDamage = damage * (1 - InGameEventManager.Inst.myShareRatio);
        int otherCount = shareUnits.Count - 1;
        if (otherCount <= 0) return;

        float perUnitDamage = remainingDamage / otherCount;

        foreach (var u in shareUnits)
        {
            if (u == this) continue;

            float actualDamage = perUnitDamage;
            if (u.shield > 0)
                actualDamage *= 0.5f;

            u.StartCoroutine(u.DamageCoroutine(actualDamage));
        }
    }
    public IEnumerator DamageCoroutine(float damage, Enemy enemy = null)
    {
        if (unitEvent.IsInvulnerable || unitEvent.IsIgnored)
            yield break;

        if (UnityEngine.Random.value < evasionValue)
        {
            FloatingText floatingText = PoolManager.Inst.Get(PoolManager.PrefabType.FloatingText, transform.position).GetComponent<FloatingText>();
            floatingText.InitializeText("회피", 4, Color.pink);
            yield break;
        }
        if (enemy != null)
            InGameEventManager.Inst.OnHit?.Invoke(enemy);
        if (shield > 0)
        {
            shield = Mathf.Max(shield - damage, 0);
            UnitCombatStatManager.Inst.AddTakenDamage(unitData, damage);
        }
        else
        {
            health -= damage;
            UnitCombatStatManager.Inst.AddTakenDamage(unitData, damage);
            if (health <= 0)
            {
                if (unitSkill is IOnUnitDeathOverride onUnitDeathOverride)
                {
                    onUnitDeathOverride.ApplyOnUnitDeathOverride(this);
                }
                else
                {
                    Dead();
                    yield break;
                }
            }
        }
        isHit = true;
        sp.color = Color.red;
        UpdateHealthBar();
        yield return new WaitForSeconds(0.1f);
        sp.color = Color.white;
        isHit = false;
    }

    public void Dead()
    {
        isLive = false;
        if (unitSkill is IOnDiePassive onDiePassive)
        {
            onDiePassive.ApplyOnDiePassive(this);
        }
        if (unitSkill is IOnDieSkill onDieSkill)
        {
            onDieSkill.ApplyOnDieSkill(this);
        }
        unitEvent.OnDeath?.Invoke();
        UnitDeActive();
        GameFlowManager.Inst.ChangeDeadUnits(1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.UnitDead);
    }

    public void UnitDeActive()
    {
        if (unitEvent.IsIgnored)
            return;

        //unitDrag.MouseUp();
        int curIndex = unitDrag.GetUnitIndex(gameObject);

        if (unitSkill is IOnDisablePassive onDisablePassive)
        {
            onDisablePassive.ApplyOnDisablePassive(this);
        }
        if (unitSkill is IOnDisableSkill onDisableSkill)
        {
            onDisableSkill.ApplyOnDisableSkill(this);
        }
        if (unitSkill is ITriggerAfterTransform unitAfterTransform)
        {
            unitAfterTransform.OnTriggerAfterTransform();
        }

        if (unitSkill is IDeActivePassive deActivePassive)
        {
            deActivePassive.ApplyDeActivePassive(this);
        }

        if (unitSkill is IDeActiveSkill deActiveSkill)
        {
            deActiveSkill.ApplyDeActiveSkill(this);
        }


        FieldManager.Inst.fieldUnits[curIndex] = null;
        FieldManager.Inst.sp[curIndex].color = Color.white;
        ResetUnitBuff();
        ResetUnitState();

        if (gameObject == FieldManager.Inst.selectUnit)
        {
            unitInfo.ToggleUnitUI(false, gameObject);
        }

        gameObject.SetActive(false);
        unitDrag.curIndex = -1;
        unitDrag.curBatchFieldTrigger.RemoveUnit();

        if (unitWeapon.weaponObj != null)
            unitWeapon.weaponObj = null;

        trail.Clear();

        unitSupportBuffController.ResetBuffFieldSlot();
        unitSupportBuffController.DisableMultSupportBuffEffect();
        FieldManager.Inst.ModifySpawnedUnitCount(-1);
        InGameEventManager.Inst.UnitRemovedEvent?.Invoke(this);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.UnitDeActive);
    }

    public void SetUnitClassColor()
    {
        Color unitColor;

        unitColor = unitData.classColorType[unitData.unitClass];

        GameObject particle = EffectManager.Inst.Get(EffectManager.PrefabType.SpawnEffect, transform.position);
        particle.transform.SetParent(visualGroup.transform);
        ParticleSystem.MainModule particleSystem = particle.GetComponent<ParticleSystem>().main;
        particleSystem.startColor = unitColor;

        shadow.color = unitColor;
        trail.startColor = unitColor;
        trail.endColor = unitColor;
    }
}
