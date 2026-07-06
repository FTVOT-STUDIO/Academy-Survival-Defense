using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombBullet : MonoBehaviour
{
    public GameObject owner;

    public AttributeData.AttributeType attributeType;
    public double damage;
    public float criticalPer;
    public float criticalDamage;

    public float explodeDelay = 1.5f;
    public float explosionRadius = 1f;
    public float lifeTime = 0.5f;

    public float defensePenetration;
    public float attributeResistancePenetration;
    public float attributeDamageMultiplier;

    public Bullet.DebuffState debuffState = Bullet.DebuffState.None;
    public Bullet.BulletOwnerType bulletOwnerType = Bullet.BulletOwnerType.None;

    private float value;
    private float debuffTime;
    private double debuffDamage;

    public bool isPlaySfx;

    private UnitUtility.AttackType attackType;

    public AudioManager.Sfx hitSound = AudioManager.Sfx.EnemyHit;
    public EffectManager.PrefabType particleType = EffectManager.PrefabType.EnemyHitEffect;

    private List<Enemy> hitEnemies = new List<Enemy>();
    System.Action<Enemy> hitAction;

    private void OnDisable()
    {
        damage = 0;
        criticalPer = 0;
        criticalDamage = 0;
        explodeDelay = 1.5f;
        explosionRadius = 1f;
        lifeTime = 0.5f;

        debuffState = Bullet.DebuffState.None;
        value = 0;
        debuffTime = 0;
        debuffDamage = 0;

        defensePenetration = 0;
        attributeResistancePenetration = 0;
        attributeDamageMultiplier = 1;

        isPlaySfx = true;

        owner = null;
        hitEnemies.Clear();
        hitAction = null;

        attackType = UnitUtility.AttackType.None;
    }

    public List<Enemy> GetHitEnemies()
    {
        return hitEnemies;
    }

    public void Launch(Vector3 position, Bullet.BulletOwnerType ownerType, float delay, float radius = 0.5f, float lifeTime = 0.5f, System.Action<Enemy> action = null)
    {
        if (position == null || !owner.activeSelf)
        {
            return;
        }
        bulletOwnerType = ownerType;
        transform.position = position;
        explodeDelay = delay;
        explosionRadius = radius;
        this.lifeTime = lifeTime;

        if (action != null)
            hitAction = action;

        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explodeDelay);
        Explode();
    }

    void Explode()
    {
        Collider2D[] hitEnemiesColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy HitBox"));

        foreach (var hit in hitEnemiesColliders)
        {
            if (!hit.transform.parent.gameObject.activeInHierarchy)
                continue;

            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                if (!hitEnemies.Contains(enemy))
                {
                    hitEnemies.Add(enemy);
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
                }

            }
        }

        EffectManager.Inst.Get(particleType, transform.position);

        if (isPlaySfx) AudioManager.Inst.PlaySfx(hitSound);

        StartCoroutine(DisableAfterDelay(lifeTime));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public void SetAttackInfo(AttributeData.AttributeType attributeType, double damage, float criticalPer, float criticalDamage, float areaDamage, float defensePenetration, float attributeResistancePenetration, float attributeDamageMultiplier, UnitUtility.AttackType attackType, float attackTypeMultiplier, bool isHealthRatio = false)
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
    }

    public void SetDebuffInfo(Bullet.DebuffState state = Bullet.DebuffState.None, float value = 0, float time = 0, double damage = 0)
    {
        debuffState = state;
        this.value = value;
        this.debuffTime = time;
        this.debuffDamage = damage;
    }

    public void SetVisualEffect(EffectManager.PrefabType particleType = EffectManager.PrefabType.EnemyHitEffect, bool isPlaySfx = true, AudioManager.Sfx hitSound = AudioManager.Sfx.EnemyHit)
    {
        this.particleType = particleType;
        this.isPlaySfx = isPlaySfx;
        this.hitSound = hitSound;
    }

}
