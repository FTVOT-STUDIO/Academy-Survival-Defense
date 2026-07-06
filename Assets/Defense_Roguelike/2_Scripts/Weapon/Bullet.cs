using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Enemy targetEnemy;
    public AttributeData.AttributeType attributeType;
    public GameObject owner;
    public double damage;
    public float speed;

    public float lifeTime;
    public bool isPlaySfx;
    public AudioManager.Sfx hitSound;
    [HideInInspector] public Rigidbody2D rb;
    CircleCollider2D coll;

    public float criticalPer;
    public float criticalDamage;

    private float value;
    private double debuffDamage;
    private float debuffTime;
    private float areaDamage;
    private float singleTargetDamagefloat;

    public float defensePenetration;
    public float attributeResistancePenetration;
    public float attributeDamageMultiplier;
    private UnitUtility.AttackType attackType;
    public enum BulletInitMode { Linear, HitScan }
    public enum DebuffState { None, Slow, Stun, Dot, InsDeathAll, InsDeathOnlyNormal, Weakness, Terrified, Charm }
    public enum BulletOwnerType { None, Unit, Sephira }
    public enum BulletType { Single, Multi }
    public enum BulletSpeedType { None, Slow, Normal, Fast }
    public BulletSpeedType bulletSpeedType;
    private Dictionary<BulletSpeedType, float> SpeedTable = new Dictionary<BulletSpeedType, float>
    {
        {BulletSpeedType.None , 0},
        { BulletSpeedType.Slow , 5},
        {BulletSpeedType.Normal , 15},
        {BulletSpeedType.Fast , 25}
    };
    public DebuffState debuffState;
    public BulletType bulletType;
    public BulletInitMode bulletInitMode;
    public BulletOwnerType bulletOwnerType;
    public EffectManager.PrefabType particleType;

    bool hasPlayedEffect = false;
    bool hasHitTarget = false;

    public List<Enemy> hitEnemies = new List<Enemy>();
    System.Action<Enemy> hitAction;

    void OnEnable()
    {
        CancelInvoke();

        damage = 0;
        criticalPer = 0;
        criticalDamage = 0;
        value = 0;
        debuffTime = 0;
        debuffDamage = 0;

        areaDamage = 1;
        singleTargetDamagefloat = 1;

        defensePenetration = 0;
        attributeResistancePenetration = 0;
        attributeDamageMultiplier = 1;

        coll.radius = 0;

        isPlaySfx = true;

        owner = null;
        targetEnemy = null;
        rb.linearVelocity = Vector2.zero;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        hasPlayedEffect = false;
        hasHitTarget = false;

        debuffState = DebuffState.None;
        bulletType = BulletType.Single;

        attackType = UnitUtility.AttackType.None;

        hitSound = AudioManager.Sfx.EnemyHit;
        particleType = EffectManager.PrefabType.EnemyHitEffect;

        hitAction = null;
        hitEnemies.Clear();

        if (GetComponentInChildren<TrailRenderer>() != null)
        {
            GetComponentInChildren<TrailRenderer>().Clear();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
    }

    /// <summary>
    /// SetAttackInfo() 과 Launch()는 필수 구현, 필요에 따라 SetDebuffInfo(), SetVisualEffect() 설정
    /// </summary>
    public void SetAttackInfo(AttributeData.AttributeType attributeType, double damage, float criticalPer, float criticalDamage, UnitUtility.AttackRangeType attackRangeType, float attackRangeDamageMultiplier, float defensePenetration, float attributeResistancePenetration, float attributeDamageMultiplier, UnitUtility.AttackType attackType, float attackTypeMultiplier, bool isHealthRatio = false)
    {
        this.attackType = attackType;
        this.attributeType = attributeType;
        this.damage = damage;
        this.damage *= attackTypeMultiplier;

        switch (attackRangeType)
        {
            case UnitUtility.AttackRangeType.Single:
                this.areaDamage = attackRangeDamageMultiplier;
                break;
            case UnitUtility.AttackRangeType.Area:
                this.singleTargetDamagefloat = attackRangeDamageMultiplier;
                break;
        }

        this.criticalPer = isHealthRatio ? 0 : criticalPer;
        this.criticalDamage = criticalDamage;
        this.defensePenetration = defensePenetration;
        this.attributeResistancePenetration = attributeResistancePenetration;
        this.attributeDamageMultiplier = attributeDamageMultiplier;
    }

    public void SetDebuffInfo(DebuffState state = DebuffState.None, float value = 0, float time = 0, double damage = 0)
    {
        debuffState = state;
        this.value = value;
        debuffTime = time;
        debuffDamage = damage;
    }

    public void SetVisualEffect(EffectManager.PrefabType particleType = EffectManager.PrefabType.EnemyHitEffect, bool isPlaySfx = true, AudioManager.Sfx hitSound = AudioManager.Sfx.EnemyHit)
    {
        this.particleType = particleType;
        this.isPlaySfx = isPlaySfx;
        this.hitSound = hitSound;
    }

    public List<Enemy> GetHitEnemies()
    {
        return hitEnemies;
    }

    public void Launch(Vector3 positionOrDirection, BulletInitMode mode, BulletType type, BulletOwnerType ownerType, float radius, float lifeTime = 0, BulletSpeedType speedType = BulletSpeedType.Normal, System.Action<Enemy> action = null)
    {
        if (positionOrDirection == null) return;
        CancelInvoke();
        this.lifeTime = lifeTime;
        bulletType = type;
        bulletInitMode = mode;
        bulletOwnerType = ownerType;
        coll.radius = radius;
        SetSpeed(speedType);

        if (bulletType == BulletType.Multi)
        {
            this.damage *= this.areaDamage;
        }
        else
        {
            this.damage *= this.singleTargetDamagefloat;
        }

        switch (mode)
        {
            case BulletInitMode.Linear:
                Vector3 dir = (positionOrDirection - transform.position).normalized;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                rb.linearVelocity = dir * speed;
                break;

            case BulletInitMode.HitScan:
                transform.position = positionOrDirection;
                break;
        }

        if (action != null)
            hitAction = action;

        Invoke(nameof(DeActiveBullet), lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!other.transform.parent.gameObject.activeInHierarchy)
                return;

            Enemy enemy = other.GetComponentInParent<Enemy>();

            if (bulletInitMode == BulletInitMode.HitScan && bulletType == BulletType.Single && owner != null)
            {
                switch (bulletOwnerType)
                {
                    case BulletOwnerType.Unit:
                        if (owner.GetComponent<Unit>().unitScanner.nearestTarget != null)
                            enemy = owner.GetComponent<Unit>().unitScanner.nearestTarget.GetComponent<Enemy>();
                        break;
                    case BulletOwnerType.Sephira:
                        if (owner.GetComponent<SephiraUnit>().nearestTarget != null)
                            enemy = owner.GetComponent<SephiraUnit>().nearestTarget.GetComponent<Enemy>();
                        break;
                }
            }

            targetEnemy = enemy;

            switch (bulletType)
            {
                case BulletType.Single:
                    if (!hasHitTarget)
                    {
                        hasHitTarget = true;
                        HandleHit(enemy);
                        TargetEffect(enemy.transform);
                    }
                    break;
                case BulletType.Multi:
                    if (!hitEnemies.Contains(enemy))
                    {
                        hitEnemies.Add(enemy);
                        HandleHit(enemy);
                    }
                    if (!hasPlayedEffect)
                    {
                        TargetEffect(enemy.transform);
                        hasPlayedEffect = true;
                    }
                    break;
            }


            if (bulletType == BulletType.Single)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void DeActiveBullet()
    {
        rb.linearVelocity = Vector2.zero;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
    }

    public void TargetEffect(Transform target)
    {
        EffectManager.Inst.Get(particleType, target.position);
        if (isPlaySfx)
            AudioManager.Inst.PlaySfx(hitSound);
    }

    private void SetSpeed(BulletSpeedType speedType)
    {
        bulletSpeedType = speedType;
        speed = SpeedTable[speedType];
    }

    private void HandleHit(Enemy enemy)
    {
        enemy.OnDamage(attributeType, damage, criticalPer, criticalDamage, defensePenetration, attributeResistancePenetration, attributeDamageMultiplier, attackType, owner, () =>
        {
            switch (bulletOwnerType)
            {
                case BulletOwnerType.Unit:
                    if (owner != null)
                    {
                        Unit unit = owner.GetComponent<Unit>();
                        if (unit.unitSkill is IKillEvent unitKillEvent)
                            unitKillEvent.ApplyKillEvent(unit);
                        unit.unitEvent.ApplyKillEffect();
                    }
                    break;
                case BulletOwnerType.Sephira:
                    if (owner != null)
                    {
                        SephiraUnit sephira = owner.GetComponent<SephiraUnit>();
                        if (sephira.unitSkill is IKillEvent sephiraKillEvent)
                            sephiraKillEvent.ApplyKillEvent(sephira);
                    }
                    break;
            }
        });

        enemy.enemyEvent.ApplyDebuff(debuffState, value, debuffTime, debuffDamage, owner, attributeType);
        hitAction?.Invoke(enemy);
    }
}
