using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitWeapon : MonoBehaviour
{
    Unit unit;

    [Header("# Attack Timer")]
    [Space]
    public float attackTime;
    [Space(15)]
    [Header("# Skill  Timer")]
    [Space]
    public float skillTime;
    [Space(15)]
    [Header("# Skill State")]
    public GameObject weaponObj;
    public bool isSkillActive;
    [HideInInspector] public int sameTargetHitCount;

    public event Action OnAttack;
    public event Action OnSkill;

    private Coroutine skillCoroutine;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }


    void Update()
    {
        if (!unit.isLive || !GameManager.Inst.isGameStart || GameManager.Inst.isPause || unit.unitEvent.IsUnitLocked || unit.unitEvent.IsIgnored || unit.unitEvent.IsSilenced || unit.unitData.type == Type.Support)
            return;

        AttackCoolTime();

        SkillCoolTime();
    }

    void LateUpdate()
    {
        if (skillTime > unit.coolTime)
        {
            skillTime = unit.coolTime;
        }
    }

    public void SetSkillCoroutine(IEnumerator coroutine)
    {
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }

        skillCoroutine = StartCoroutine(coroutine);
    }

    public void DeleteSkillCoroutine()
    {
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }
    }

    public void AttackCoolTime()
    {
        attackTime = Mathf.Min(attackTime + Time.deltaTime, unit.attackSpeed);
        unit.UpdateAttackSpeedBar();
        if (attackTime >= unit.attackSpeed && (unit.unitScanner.nearestTarget != null || unit.unitScanner.lowestHealthTarget != null || unit.unitScanner.nearestUnit != null))
        {
            attackTime = 0;
            float randomValue = Random.Range(0f, 100f);

            if (randomValue <= unit.hitChance)
            {
                skillTime = Mathf.Min(skillTime + 1, unit.coolTime);
                UnitAttack();
                OnAttack?.Invoke();
            }
            else
            {
                unit.anim.SetTrigger("Attack");
                FloatingText floatingText = PoolManager.Inst.Get(PoolManager.PrefabType.FloatingText, transform.position).GetComponent<FloatingText>();
                floatingText.InitializeText("MISS", 4, Color.white);
            }
            unit.RecalculateStats();
        }
    }


    public void SkillCoolTime()
    {
        switch (unit.unitData.type)
        {
            case Type.Healer when unit.unitScanner.lowestHealthTarget == null:
            case Type.Support when unit.unitScanner.nearestUnit == null:
                return;
            case Type.Melee when unit.unitScanner.nearestTarget == null:
            case Type.Ranged when unit.unitScanner.nearestTarget == null:
            case Type.Tank when unit.unitScanner.nearestTarget == null:
                return;
        }

        if (unit.unitSkill is not ICoolTimeSkill && unit.unitSkill is not ICoolTimePassive && unit.unitSkill is not ICoolTimeOverride)
            return;


        //skillTime = Mathf.Min(skillTime + Time.deltaTime, unit.coolTime);

        if (skillTime >= unit.coolTime)
        {
            if (unit.unitSkill is ICoolTimeOverride)
                return;
            float randomValue = Random.Range(0f, 100f);

            skillTime = unit.coolTime;
            skillTime = 0;
            if (randomValue <= unit.hitChance)
            {
                UnitSkill();
                OnSkill?.Invoke();
                InGameEventManager.Inst.UnitSkillEvent?.Invoke(unit);
            }
            else
            {
                unit.anim.SetTrigger("Attack");
                FloatingText floatingText = PoolManager.Inst.Get(PoolManager.PrefabType.FloatingText, transform.position).GetComponent<FloatingText>();
                floatingText.InitializeText("MISS", 4, Color.white);
            }
            unit.RecalculateStats();
        }
    }
    public void UnitAttack()
    {
        bool targetExists = unit.unitScanner.nearestTarget != null || unit.unitScanner.nearestUnit != null || unit.unitScanner.lowestHealthTarget != null;

        if (!targetExists)
            return;

        switch (unit.unitData.type)
        {
            case Type.Melee:
            case Type.Tank:
                Enemy enemy = unit.unitScanner.GetCurrentTarget();
                if (enemy == null)
                    return;
                if (unit.unitSkill is IAttackPassive attackPassive1)
                {
                    attackPassive1.ApplyAttackPassive(unit);
                }

                if (unit.unitSkill is IAttackSkill attackSkill1)
                {
                    attackSkill1.ApplyIAttackSkill(unit);
                }

                if (unit.unitSkill is not IAttackPassive and not IAttackSkill)
                {
                    TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, 0, 0, 1, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy, UnitUtility.DamageType.Fixed);
                }

                break;
            case Type.Ranged:
            case Type.Healer:
                if (unit.unitSkill is IAttackPassive attackPassive)
                {
                    attackPassive.ApplyAttackPassive(unit);
                }

                if (unit.unitSkill is IAttackSkill attackSkill)
                {
                    attackSkill.ApplyIAttackSkill(unit);
                }
                break;
        }
        switch (unit.unitData.type)
        {
            case Type.Melee:
            case Type.Tank:
            case Type.Ranged:
                Enemy enemy = unit.unitScanner.nearestTarget.transform.GetComponent<Enemy>();
                if (enemy == null)
                    return;
                SetDir(enemy);
                if (unit.unitScanner.nearestTarget == unit.unitScanner.lastNearestTarget)
                    sameTargetHitCount++;
                else
                    sameTargetHitCount = 0;

                break;

            case Type.Support:
                if (unit.unitScanner.nearestUnit != null)
                {
                    if (unit.unitData.unitSupportType == UnitSupportType.Single)
                    {
                        if (unit.name == unit.unitScanner.nearestUnit.GetComponent<Unit>().name || unit.unitScanner.nearestUnit.GetComponent<Unit>() == null)
                        {
                            return;
                        }

                        if (unit.unitSkill is IAttackPassive passive1)
                        {
                            passive1.ApplyAttackPassive(unit);
                        }

                        if (unit.unitSkill is ISupportApplyOnce supportAttackOnce)
                        {
                            supportAttackOnce.ApplySupportApplyOnce(unit);
                        }
                    }
                }

                break;
            case Type.Healer:

                Unit lowHealthUnit = unit.unitScanner.lowestHealthTarget.transform.GetComponent<Unit>();

                if (lowHealthUnit.health < lowHealthUnit.maxHealth && lowHealthUnit.isLive)
                {
                    unit.sp.flipX = lowHealthUnit.sp.flipX;

                    lowHealthUnit.unitEvent.ApplyGetHeal((float)unit.damageAmout, unit.healingMultiplier, lowHealthUnit.healingReceivedMultiplier, unit.unitData);

                    Transform targetTrans = lowHealthUnit.transform;
                    Vector3 batchPos = new Vector3(targetTrans.position.x, targetTrans.position.y + 1f, targetTrans.position.z);
                    EffectManager.Inst.Get(EffectManager.PrefabType.HealEffect, batchPos);

                    if (unit.unitScanner.lowestHealthTarget == unit.unitScanner.lastLowestHealthTarget)
                        sameTargetHitCount++;
                    else
                        sameTargetHitCount = 0;
                }
                break;
        }

        if (unit.unitData.type != Type.Support)
            unit.anim.SetTrigger("Attack");

        unit.hitCount++;
        InGameEventManager.Inst.UnitAttackEvent?.Invoke(unit);
        if (unit.hitCount >= int.MaxValue - 1000)
            unit.hitCount = 0;
    }

    public void UnitSkill()
    {
        if (unit.unitSkill is ICoolTimeSkill CoolTimeskill)
        {
            CoolTimeskill.ApplyCoolTimeSkill(unit);
        }

        if (unit.unitSkill is ICoolTimePassive CoolTimePassive)
        {
            CoolTimePassive.ApplyPassive(unit);
        }

        if (unit.unitData.type != Type.Support)
            unit.anim.SetTrigger("Attack");
    }

    /// <summary>
    /// 대상에게 피해를 계산하여 적용한다.
    /// 치명타, 방어력 관통, 속성 저항 무시, 체력 비례 피해 등을 포함한다.
    /// </summary>
    /// <param name="attributeType">가하는 피해의 속성</param>
    /// <param name="baseDamage">기본 데미지</param>
    /// <param name="criticalPer">치명타 확률 (0 ~ 1)</param>
    /// <param name="criticalDamage">치명타 피해 배율</param>
    /// <param name="defensePenetration">방어력 관통 비율 (0 ~ 1)</param>
    /// <param name="attributeResistanceIgnore">속성 저항 무시 비율 (0 ~ 1)</param>
    /// <param name="enemy">현재 유닛의 목표 타겟</param>
    /// <param name="damageType">체력 비례 데미지 판별</param>
    /// <param name="ratio">체력 비례 피해 비율</param>
    /// <param name="hitEffect">피격 이펙트 프리팹</param>
    /// <param name="isPlaySfx">피격 사운드 재생 여부</param>
    /// <param name="hitSound">피격 사운드 클립</param>
    /// <returns>최종 적용된 실제 데미지 값</returns>
    public double TakeDamage(AttributeData.AttributeType attributeType, double baseDamage, float criticalPer, float criticalDamage, float singleTargetDamage, float defensePenetration, float attributeResistanceIgnore, float attributeDamageMultiplier, UnitUtility.AttackType attackType, float attackTypeMultiplier, Enemy enemy,
     UnitUtility.DamageType damageType = UnitUtility.DamageType.Fixed, float ratio = 0,
     EffectManager.PrefabType hitEffect = EffectManager.PrefabType.EnemyHitEffect,
     bool isPlaySfx = true, AudioManager.Sfx hitSound = AudioManager.Sfx.EnemyHit)
    {
        double finalDamage = baseDamage;
        finalDamage *= singleTargetDamage;

        finalDamage *= attackTypeMultiplier;
        finalDamage += UnitUtility.CalculateDamage(damageType, unit.damageAmout, ratio, enemy);

        if (damageType != UnitUtility.DamageType.Fixed)
            criticalPer = 0;

        enemy.OnDamage(attributeType, finalDamage, criticalPer, criticalDamage, defensePenetration, attributeResistanceIgnore, attributeDamageMultiplier, attackType, unit.gameObject, () =>
        {
            unit.unitEvent.ApplyKillEffect();
            if (unit.unitSkill is IKillEvent unitKillEvent)
            {
                unitKillEvent.ApplyKillEvent(unit);
            }
        });

        EffectManager.Inst.Get(hitEffect, enemy.transform.position);

        if (isPlaySfx)
            AudioManager.Inst.PlaySfx(hitSound);

        return finalDamage;
    }

    void SetDir(Enemy enemy)
    {
        Vector3 myPos = transform.position;
        Vector3 enemyPos = enemy.transform.position;
        if (myPos.x < enemyPos.x)
        {
            unit.sp.flipX = false;
        }
        else
        {
            unit.sp.flipX = true;
        }
    }

    public void RemoveAllEventHandler()
    {
        OnAttack = null;
        OnSkill = null;
    }
}
