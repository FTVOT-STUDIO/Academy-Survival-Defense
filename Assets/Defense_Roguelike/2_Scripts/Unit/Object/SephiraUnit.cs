using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using VInspector;
using UnityEngine.UI;

public class SephiraUnit : MonoBehaviour
{
    [Tab("Core State")]
    [Header("# Core State")]
    public UnitData unitData;
    [HideInInspector] public ScriptableObject skillSO;
    public ISephiraSkill unitSkill;
    public float abllityApplyTime;
    public double damageAmout;
    public float skillAmount;
    public float defenseAmount;
    public float criticalPer;
    public float criticalDamage;

    public float abllityCount;
    public float maxAbllityCount;

    public float maxHealth;
    public float health;
    public float maxShield;
    public float shield;
    public float attackSpeed;
    public float attackTime;
    public float range;
    public float hitChance;
    public float skillTriggerChance;
    public float areaDamage;
    public float singleTargetDamage;
    public int killCount;
    public int hitCount;
    public float defensePenetration;
    public float attributeResistancePenetration;
    public float attributeDamageMultiplier;

    public float healingMultiplier = 1f;
    public float healingReceivedMultiplier = 1f;
    public float shieldMultiplier = 1f;
    public float shieldReceivedMultiplier = 1f;

    public float normalAttackMultiplier;
    public float skillMultiplier;
    public float abilityMultiplier;
    [Tab("Base State")]
    public float baseDamage;
    public float baseSkillValue;
    public float baseMaxHealth;
    public float baseMaxShield;
    public float baseDefense;
    public float baseCriticalPer;
    public float baseCriticalDamage;
    public float baseAttackSpeed;
    public float baseAbilityCount;
    public float baseHitChance;
    public float basesSkillTriggerChance;
    public float baseAreaDamage;
    public float baseSingleTargetDamage;
    public float baseDefensePenetration;
    public float baseAttributeResistancePenetration;
    public float baseAttributeDamageMultiplier;

    public float baseHealingMultiplier;
    public float baseHealingReceivedMultiplier;

    public float baseShieldMultiplier;
    public float baseShieldReceivedMultiplier;

    public float baseNormalAttackMultiplier;
    public float baseSkillMultiplier;
    public float baseAbilityMultiplier;

