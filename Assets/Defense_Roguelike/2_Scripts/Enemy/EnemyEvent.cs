using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class EnemyEvent : MonoBehaviour
{
    Enemy enemy;

    public bool isSlowActive;
    public bool isDotActive;
    public bool isWeaknessActive;
    public bool isStunned;
    public bool isTerrified;
    public bool isGravityPull;
    public bool isCharmed;
    public bool isHealBan;
    public bool isShieldBan;

    public int debuffCount;

    private Coroutine weaknessCoroutine;
    private Coroutine slowDebuffCoroutine;
    private Coroutine dotDamageCoroutine;
    private Coroutine stunDebuffCoroutine;
    private Coroutine terrifiedDebuffCoroutine;
    private Coroutine charmedDebuffCoroutine;
    private Coroutine healBenDebuffCoroutine;
    private Coroutine shieldBenDebuffCoroutine;

    float weaknessMultiplier = 1f;
    float slowMultiplier = 1f;
    float terrifiedSlowMultiplier = 1;
    float charmSlowMultiplier = 1;

    public Action<Enemy> OnDamaged;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ApplyGetHeal(double value)
    {
        if (isHealBan)
            return;
        enemy.health = Math.Min(enemy.health + value, enemy.maxHealth);
        enemy.UpdateStats();
    }

    public void ApplyGetShield(double value)
    {
        if (isShieldBan)
            return;
        enemy.shield = Math.Min(enemy.shield + value, enemy.maxShield);
        enemy.UpdateStats();
    }

    public void ApplyDurationHealToEnemy(float healAmount, float time, float tick)
    {
        if (isHealBan)
            return;
        StartCoroutine(DurationHealRoutine(healAmount, time, tick));
    }

    IEnumerator DurationHealRoutine(float healAmount, float time, float tick)
    {
        ObjectLifecycleManager effect = EffectManager.Inst.Get(EffectManager.PrefabType.DurationHealEffect, transform.position).GetComponent<ObjectLifecycleManager>();

        effect.SetParent(enemy.buffPrefabParent);
        effect.Init(ObjectLifecycleManager.ObjectType.Effect);
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            ApplyGetHeal(healAmount);
            yield return new WaitForSeconds(tick);

            elapsedTime += tick;
        }

        effect.DeActive();
    }

    public void ApplyDebuff(Bullet.DebuffState debuffState, float value, float debuffTime, double debuffDamage = 0, GameObject owner = null, AttributeData.AttributeType attributeType = AttributeData.AttributeType.Fixed)
    {
        debuffTime *= GameStatManager.Inst.EnemyDebuffDurationMultiplier;
        switch (debuffState)
        {
            case Bullet.DebuffState.Slow:
                TrySlowDebuff(value, debuffTime);
                break;
            case Bullet.DebuffState.Stun:
                TryStun(value, debuffTime);
                break;
            case Bullet.DebuffState.Dot:
                TryDotDamage(value, debuffTime, debuffDamage, owner, attributeType);
                break;
            case Bullet.DebuffState.Weakness:
                TryWeakness(value, debuffTime);
                break;
            case Bullet.DebuffState.Terrified:
                TryTerrified(value, debuffTime);
                break;
            case Bullet.DebuffState.InsDeathAll:
                if (value <= debuffDamage)
                    InsDeathAll();
                break;
            case Bullet.DebuffState.InsDeathOnlyNormal:
                if (value <= debuffDamage)
                    InsDeathOnlyNormal();
                break;
            case Bullet.DebuffState.Charm:
                TryCharmed(value, debuffTime);
                break;
            case Bullet.DebuffState.None:
            default:
                break;
        }
    }

    public void ResetEnemyEvent()
    {
        debuffCount = 0;
        isSlowActive = false;
        isDotActive = false;
        isWeaknessActive = false;
        isStunned = false;
        isTerrified = false;
        isGravityPull = false;
        isCharmed = false;
        isHealBan = false;
        isShieldBan = false;
        weaknessMultiplier = 1;
        slowMultiplier = 1;

        weaknessCoroutine = null;
        slowDebuffCoroutine = null;
        dotDamageCoroutine = null;
        stunDebuffCoroutine = null;
        terrifiedDebuffCoroutine = null;
        charmedDebuffCoroutine = null;
        healBenDebuffCoroutine = null;
        shieldBenDebuffCoroutine = null;


        ObjectLifecycleManager[] buffObjs = enemy.buffPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager buffObj in buffObjs)
        {
            if (buffObj != null)
                buffObj.DeActive();
        }

        ObjectLifecycleManager[] debuffObjs = enemy.debuffPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager debuffObj in debuffObjs)
        {
            if (debuffObj != null)
                debuffObj.DeActive();
        }

        ObjectLifecycleManager[] weaponObjs = enemy.weaponPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager weaponObj in weaponObjs)
        {
            if (weaponObj != null)
                weaponObj.DeActive();
        }

        ObjectLifecycleManager[] eventObjs = enemy.eventPrefabParent.GetComponentsInChildren<ObjectLifecycleManager>();
        foreach (ObjectLifecycleManager eventObj in eventObjs)
        {
            if (eventObj != null)
                eventObj.DeActive();
        }


        StopAllCoroutines();

    }

    public void TryInstDeath()
    {
        if (WeaponManager.Inst.isInsDeath && enemy.enemyData.enemyType == EnemyData.EnemyType.Normal)
        {
            float ran = Random.Range(0f, 1f);

            if (ran < WeaponManager.Inst.insDeathProb)
            {
                InsDeathOnlyNormal();
            }
        }
    }

    public void TryShotBomb()
    {
        if (WeaponManager.Inst.isBoomb)
        {
            Bullet bombBullet = PoolManager.Inst.Get(PoolManager.PrefabType.Bomb, transform.position).GetComponent<Bullet>();
            bombBullet.SetAttackInfo(AttributeData.AttributeType.Fixed, enemy.maxHealth * WeaponManager.Inst.baseBoombDamage, GameStatManager.Inst.GameCriticalPer, GameStatManager.Inst.GameCriticalDamage, UnitUtility.AttackRangeType.Area, GameStatManager.Inst.GameAreaDamage, 0, 0, 1, UnitUtility.AttackType.None, 1);

            bombBullet.SetVisualEffect(EffectManager.PrefabType.BombEffect, false);

            bombBullet.Launch(transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.None, 1f, 0.3f);

            EffectManager.Inst.Get(EffectManager.PrefabType.BombEffect, transform.position);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Boomb);
        }
    }

    public void TrySlowDebuff(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isSlowActive)
        {
            debuffCount++;
            return;
        }

        debuffCount++;
        slowDebuffCoroutine = StartCoroutine(SlowDebuffCoroutine(value, debuff_Time));

    }

    public void TryDotDamage(float tick, float debuff_Time, double damagePerTick, GameObject owner, AttributeData.AttributeType attributeType)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isDotActive)
        {
            debuffCount++;
            return;
        }

        debuffCount++;
        dotDamageCoroutine = StartCoroutine(DotDamageCoroutine(debuff_Time, tick, damagePerTick, 0.2f, 0.2f, 2, owner, attributeType));

    }

    public void TryWeakness(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isWeaknessActive)
        {
            debuffCount++;
            return;
        }

        debuffCount++;

        weaknessCoroutine = StartCoroutine(WeaknessCoroutine(value, debuff_Time));

    }

    public void TryStun(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isStunned)
        {
            debuffCount++;
            return;
        }

        debuffCount++;

        stunDebuffCoroutine = StartCoroutine(StunDebuffCoroutine(value, debuff_Time));
    }

    public void TryTerrified(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isTerrified)
        {
            debuffCount++;
            return;
        }

        debuffCount++;

        terrifiedDebuffCoroutine = StartCoroutine(TerrifiedDebuffCoroutine(value, debuff_Time));
    }

    public void TryCharmed(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isCharmed)
        {
            debuffCount++;
            return;
        }

        debuffCount++;

        charmedDebuffCoroutine = StartCoroutine(CharmedDebuffCoroutine(value, debuff_Time));
    }

    public void TryHealben(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isHealBan)
        {
            debuffCount++;
            return;
        }

        debuffCount++;

        healBenDebuffCoroutine = StartCoroutine(HealBenDebuffCoroutine(value, debuff_Time));
    }

    public void TryShieldBen(float value, float debuff_Time)
    {
        if (!gameObject.activeInHierarchy || !enemy.isLive)
            return;

        if (isShieldBan)
        {
            debuffCount++;
            return;
        }

        debuffCount++;

        shieldBenDebuffCoroutine = StartCoroutine(ShieldBenDebuffCoroutine(value, debuff_Time));
    }

    public void InsDeathAll()
    {
        if (enemy.enemyData.enemyType == EnemyData.EnemyType.JointOperationBoss)
            return;

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.TryInstDeath);
        EffectManager.Inst.Get(EffectManager.PrefabType.InstDeathEffect, enemy.transform.position);
        enemy.OnDead();
    }

    public void InsDeathOnlyNormal()
    {
        if (enemy.enemyData.enemyType != EnemyData.EnemyType.Normal)
            return;

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.TryInstDeath);
        EffectManager.Inst.Get(EffectManager.PrefabType.InstDeathEffect, enemy.transform.position);
        enemy.OnDead();
    }

    IEnumerator WeaknessCoroutine(float value, float debuff_Time)
    {
        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.Weakness, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);

        ApplyWeaknessDebuff(value);

        yield return new WaitForSeconds(debuff_Time);

        RemoveWeaknessDebuff();

        effectLifeCycle.DeActive();
    }
    void ApplyWeaknessDebuff(float weaknessMultiplier)
    {
        isWeaknessActive = true;
        this.weaknessMultiplier = weaknessMultiplier;
        enemy.baseWeakness *= weaknessMultiplier + 1;
        enemy.UpdateStats();
    }

    void RemoveWeaknessDebuff()
    {
        enemy.baseWeakness /= weaknessMultiplier + 1;
        weaknessMultiplier = 0;
        isWeaknessActive = false;
        enemy.UpdateStats();
    }

    IEnumerator SlowDebuffCoroutine(float value, float debuff_Time)
    {
        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.Slow, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);

        ApplySlowDebuff(value);
        yield return new WaitForSeconds(debuff_Time);

        RemoveSlowDebuff();
        effectLifeCycle.DeActive();

    }

    void ApplySlowDebuff(float slowMultiplier)
    {
        isSlowActive = true;
        this.slowMultiplier = slowMultiplier;
        enemy.baseSpeed -= slowMultiplier;
        enemy.UpdateStats();
    }

    void RemoveSlowDebuff()
    {
        enemy.baseSpeed += slowMultiplier;
        slowMultiplier = 0;
        isSlowActive = false;
        enemy.UpdateStats();
    }

    void ApplyCharmSlowDebuff(float slowMultiplier)
    {
        charmSlowMultiplier = slowMultiplier;
        enemy.baseSpeed -= charmSlowMultiplier;
        enemy.UpdateStats();
    }

    void RemoveCharmSlowDebuff()
    {
        enemy.baseSpeed += charmSlowMultiplier;
        charmSlowMultiplier = 0;
        enemy.UpdateStats();
    }

    void ApplyTerrifiedSlowDebuff(float slowMultiplier)
    {
        terrifiedSlowMultiplier = slowMultiplier;
        enemy.baseSpeed -= terrifiedSlowMultiplier;
        enemy.UpdateStats();
    }

    void RemoveTerrifiedSlowDebuff()
    {
        enemy.baseSpeed += terrifiedSlowMultiplier;
        terrifiedSlowMultiplier = 0;
        enemy.UpdateStats();
    }

    IEnumerator DotDamageCoroutine(float duration, float interval, double damage, float defensePenetration, float attributeResistanceIgnore, float attributeDamageMultiplier, GameObject owner, AttributeData.AttributeType attributeType)
    {
        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.Dot, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);

        isDotActive = true;

        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += interval;

            enemy.OnDamage(attributeType, damage, GameStatManager.Inst.GameCriticalPer, GameStatManager.Inst.GameCriticalDamage, defensePenetration, attributeResistanceIgnore, attributeDamageMultiplier, UnitUtility.AttackType.None, owner, () => { /*Debug.Log("Enemy Die!");*/ });

            if (enemy.health <= 0)
            {
                isDotActive = false;
                break;
            }

            yield return new WaitForSeconds(interval);
        }

        isDotActive = false;
        effectLifeCycle.DeActive();
    }

    IEnumerator StunDebuffCoroutine(float value, float debuff_Time)
    {
        isStunned = true;

        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.StunEffect, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);

        yield return new WaitForSeconds((value + 1) * debuff_Time);

        isStunned = false;
        effectLifeCycle.DeActive();
    }

    IEnumerator TerrifiedDebuffCoroutine(float value, float debuff_Time)
    {
        isTerrified = true;
        ApplyTerrifiedSlowDebuff(0.4f);
        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.Terrified, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);
        yield return new WaitForSeconds((value + 1) * debuff_Time);

        RemoveTerrifiedSlowDebuff();
        isTerrified = false;
        effectLifeCycle.DeActive();
    }

    IEnumerator CharmedDebuffCoroutine(float value, float debuff_Time)
    {
        isCharmed = true;
        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.CharmedEffect, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);
        ApplyCharmSlowDebuff(0.4f);
        yield return new WaitForSeconds((value + 1) * debuff_Time);

        RemoveCharmSlowDebuff();

        isCharmed = false;
        effectLifeCycle.DeActive();
    }

    IEnumerator HealBenDebuffCoroutine(float value, float debuff_Time)
    {
        isHealBan = true;

        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.EnemyHealBenEffect, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);

        yield return new WaitForSeconds((value + 1) * debuff_Time);

        isHealBan = false;
        effectLifeCycle.DeActive();
    }

    IEnumerator ShieldBenDebuffCoroutine(float value, float debuff_Time)
    {
        isShieldBan = true;

        enemy.shield = 0;
        enemy.UpdateShieldBar();

        GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.EnemyShieldBenEffect, transform.position);
        ObjectLifecycleManager effectLifeCycle = effect.GetComponent<ObjectLifecycleManager>();
        effectLifeCycle.SetParent(enemy.debuffPrefabParent);
        effectLifeCycle.Init(ObjectLifecycleManager.ObjectType.Effect);

        yield return new WaitForSeconds((value + 1) * debuff_Time);

        isShieldBan = false;
        effectLifeCycle.DeActive();
    }

    public void ResetToStartPosition(float duration)
    {
        if (!enemy.enemyMove.enabled)
            return;
        enemy.enemyMove.PosNum = 0;
        enemy.transform.DOMove(enemy.enemyMove.wayPos[0].position, duration).SetEase(Ease.OutBack);
    }


    public void ActivateDebuffClear()
    {
        isSlowActive = false;
        isDotActive = false;
        isWeaknessActive = false;
        isStunned = false;

        debuffCount = 0;

        if (weaknessCoroutine != null)
        {
            StopCoroutine(weaknessCoroutine);
            RemoveWeaknessDebuff();
            weaknessCoroutine = null;
        }

        if (slowDebuffCoroutine != null)
        {
            StopCoroutine(slowDebuffCoroutine);
            RemoveSlowDebuff();
            slowDebuffCoroutine = null;
        }

        if (dotDamageCoroutine != null)
        {
            StopCoroutine(dotDamageCoroutine);
            dotDamageCoroutine = null;
        }

        if (stunDebuffCoroutine != null)
        {
            StopCoroutine(stunDebuffCoroutine);
            stunDebuffCoroutine = null;
        }

        if (terrifiedDebuffCoroutine != null)
        {
            StopCoroutine(terrifiedDebuffCoroutine);
            RemoveTerrifiedSlowDebuff();
            terrifiedDebuffCoroutine = null;
        }

        if (charmedDebuffCoroutine != null)
        {
            StopCoroutine(charmedDebuffCoroutine);
            RemoveCharmSlowDebuff();
            charmedDebuffCoroutine = null;
        }

        if (healBenDebuffCoroutine != null)
        {
            StopCoroutine(healBenDebuffCoroutine);
            healBenDebuffCoroutine = null;
        }

        if (shieldBenDebuffCoroutine != null)
        {
            StopCoroutine(shieldBenDebuffCoroutine);
            shieldBenDebuffCoroutine = null;
        }

        List<ObjectLifecycleManager> lifecycleManagers = new List<ObjectLifecycleManager>();
        for (int i = 0; i < enemy.debuffPrefabParent.childCount; i++)
        {
            ObjectLifecycleManager lifecycleManager = enemy.debuffPrefabParent.GetChild(i).GetComponent<ObjectLifecycleManager>();
            lifecycleManagers.Add(lifecycleManager);
        }
        for (int i = 0; i < lifecycleManagers.Count; i++)
        {
            lifecycleManagers[i].DeActive();
        }
    }
}
