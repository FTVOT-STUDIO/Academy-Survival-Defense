using System;
using System.Collections.Generic;
using UnityEngine;


public class UnitScanner : MonoBehaviour
{
    Unit unit;

    [Header("# Enemy Target")]
    public LayerMask targetLayer;
    public Transform nearestTarget;
    public Transform lastNearestTarget;
    [Space(15)]
    [Header("# Unit Target")]
    public Transform nearestUnit;
    public Transform lastNearestUnit;
    [Space(15)]
    [Header("# Heal Target")]
    public Transform lowestHealthTarget;
    public Transform lastLowestHealthTarget;
    [HideInInspector] public GameObject buffUnit;
    [HideInInspector] public Transform result = null;
    Collider2D[] targets;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    void FixedUpdate()
    {
        if (!unit.isLive || unit.unitEvent.IsUnitLocked || GameManager.Inst.isPause || !GameManager.Inst.isGameStart || unit.unitEvent.IsIgnored || unit.unitEvent.IsSilenced)
            return;
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        targets = Physics2D.OverlapCircleAll(rayOrigin, unit.range, targetLayer);

        switch (unit.unitData.type)
        {
            case Type.Healer:
                lastLowestHealthTarget = lowestHealthTarget;
                lowestHealthTarget = GetHealingTarget();
                break;
            default:
                lastNearestTarget = nearestTarget;
                nearestTarget = GetTarget();
                break;
        }
    }

    void LateUpdate()
    {
        if (unit.unitData.unitSupportType != UnitSupportType.Mult || unit.unitSupportBuffController.curBuffFieldSlots.Count == 0)
            return;

        for (int i = 0; i < unit.unitSupportBuffController.curBuffFieldSlots.Count; i++)
        {
            var slot = this.unit.unitSupportBuffController.curBuffFieldSlots[i];
            if (slot == null)
                continue;

            var targetUnit = slot.GetComponent<FieldPlacementTrigger>().curUnit;

            if (targetUnit != null)
            {
                if (!this.unit.unitSupportBuffController.activeParticles.ContainsKey(i))
                {
                    GameObject particleObj = EffectManager.Inst.Get(this.unit.unitData.effectPrefab, slot.transform.position);
                    this.unit.unitSupportBuffController.activeParticles.Add(i, particleObj);
                }
            }
            else
            {
                if (this.unit.unitSupportBuffController.activeParticles.ContainsKey(i))
                {
                    this.unit.unitSupportBuffController.activeParticles[i].SetActive(false);
                    this.unit.unitSupportBuffController.activeParticles.Remove(i);
                }
            }
        }
    }



    private Transform GetTarget()
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

    private Transform GetHealingTarget()
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

    public Enemy GetCurrentTarget()
    {
        if (nearestTarget == null) return null;

        var enemy = nearestTarget.GetComponent<Enemy>();
        if (enemy == null || !enemy.isLive) return null;

        return enemy;
    }

    public Unit GetCurrentBuffedUnit()
    {
        if (nearestUnit == null) return null;

        var Unit = nearestUnit.GetComponent<Unit>();
        if (Unit == null) return null;

        return Unit;
    }

    public Unit GetCurrentHealingTarget()
    {
        if (lowestHealthTarget == null) return null;

        var Unit = lowestHealthTarget.GetComponent<Unit>();
        if (Unit == null || !Unit.isLive) return null;

        return Unit;
    }

    public Enemy GetNearestTarget(float range)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        targets = Physics2D.OverlapCircleAll(rayOrigin, range, targetLayer);

        Enemy result = null;
        float minSqrDistance = float.MaxValue;
        Vector3 myPosition = transform.position;

        for (int i = 0; i < targets.Length; i++)
        {
            Collider2D col = targets[i];
            Enemy enemy = col.GetComponentInParent<Enemy>();

            float sqrDistance = (enemy.transform.position - myPosition).sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                result = enemy;
            }
        }
        return result;
    }

    public List<Enemy> GetNearestTargets(float range)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        Collider2D[] targets = Physics2D.OverlapCircleAll(rayOrigin, range, targetLayer);

        List<Enemy> results = new List<Enemy>(targets.Length);

        for (int i = 0; i < targets.Length; i++)
        {
            Collider2D col = targets[i];
            if (col == null) continue;

            Enemy enemy = col.GetComponentInParent<Enemy>();

            if (!results.Contains(enemy))
                results.Add(enemy);
        }
        return results;
    }

    public List<Unit> GetBuffedUnits()
    {
        List<Unit> units = new List<Unit>();

        for (int i = 0; i < unit.unitSupportBuffController.curBuffFieldSlots.Count; i++)
        {
            FieldPlacementTrigger curField = unit.unitSupportBuffController.curBuffFieldSlots[i].GetComponent<FieldPlacementTrigger>();

            if (curField.curUnit == null)
                continue;

            units.Add(curField.curUnit);

        }
        return units;
    }

    public Unit GetLowestHealthUnit(float range)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        Collider2D[] targets = Physics2D.OverlapCircleAll(rayOrigin, range, targetLayer);

        Unit result = null;
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
            result = unit;
        }

        return result;
    }

    public List<Unit> GetDamagedUnits(float range)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        Collider2D[] targets = Physics2D.OverlapCircleAll(rayOrigin, range, targetLayer);

        List<Unit> result = new List<Unit>();
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
            result.Add(unit);
        }

        return result;
    }
}