    [Tab("Target State")]
    [Header("# Targets")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Collider2D[] targets;
    public Transform nearestTarget;
    public Transform lowestHealthTarget;
    [HideInInspector] public Transform lastNearestTarget;
    [HideInInspector] public Transform lastLowestHealthTarget;
    [HideInInspector] public int sameTargetHitCount;
    [Tab("Info State")]
    [Header("# Info")]
    public UnitInfo unitInfo;
    [HideInInspector] public SephiraUnitEvent sephiraUnitEvent;
    bool isPresed;
    public bool isSkillActive;
    [HideInInspector] public bool isClicked;
    [HideInInspector] public bool isHit;
    private bool isInvulnerable;

    public SpriteRenderer sp;
    [HideInInspector] public Animator anim;
    public HpBarColorController hpBarColorController;
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider abilitySlider;
    [SerializeField] Slider shieldSlider;
    public GameObject visualGroupObject;
    public Transform buffPrefabParent;
    public Transform debuffPrefabParent;
    public Transform weaponPrefabParent;

    public void Init()
    {
        if (gameObject == GetComponentInParent<FieldManager>().selectUnit)
        {
            unitInfo.ToggleUnitUI(false, gameObject);
            isClicked = false;
        }
        skillSO = unitData.unitSkill;
        if (skillSO is ISephiraSkillBase skillBase)
        {
            unitSkill = skillBase.GenerateRuntimeSkill();
            unitSkill.Initialize(this, this);
        }

        abllityApplyTime = unitData.abllityApplyTime;
        hitChance = 100;
        hitCount = 0;
        killCount = 0;
        maxAbllityCount = DeckManager.Inst.curDeckSephiraData.coolTime;
        baseDamage = 1;
        baseSkillValue = 1;
        baseDefense = 1;
        baseMaxHealth = 1;
        baseMaxShield = 1;
        baseAbilityCount = 1;
        baseCriticalPer = 1;
        baseCriticalDamage = 1;
        baseAttackSpeed = 1;
        baseHitChance = 1;
        basesSkillTriggerChance = 0;
        baseAreaDamage = 1;
        baseSingleTargetDamage = 1;

        baseDefensePenetration = 0;
        baseAttributeResistancePenetration = 0;
        baseAttributeDamageMultiplier = 1;

        baseHealingMultiplier = 1;
        baseHealingReceivedMultiplier = 1;
        baseShieldMultiplier = 1;
        baseShieldReceivedMultiplier = 1;

        baseNormalAttackMultiplier = 1;
        baseSkillMultiplier = 1;
        baseAbilityMultiplier = 1;

        sameTargetHitCount = 0;

        lowestHealthTarget = null;
        nearestTarget = null;
        lastLowestHealthTarget = null;
        lastNearestTarget = null;

        damageAmout = unitData.damageAmount * GameStatManager.Inst.GameDamage;
        skillAmount = unitData.skillAmount * GameStatManager.Inst.GameSkillAmount;
        defenseAmount = unitData.defenseAmount * GameStatManager.Inst.GameDefense;

        criticalPer = unitData.criticalPer * GameStatManager.Inst.GameCriticalPer;
        criticalDamage = unitData.criticalDamage * GameStatManager.Inst.GameCriticalDamage;

        attackSpeed = unitData.attackSpeed * GameStatManager.Inst.GameAttackSpeed;

        range = unitData.GetRange();

        maxAbllityCount = unitData.coolTime * GameStatManager.Inst.GameMaxAbilityCount;

        maxHealth = unitData.maxHealth;
        health = maxHealth;

        maxShield = (unitData.maxShield * GameStatManager.Inst.GameMaxShield) + maxHealth;
        shield = 0;

        skillTriggerChance = 0;
        areaDamage = 1;
        singleTargetDamage = 1;
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

        anim.runtimeAnimatorController = unitData.animController;

        UpdateHealthBar();
        UpdateShieldBar();
        UpdateAbilityBar();

        switch (unitData.type)
        {
            case Type.Healer:
                targetLayer = LayerMask.GetMask("Unit");
                break;
            default:
                targetLayer = LayerMask.GetMask("Enemy Sephira Detction");
                break;
        }

        if (unitSkill is IOnEnablePassive sephiraOnEnablePassive)
        {
            sephiraOnEnablePassive.ApplyOnEnablePassive(this);
        }

        if (unitSkill is IOnEnableSkill sephiraOnEnableSkill)
        {
            sephiraOnEnableSkill.ApplyOnEnableSkill(this);
        }
        GameStatManager.Inst.MarkStatsDirty();
    }

    public void TransInit(UnitData transData)
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
        Init();
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        unitInfo = GetComponentInChildren<UnitInfo>();

        sephiraUnitEvent = GetComponent<SephiraUnitEvent>();
    }

    void OnEnable()
    {
        GameStatManager.Inst.OnStatsChanged += UpdateStats;
    }

    void OnDisable()
    {
        GameStatManager.Inst.OnStatsChanged -= UpdateStats;
    }

    public void SetUp()
    {
        unitData = DeckManager.Inst.currentInGameDeck.sephiraUnit;
        Init();
    }

    public void MouseDown()
    {
        if (GameManager.Inst.isPause || !GameManager.Inst.isGameStart || InGameUIManager.Inst.gameStatePanel.activeSelf)
            return;

        if (Time.timeScale == 0)
            return;

        if (isPresed)
            return;

        isPresed = true;

        if (FieldManager.Inst.selectUnit != null)
            FieldManager.Inst.lastUnit = FieldManager.Inst.selectUnit;

        FieldManager.Inst.selectUnit = gameObject;
        unitInfo.SetAttackRange(true);
        unitInfo.OnToggleChangeTarget();

        isClicked = !isClicked;

        anim.SetTrigger("Attack");
    }

    public void MouseUp()
    {
        if (GameManager.Inst.isPause || !GameManager.Inst.isGameStart || InGameUIManager.Inst.gameStatePanel.activeSelf)
            return;

        isPresed = false;

        unitInfo.ToggleUnitUI(isClicked, gameObject);

    }

