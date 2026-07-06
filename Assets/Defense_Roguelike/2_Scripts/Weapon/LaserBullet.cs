using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ObjectLifecycleManager))]
public class LaserBullet : MonoBehaviour
{
    public GameObject owner;
    public AttributeData.AttributeType attributeType;
    public double damage;
    public float criticalPer;
    public float criticalDamage;
    public float tickTime = 0.2f;
    public float defensePenetration;
    public float attributeResistancePenetration;
    public float attributeDamageMultiplier;

    public Bullet.DebuffState debuffState;
    public Bullet.BulletOwnerType bulletOwnerType = Bullet.BulletOwnerType.None;

    private float value;
    private float debuffTime;
    private double debuffDamage;

    private UnitUtility.AttackType attackType;

    private Dictionary<Enemy, Coroutine> activeCoroutines = new Dictionary<Enemy, Coroutine>();

    [HideInInspector] public ObjectLifecycleManager objectLifecycleManager;

    private List<Enemy> hitEnemies = new List<Enemy>();
    System.Action<Enemy> hitAction;

    void Awake()
    {
        objectLifecycleManager = GetComponent<ObjectLifecycleManager>();
    }
    void OnDisable()
    {
        owner = null;

        debuffState = Bullet.DebuffState.None;

        damage = 0;
        criticalPer = 0;
        criticalDamage = 0;

        value = 0;
        debuffTime = 0;
        debuffDamage = 0;

        defensePenetration = 0;
        attributeResistancePenetration = 0;
        attributeDamageMultiplier = 1;
        attackType = UnitUtility.AttackType.None;
        StopAllCoroutines();
        activeCoroutines.Clear();
        hitEnemies.Clear();
        hitAction = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!other.transform.parent.gameObject.activeInHierarchy)
                return;
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null && !activeCoroutines.ContainsKey(enemy))
            {
                if (!hitEnemies.Contains(enemy))
                    hitEnemies.Add(enemy);
                Coroutine coroutine = StartCoroutine(DamageCoroutine(enemy));
                activeCoroutines.Add(enemy, coroutine);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>(true);
            if (enemy != null && activeCoroutines.TryGetValue(enemy, out Coroutine routine))
            {
                if (routine != null)
                    StopCoroutine(routine);
                activeCoroutines.Remove(enemy);
            }
        }
    }

    IEnumerator DamageCoroutine(Enemy enemy)
    {
        while (enemy != null && enemy.health > 0 && enemy.gameObject.activeInHierarchy)
        {
            enemy.OnDamage(attributeType, damage, criticalPer, criticalDamage, defensePenetration, attributeResistancePenetration, attributeDamageMultiplier, attackType, owner, () =>
            {
                if (owner != null)
                {
                    switch (bulletOwnerType)
                    {
                        case Bullet.BulletOwnerType.Unit:
                            Unit unit = owner.GetComponent<Unit>();
                            if (unit.unitSkill is IKillEvent unitKillEvent)
                            {
                                unitKillEvent.ApplyKillEvent(unit);
                            }
                            unit.unitEvent.ApplyKillEffect();
                            break;
                        case Bullet.BulletOwnerType.Sephira:
                            SephiraUnit sephira = owner.GetComponent<SephiraUnit>();
                            if (sephira.unitSkill is IKillEvent sephiraKillEvent)
                            {
                                sephiraKillEvent.ApplyKillEvent(sephira);
                            }
                            //sephiraOwner?.
                            break;
                    }
                }
            });

            enemy.enemyEvent.ApplyDebuff(debuffState, value, debuffTime, debuffDamage);
            hitAction?.Invoke(enemy);

            yield return new WaitForSeconds(tickTime);
        }

        if (enemy != null)
            activeCoroutines.Remove(enemy);
    }


    public void SetAttackInfo(AttributeData.AttributeType attributeType, double damage, float criticalPer, float criticalDamage, float areaDamage, float defensePenetration, float attributeResistancePenetration, float attributeDamageMultiplier, UnitUtility.AttackType attackType, float attackTypeMultiplier, float tickTime = 0.5f, bool isHealthRatio = false)
    {
        this.attackType = attackType;
        this.attributeType = attributeType;
        this.damage = damage * areaDamage;
        this.damage *= attackTypeMultiplier;
        this.criticalPer = isHealthRatio ? 0 : criticalPer;
        this.criticalDamage = criticalDamage;
        this.defensePenetration = defensePenetration;
        this.attributeResistancePenetration = attributeResistancePenetration;
        this.attributeDamageMultiplier = attributeDamageMultiplier;
        this.tickTime = tickTime;
    }

    public void Launch(Vector3 position, Bullet.BulletOwnerType ownerType, System.Action<Enemy> action = null)
    {
        if (position == null || !owner.activeSelf)
        {
            return;
        }

        bulletOwnerType = ownerType;
        transform.position = position;
        if (action != null)
            hitAction = action;
    }

    public void SetDebuffInfo(Bullet.DebuffState state = Bullet.DebuffState.None, float value = 0, float time = 0, double damage = 0)
    {
        debuffState = state;
        this.value = value;
        this.debuffTime = time;
        this.debuffDamage = damage;
    }
}
