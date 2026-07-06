using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Tab("Core")]
    [Header("# Data")]
    public EnemyData enemyData;
    public IEnemyBehavior enemyBehavior;
    public int spawnIndex;
    [HideInInspector] public ScriptableObject enemySkill;
    [HideInInspector] public EnemyData.EnemyType enemyType;
    [HideInInspector] public EnemyEvent enemyEvent;
    [HideInInspector] public EnemyMove enemyMove;
    [HideInInspector] public EnemyScanner enemyScanner;
    [HideInInspector] public EnemyWeapon enemyWeapon;
    [HideInInspector] public EnemyAttributeWeaknessManager enemyAttributeWeaknessManager;
    [Space]
    public AttributeModifierData[] attributeResistanceMultiplierDatas;
    public AttributeModifierData[] attributeWeaknessMultiplierDatas;
    public List<AttributeModifierData> attributeResistance;
    public List<AttributeModifierData> attributeWeakness;
    [Space]
    [Header("# Core")]
    public bool isDummy = false;
    public float damage;
    public double health;
    public double shield;
    [Space]
    [Header("# Hp Bar")]
    public GameObject hpBarObject;
    [SerializeField] GameObject hpBar;
    [SerializeField] GameObject shieldBar;
    [SerializeField] GameObject hpBackground;
    [SerializeField] GameObject hpAndShieldObject;
    public HpBarColorController HpBarColorController;
    [HideInInspector] public double maxShield;
    [HideInInspector] public double maxHealth;
    public float defenseAmount;
    [Space]
    public float baseSpeed = 1;
    public float speed;
    public float attackSpeed;
    public float baseAttackSpeed;
    public float baseWeakness = 1;
    public float weaknessValue;

    public float normalAttackDamageTakenMultiplier;
    public float skillDamageTakenMultiplier;
    public float abilityDamageTakenMultiplier;

    public float baseNormalAttackDamageTakenMultiplier;
    public float baseSkillDamageTakenMultiplier;
    public float baseAbilityDamageTakenMultiplier;

    public bool isLive;
    [Space(15)]
    [Tab("State Group")]
    [Header("# State Groups")]
    public Transform buffPrefabParent;
    public Transform debuffPrefabParent;
    public Transform weaponPrefabParent;
    public Transform eventPrefabParent;
    Rigidbody2D rb;
    public BoxCollider2D unitDetectionColl;
    public BoxCollider2D sephiraDetectionColl;
    public BoxCollider2D hitboxColl;
    [HideInInspector] public int startSortingOrder;

    [HideInInspector] public SpriteRenderer sp;
    [HideInInspector] public Animator anim;
    [SerializeField] TMP_Text hpText;

    void OnEnable()
    {
        GameStatManager.Inst.OnStatsChanged += UpdateStats;
        if (isDummy)
            Init();
    }

    void OnDisable()
    {
        GameStatManager.Inst.OnStatsChanged -= UpdateStats;
    }

    public void Init()
    {
        enemySkill = null;
        enemyBehavior = null;

        sp.color = Color.white;
        rb.simulated = true;
        EnableCollider(true);
        hpBarObject.SetActive(false);
        gameObject.name = enemyData.enemyName;
        enemyWeapon.coolTime = enemyData.coolTime;
        enemyWeapon.baseCoolTime = 1;
        enemyWeapon.isSkillActive = false;
        enemyScanner.range = enemyData.range;

        baseWeakness = 1;
        baseSpeed = 1;
        baseAttackSpeed = 1;

        baseNormalAttackDamageTakenMultiplier = 1;
        baseSkillDamageTakenMultiplier = 1;
        baseAbilityDamageTakenMultiplier = 1;

        weaknessValue = 1;

        normalAttackDamageTakenMultiplier = 1;
        skillDamageTakenMultiplier = 1;
        abilityDamageTakenMultiplier = 1;

        for (int i = 0; i < attributeResistanceMultiplierDatas.Length; i++)
        {
            attributeResistanceMultiplierDatas[i].value = 1;
        }
        for (int i = 0; i < attributeWeaknessMultiplierDatas.Length; i++)
        {
            attributeWeaknessMultiplierDatas[i].value = 1;
        }

        enemyEvent.ResetEnemyEvent();
        enemyType = enemyData.enemyType;

        transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
        enemyMove.isMoving = true;
        enemyMove.isPathLoop = EnemySpawnManager.Inst.isPathLoop;

        SetHpBarLocateAndScale(0.6f, 0.5f, new Vector3(1, 1, 1));
        EnemySpawnManager.Inst.spawnedEnemies.Add(this);

        switch (enemyType)
        {
            case EnemyData.EnemyType.Normal:
                hpText.enabled = false;
                enemyBehavior = new NormalEnemyBehavior();
                break;
            case EnemyData.EnemyType.Elite:
                hpText.enabled = true;
                enemyBehavior = new EliteEnemyBehavior();
                break;
            case EnemyData.EnemyType.Boss:
                hpText.enabled = true;
                enemyBehavior = new BossEnemyBehavior();
                break;
            case EnemyData.EnemyType.JointOperationBoss:
                hpText.enabled = false;
                enemyBehavior = new JointOperationBossEnemyBehavior();
                break;
        }
        enemyBehavior.Initialize(this);
        enemyAttributeWeaknessManager.Init();

        UpdateStats();

        GameFlowManager.Inst.ChangeEnemyCount(1);
    }

    public void WaitForAnimationMoveEvent()
    {
        anim.ResetTrigger("Appear");

        isLive = true;
        EnableCollider(true);
        hpBarObject.SetActive(enemyBehavior.SetHpBarActive());
        gameObject.layer = 10;

        enemyBehavior.MoveWait();

        if (enemySkill is IEnemyCoolTimeAttackPassive CoolTimeskill)
        {
            CoolTimeskill.ApplyEnemyCoolTimeAttackPassive(this, this);
            anim.SetTrigger("Skill");
        }

        if (enemySkill is IWaitForAnimationEvent waitForAnimationEvent)
        {
            waitForAnimationEvent.ApplyWaitForAnimationEvent(this, this);
        }
    }

    void Awake()
    {
        enemyEvent = GetComponent<EnemyEvent>();
        enemyWeapon = GetComponent<EnemyWeapon>();
        enemyScanner = GetComponent<EnemyScanner>();
        enemyMove = GetComponent<EnemyMove>();
        enemyAttributeWeaknessManager = GetComponentInChildren<EnemyAttributeWeaknessManager>();

        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause || enemyEvent.isStunned)
            return;

        if (enemyMove.nextVec.x != 0)
        {
            sp.flipX = enemyMove.nextVec.x < 0;
        }
    }

    public void UpdateStats()
    {
        int difficultyIndex = (int)GameManager.Inst.curGameDifficulty;

        float slowRatio = Mathf.Clamp01(2f - GameStatManager.Inst.GameSlowValue);
        speed = Mathf.Max(enemyData.speed[difficultyIndex] * (1 + 0.001f * GameManager.Inst.wave) * baseSpeed * slowRatio, 0.1f);

        weaknessValue = Mathf.Min(baseWeakness * GameStatManager.Inst.GameWeakness, 10000);

        attackSpeed = Mathf.Max(0.03f, enemyData.attackSpeed[difficultyIndex] * baseAttackSpeed * GameStatManager.Inst.GameEnemyAttackSpeedMultiplier);

        normalAttackDamageTakenMultiplier = MathF.Max(0, baseNormalAttackDamageTakenMultiplier * GameStatManager.Inst.GameEnemyNormalAttackDamageTakenMultiplier);
        skillDamageTakenMultiplier = MathF.Max(0, baseSkillDamageTakenMultiplier * GameStatManager.Inst.GameEnemySkillDamageTakenMultiplier);
        abilityDamageTakenMultiplier = MathF.Max(0, baseAbilityDamageTakenMultiplier * GameStatManager.Inst.GameEnemyAbilityDamageTakenMultiplier);

        for (int i = 0; i < attributeResistance.Count; i++)
        {
            AttributeModifierData resistanceData = attributeResistance[i];
            AttributeDatabase attributeDatabase = AttributeDatabase.Inst;
            float baseValue = attributeDatabase.GetAttributeMultiplier(resistanceData.data.attributeType, enemyData.attributeStats[difficultyIndex].resistances);

            resistanceData.value = Mathf.Clamp01(baseValue * attributeDatabase.GetAttributeMultiplier(resistanceData.data.attributeType, attributeResistanceMultiplierDatas) * attributeDatabase.GetAttributeMultiplier(resistanceData.data.attributeType, GameStatManager.Inst.gameAttributeResistanceValues));
        }

        for (int i = 0; i < attributeWeakness.Count; i++)
        {
            AttributeModifierData attributeModifierData = attributeWeakness[i];
            AttributeDatabase attributeDatabase = AttributeDatabase.Inst;
            float baseValue = attributeDatabase.GetAttributeMultiplier(attributeModifierData.data.attributeType, enemyData.attributeStats[difficultyIndex].weaknesses);

            attributeModifierData.value = baseValue * attributeDatabase.GetAttributeMultiplier(attributeModifierData.data.attributeType, attributeWeaknessMultiplierDatas) * attributeDatabase.GetAttributeMultiplier(attributeModifierData.data.attributeType, GameStatManager.Inst.gameAttributeWeaknessValues);
        }

        enemyBehavior.UpdateHealthBar();
        enemyBehavior.UpdateShieldBar();
        HpBarColorController.UpdateHealthBarColor((float)(health / maxHealth));
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

    public void OnDamage(AttributeData.AttributeType attributeType, double damage, float criticalPer, float criticalDamage, float defensePenetration = 0, float attributeResistancePenetration = 0, float attributeDamageMultiplier = 1, UnitUtility.AttackType damageTakenType = UnitUtility.AttackType.None, GameObject owner = null, Action onDeathCallBack = null)
    {
        if (isLive)
        {
            StartCoroutine(DamageCoroutine(attributeType, damage, criticalPer, criticalDamage, defensePenetration, attributeResistancePenetration, attributeDamageMultiplier, damageTakenType, owner, onDeathCallBack));
        }

    }

    public IEnumerator DamageCoroutine(AttributeData.AttributeType attributeType, double damage, float criticalPer, float criticalDamage, float defensePenetration, float attributeResistancePenetration, float attributeDamageMultiplier, UnitUtility.AttackType damageTakenType, GameObject owner, Action onDeathCallBack)
    {
        if (!isLive)
            yield break;

        double finalDamage;
        bool isCritical;
        UnitData ownerData = null;
        if (shield > 0)
        {
            (finalDamage, isCritical) = SetFinalDamageMult(attributeType, damage, 0, 1, defensePenetration, attributeResistancePenetration, attributeDamageMultiplier, damageTakenType);
            shield = Math.Max(shield - finalDamage, 0);
        }
        else
        {
            (finalDamage, isCritical) = SetFinalDamageMult(attributeType, damage, criticalPer, criticalDamage, defensePenetration, attributeResistancePenetration, attributeDamageMultiplier, damageTakenType);
            health -= finalDamage;
        }
        if (owner != null)
        {
            if (owner.TryGetComponent<Unit>(out var unit))
            {
                ownerData = unit.unitData;
                UnitCombatStatManager.Inst.AddDamage(ownerData, finalDamage);

                if (isCritical)
                    unit.unitEvent.OnCriticalHit?.Invoke();
                else
                    unit.unitEvent.OnAttackHit?.Invoke();
            }
            else if (owner.TryGetComponent<SephiraUnit>(out var sephira))
            {
                ownerData = sephira.unitData;
                UnitCombatStatManager.Inst.AddDamage(ownerData, finalDamage);

                if (isCritical)
                    sephira.sephiraUnitEvent.OnCriticalHit?.Invoke();
                else
                    sephira.sephiraUnitEvent.OnAttackHit?.Invoke();
            }
        }
        sp.color = Color.red;

        enemyEvent.TryInstDeath();

        UpdateStats();
        enemyEvent.OnDamaged?.Invoke(this);
        if (health <= 0)
        {
            if (enemyType != EnemyData.EnemyType.JointOperationBoss)
            {
                OnDead();
            }
            else
            {
                anim.SetBool("IsDead", true);
                EnableCollider(false);
                yield return new WaitForSeconds(2);
                OnDead();
            }
            onDeathCallBack?.Invoke();
        }

        yield return new WaitForSeconds(0.1f);

        sp.color = Color.white;
    }

    public void OnDead()
    {
        isLive = false;

        GameFlowManager.Inst.ChangeEnemyCount(-1);
        GameFlowManager.Inst.ChangeKillCount(1);

        EnemySpawnManager.Inst.spawnedEnemies.Remove(this);

        if (Random.Range(0f, 100f) < GameStatManager.Inst.GameGetDarkMatteCoin)
            GameManager.Inst.ModifyDarkMatteCoin(1);

        int coin;
        coin = (int)(enemyData.coinDropPerDifficulty[(int)GameManager.Inst.curGameDifficulty] * GameStatManager.Inst.GameGetCoin);
        GameManager.Inst.ModifyCoin(coin);

        //DeahReward
        enemyBehavior.GrantDeathReward();

        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.EnemyKill, 1);
        enemyEvent.TryShotBomb();

        rb.simulated = false;
        EnableCollider(false);
        gameObject.SetActive(false);
    }

    public (double finalDamage, bool isCritical) SetFinalDamageMult(
    AttributeData.AttributeType attributeType,
    double damageAmount,
    float criticalPer,
    float criticalDamage,
    float defensePenetration,
    float attributeResistancePenetration,
    float attributeDamageMultiplier, UnitUtility.AttackType damageTakenType)
    {
        DamageText damageText = PoolManager.Inst
            .Get(PoolManager.PrefabType.damageText, transform.position)
            .GetComponent<DamageText>();

        defensePenetration = Mathf.Clamp01(defensePenetration);
        attributeResistancePenetration = Mathf.Clamp01(attributeResistancePenetration);
        attributeDamageMultiplier = Mathf.Max(0, attributeDamageMultiplier);

        int randomValue = UnityEngine.Random.Range(1, 101);

        float effectiveDefense = defenseAmount * (1f - defensePenetration);
        float defenseMultiplier = 100f / (100f + effectiveDefense);

        float debuffBonus = 1f + enemyEvent.debuffCount * 0.1f;
        float baseMultiplier = weaknessValue * debuffBonus;

        float attributeResistance = 0f;
        float attributeWeakness = 0f;

        foreach (AttributeModifierData resistance in this.attributeResistance)
        {
            if (attributeType == resistance.data.attributeType &&
                attributeType != AttributeData.AttributeType.Fixed)
            {
                attributeResistance = Mathf.Clamp01(resistance.value);
                break;
            }
        }

        foreach (AttributeModifierData weakness in this.attributeWeakness)
        {
            if (attributeType == weakness.data.attributeType)
            {
                attributeWeakness = weakness.value;
                break;
            }
        }


        float effectiveResist = attributeResistance * (1f - attributeResistancePenetration);

        float finalWeakness =
            attributeWeakness *
            attributeDamageMultiplier * AttributeDatabase.Inst.GetAttributeMultiplier(attributeType, GameStatManager.Inst.gameAttributeDamageBonus);

        float attributeMultiplier =
            (1f - effectiveResist) *
            (1f + finalWeakness);

        float totalMultiplier = baseMultiplier * attributeMultiplier;

        bool isCritical = randomValue <= criticalPer;

        float takenDamage;

        switch (damageTakenType)
        {
            case UnitUtility.AttackType.None:
                takenDamage = 1;
                break;
            case UnitUtility.AttackType.NormalAttack:
                takenDamage = normalAttackDamageTakenMultiplier;
                break;
            case UnitUtility.AttackType.Skill:
                takenDamage = skillDamageTakenMultiplier;
                break;
            case UnitUtility.AttackType.Ability:
                takenDamage = abilityDamageTakenMultiplier;
                break;
            default:
                takenDamage = 1;
                break;
        }

        double finalDamage = isCritical
            ? damageAmount * (criticalDamage / 10f) * totalMultiplier * defenseMultiplier
            : damageAmount * totalMultiplier * defenseMultiplier * takenDamage;

        if (isCritical)
            damageText.InitializeText(attributeType, "CR " + finalDamage.ToString("F0") + "!", true);
        else
            damageText.InitializeText(attributeType, finalDamage.ToString("F0"), false);

        return (finalDamage, isCritical);
    }

    public void SetHitColliderSizeAndOffset(float sizeX, float sizeY, float offsetX, float offsetY)
    {
        hitboxColl.size = new Vector2(sizeX, sizeY);
        hitboxColl.offset = new Vector2(offsetX, offsetY);
    }

    public void SetUnitDetecColliderSizeAndOffset(float sizeX, float sizeY, float offsetX, float offsetY)
    {
        unitDetectionColl.size = new Vector2(sizeX, sizeY);
        unitDetectionColl.offset = new Vector2(offsetX, offsetY);
    }

    public void SetSephiraDetecColliderSizeAndOffset(float sizeX, float sizeY, float offsetX, float offsetY)
    {
        sephiraDetectionColl.size = new Vector2(sizeX, sizeY);
        sephiraDetectionColl.offset = new Vector2(offsetX, offsetY);
    }

    public void SetHpBarLocateAndScale(float newBackgroundScaleX, float newHpBarPostionY, Vector3 newHpObjectScale)
    {
        float currentBackgroundScaleX = hpBackground.transform.localScale.x;

        float backgroundScaleUnit = 0.1f;
        float hpBarScalePerUnit = 0.17f;

        float scaleDifference = Mathf.Abs(currentBackgroundScaleX - newBackgroundScaleX);
        int unitSteps = Mathf.RoundToInt(scaleDifference / backgroundScaleUnit);

        hpBarObject.transform.localPosition = new Vector3(0, newHpBarPostionY, 0);

        hpBarObject.transform.localScale = newHpObjectScale;

        Vector3 updatedBackgroundScale = hpBackground.transform.localScale;
        updatedBackgroundScale.x = newBackgroundScaleX;
        hpBackground.transform.localScale = updatedBackgroundScale;

        float newHpBarScaleX = 1 + (unitSteps * hpBarScalePerUnit);
        Vector3 updatedHpBarScale = hpAndShieldObject.transform.localScale;
        updatedHpBarScale.x = newHpBarScaleX;
        hpAndShieldObject.transform.localScale = updatedHpBarScale;
    }

    public void EnableCollider(bool isOn)
    {
        unitDetectionColl.enabled = isOn;
        sephiraDetectionColl.enabled = isOn;
    }

    public void SetBuffAndDebuffParentScale(float sclaeFactor)
    {
        buffPrefabParent.transform.localScale = new Vector3(sclaeFactor, sclaeFactor, sclaeFactor);
        debuffPrefabParent.transform.localScale = new Vector3(sclaeFactor, sclaeFactor, sclaeFactor);
    }


}