    void Update()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause)
            return;

        attackTime = Mathf.Min(attackTime + Time.deltaTime, attackSpeed);

        if (attackTime >= attackSpeed && (GetNearestTarget() != null || GetLowestHealthTarget() != null))
        {
            float randomValue = Random.Range(0f, 100f);

            if (randomValue <= hitChance)
                UnitAttack();

            attackTime = 0;
        }

        if (unitSkill is IUpdatePassive updatePassive)
        {
            updatePassive.ApplyUpdatePassive(this);
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause)
            return;

        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        targets = Physics2D.OverlapCircleAll(rayOrigin, range, targetLayer);

        if (unitData.type == Type.Healer)
        {

            lastLowestHealthTarget = lowestHealthTarget;
            lowestHealthTarget = GetHealTarget();
        }
        else
        {
            lastNearestTarget = nearestTarget;

            nearestTarget = GetTarget();
        }
    }

    public void RecalculateStats()
    {
        float damageMultiplier = (unitSkill is IDamageModifier dmgMod) ? dmgMod.GetDamageMultiplier() : 1f;
        float skillMultiplier = (unitSkill is ISkillModifier sklMod) ? sklMod.GetSkillMultiplier() : 1f;
        float defenseMultiplier = (unitSkill is IDefenseModifier defMod) ? defMod.GetDefenseMultiplier() : 1f;
        float criticalPerMultiplier = (unitSkill is ICriticalPerModifier cpmMod) ? cpmMod.GetCriticalPerMultiplier() : 1f;
        float criticalDamageMultiplier = (unitSkill is ICriticalDamageModifier cdmMod) ? cdmMod.GetCriticalDamageMultiplier() : 1f;

        float coinMultiplier = Mathf.Max(GameStatManager.Inst.CoinDamage * GameManager.Inst.coin / 100, 1.0f);

        maxHealth = Mathf.Max(unitData.maxHealth * baseMaxHealth, 0.0001f);

        damageAmout = Math.Max(
                   ((unitData.damageAmount * baseDamage * GameStatManager.Inst.GameDamage) + coinMultiplier) * damageMultiplier,
                   0);

        skillAmount = Mathf.Max(unitData.skillAmount * baseSkillValue * GameStatManager.Inst.GameSkillAmount * skillMultiplier, 0.0001f);
        defenseAmount = Mathf.Clamp(unitData.defenseAmount * baseDefense * GameStatManager.Inst.GameDefense * defenseMultiplier, 1, 90);
        criticalPer = Mathf.Max(unitData.criticalPer * baseCriticalPer * GameStatManager.Inst.GameCriticalPer * criticalPerMultiplier, 0);
        criticalDamage = Mathf.Max(unitData.criticalDamage * baseCriticalDamage * GameStatManager.Inst.GameCriticalDamage * criticalDamageMultiplier, 1);
        attackSpeed = Mathf.Max(unitData.attackSpeed / baseAttackSpeed / GameStatManager.Inst.GameAttackSpeed, 0.03f);
        hitChance = Mathf.Clamp(hitChance * baseHitChance, 0, 100);
        skillTriggerChance = Mathf.Clamp(basesSkillTriggerChance + GameStatManager.Inst.GameSkillTriggerChance, 0, 1);
        defensePenetration = Mathf.Clamp
                (baseDefensePenetration + GameStatManager.Inst.GameDefensePenetration, 0, 1);
        maxAbllityCount = unitData.coolTime * baseAbilityCount * GameStatManager.Inst.GameMaxAbilityCount;
        attributeResistancePenetration = Mathf.Clamp(baseAttributeResistancePenetration + GameStatManager.Inst.GameAttributeResistancePenetration, 0, 1);

        areaDamage = Mathf.Max(
    baseAreaDamage * GameStatManager.Inst.GameAreaDamage,
    0.0001f);

        singleTargetDamage = Mathf.Max(
            baseSingleTargetDamage * GameStatManager.Inst.GameSingleTargetDamagefloat,
            0.0001f);

        AttributeDatabase attributeDatabase = AttributeDatabase.Inst;

        attributeDamageMultiplier = Mathf.Max(
         baseAttributeDamageMultiplier * GameStatManager.Inst.GameAttributeDamageMultiplier * attributeDatabase.GetAttributeMultiplier(unitData.attribute.attributeType, GameStatManager.Inst.gameAttributeDamageBonus),
          0);

        healingMultiplier = Mathf.Max(
            baseHealingMultiplier * GameStatManager.Inst.GameHealingMultiplier,
            0.0001f);

        healingReceivedMultiplier = Mathf.Max(
            baseHealingReceivedMultiplier * GameStatManager.Inst.GameHealingReceivedMultiplier,
            0.0001f);

        shieldMultiplier = Mathf.Max(
            baseShieldMultiplier * GameStatManager.Inst.GameShieldMultiplier,
            0.0001f);

        shieldReceivedMultiplier = Mathf.Max(
            baseShieldReceivedMultiplier * GameStatManager.Inst.GameShieldReceivedMultiplier,
            0.0001f);

        normalAttackMultiplier = Mathf.Max(baseNormalAttackMultiplier * GameStatManager.Inst.GameNormalAttackMultiplier, 0);
        skillMultiplier = Mathf.Max(baseSkillMultiplier * GameStatManager.Inst.GameSkillMultiplier, 0);
        abilityMultiplier = Mathf.Max(baseAbilityMultiplier * GameStatManager.Inst.GameAbilityMultiplier, 0);


        sephiraUnitEvent.OnStatChanged?.Invoke();
        UpdateAbilityBar();
        UpdateHealthBar();
        UpdateShieldBar();
    }

    private void UpdateStats()
    {
        RecalculateStats();
    }

    public void UnitAttack()
    {
        if (unitSkill is IAttackPassive attackPassive)
        {
            attackPassive.ApplyAttackPassive(this);
        }

        if (unitSkill is IAttackSkill attackSkill)
        {
            attackSkill.ApplyIAttackSkill(this);
        }

        switch (unitData.type)
        {
            case Type.Ranged:
                Enemy enemy = GetNearestTarget();

                SetDir(enemy);

                if (nearestTarget == lastNearestTarget)
                    sameTargetHitCount++;
                else
                    sameTargetHitCount = 0;

                break;
            case Type.Support:
                break;
            case Type.Healer:

                Unit lowHealthUnit = lowestHealthTarget.transform.GetComponent<Unit>();

                if (lowHealthUnit.health < lowHealthUnit.maxHealth && lowHealthUnit.isLive)
                {
                    Debug.LogWarning("힐링 능력");
                    sp.flipX = !lowHealthUnit.sp.flipX;

                    lowHealthUnit.unitEvent.ApplyGetHeal((float)damageAmout, healingMultiplier, lowHealthUnit.healingReceivedMultiplier, unitData);

                    if (lowestHealthTarget == lastLowestHealthTarget)
                        sameTargetHitCount++;
                    else
                        sameTargetHitCount = 0;
                }
                break;
        }
        hitCount++;
        if (hitCount >= int.MaxValue - 1000)
            hitCount = 0;

        anim.SetTrigger("Attack");
        InGameEventManager.Inst.SephiraAttackEvent?.Invoke(this);
    }


    Transform GetTarget()
    {
        Transform result = null;
        float minSqrDistance = float.MaxValue;
        Vector3 myPosition = transform.position;

        for (int i = 0; i < targets.Length; i++)
        {
            Collider2D col = targets[i];
            if (col == null) continue;

            Transform targetTransform = col.transform;
            Vector3 diff = targetTransform.position - myPosition;
            float sqrDistance = diff.sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                result = targetTransform.parent;
            }
        }
        return result;
    }



    Transform GetHealTarget()
    {
        Transform result = null;
        float lowestHealth = float.MaxValue;

        for (int i = 0; i < targets.Length; i++)
        {
            Collider2D col = targets[i];
            Unit unit = col.GetComponentInParent<Unit>();
            if (unit == null || !unit.isLive) continue;
            float currentHealth = unit.health;

            if (currentHealth >= unit.maxHealth || currentHealth >= lowestHealth)
                continue;

            lowestHealth = currentHealth;
            result = unit.transform;
        }
        return result;
    }

    public void SetDir(Enemy enemy)
    {
        if (enemy == null)
            return;
        Vector3 myPos = transform.position;
        Vector3 enemyPos = enemy.transform.position;
        if (myPos.x < enemyPos.x)
        {
            sp.flipX = true;
        }
        else
        {
            sp.flipX = false;
        }
    }

    public void Ondamage(float damage, Enemy enemy = null)
    {
        if (isInvulnerable)
            return;

        StartCoroutine(DamageCoroutine(damage, enemy));
    }

    public IEnumerator DamageCoroutine(float damage, Enemy enemy = null)
    {
        isInvulnerable = true;
        damage *= 1 - defenseAmount / 100f;
        if (shield > 0)
            damage *= 0.5f;

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
            UnitCombatStatManager.Inst.AddTakenDamage(unitData, damage * (1 - defenseAmount / 100f));
            if (health <= 0)
            {
                GameManager.Inst.isDefeat = true;
                InGameUIManager.Inst.ResultUIPanel.SetActive(true);
            }
        }


        isHit = true;
        Color color = sp.color;
        float alpha = color.a;
        sp.color = new Color(1f, 1f, 1f, 0.5f);
        GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0, 0, 0.05f), 0.3f);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.damaged);
        UpdateHealthBar();
        UpdateShieldBar();
        yield return new WaitForSecondsRealtime(1f);
        sp.color = new Color(1f, 1f, 1f, alpha);
        isHit = false;
        isInvulnerable = false;
    }

    public void UpdateHealthBar()
    {
        hpSlider.value = FieldManager.Inst.fieldSephira.health / FieldManager.Inst.fieldSephira.maxHealth;
        InGameUIManager.Inst.sephiraHealthText.text = health.ToString("F0") + "/" + maxHealth.ToString("F0");
        hpBarColorController.UpdateHealthBarColor(health / maxHealth);
    }
    public void UpdateShieldBar()
    {
        shieldSlider.value = FieldManager.Inst.fieldSephira.shield / FieldManager.Inst.fieldSephira.maxShield;
    }

    public void UpdateAbilityBar()
    {
        if (isSkillActive)
            return;
        abilitySlider.value = abllityCount / maxAbllityCount;
        InGameUIManager.Inst.abilityCountText.text = (abilitySlider.value * 100).ToString("F0") + "%";
        InGameUIManager.Inst.abilityCountAnim.SetTrigger("POP");
    }
    public void OnAbllity()
    {
        if (abllityCount >= maxAbllityCount && !isSkillActive)
        {
            StartCoroutine(DecreaseAbilitySlider());
            abllityCount = 0;
            GameFlowManager.Inst.ChangeAbilityCount(1);

            if (unitSkill is ICoolTimeSkill coolTimeSkill)
            {
                coolTimeSkill.ApplyCoolTimeSkill(this);
            }

            if (unitSkill is ICoolTimePassive coolTimePassive)
            {
                coolTimePassive.ApplyPassive(this);
            }

            InGameUIManager.Inst.abilityReadyEffect.SetActive(false);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.AbiltyOn);
        }
    }

    IEnumerator DecreaseAbilitySlider()
    {
        isSkillActive = true;
        float duration = abllityApplyTime;
        float decreasePerSecond = InGameUIManager.Inst.abilitySlider.maxValue / duration;
        float elapsed = 0f;

        while (elapsed < duration && isSkillActive)
        {
            InGameUIManager.Inst.abilitySlider.value -= decreasePerSecond * Time.deltaTime;
            InGameUIManager.Inst.abilityCountText.text = (InGameUIManager.Inst.abilitySlider.value * 100).ToString("F0") + "%";

            elapsed += Time.deltaTime;
            yield return null;
        }
        isSkillActive = false;
        sephiraUnitEvent.isAbilityReady = false;
        InGameUIManager.Inst.abilitySlider.value = 0;
        InGameUIManager.Inst.abilityCountText.text = "0%";
    }


    public double TakeDamage(AttributeData.AttributeType attributeType, double baseDamage, float criticalPer, float criticalDamage, float singleTargetDamage, float defensePenetration, float attributeResistanceIgnore, float attributeDamageMultiplier, UnitUtility.AttackType attackType, float attackTypeMultiplier, Enemy enemy,
     UnitUtility.DamageType damageType = UnitUtility.DamageType.Fixed, float ratio = 0,
     EffectManager.PrefabType hitEffect = EffectManager.PrefabType.EnemyHitEffect,
     bool isPlaySfx = true, AudioManager.Sfx hitSound = AudioManager.Sfx.EnemyHit)
    {
        double finalDamage = baseDamage;
        finalDamage *= singleTargetDamage;

        finalDamage *= attackTypeMultiplier;
        finalDamage += UnitUtility.CalculateDamage(damageType, damageAmout, ratio, enemy);

        if (damageType != UnitUtility.DamageType.Fixed)
            criticalPer = 0;

        enemy.OnDamage(attributeType, finalDamage, criticalPer, criticalDamage, defensePenetration, attributeResistanceIgnore, attributeDamageMultiplier, attackType, gameObject, () =>
        {
            //unit.unitEvent.ApplyKillEffect();
            if (unitSkill is IKillEvent sephiraKillEvent)
            {
                sephiraKillEvent.ApplyKillEvent(this);
            }
        });

        EffectManager.Inst.Get(hitEffect, enemy.transform.position);

        if (isPlaySfx)
            AudioManager.Inst.PlaySfx(hitSound);

        return finalDamage;
    }

    public Enemy GetNearestTarget()
    {
        if (nearestTarget == null) return null;

        var enemy = nearestTarget.GetComponent<Enemy>();
        if (enemy == null || !enemy.isLive) return null;

        return enemy;
    }

    public Unit GetLowestHealthTarget()
    {
        if (lowestHealthTarget == null) return null;

        var Unit = lowestHealthTarget.GetComponent<Unit>();
        if (Unit == null || !Unit.isLive) return null;

        return Unit;
    }
}
