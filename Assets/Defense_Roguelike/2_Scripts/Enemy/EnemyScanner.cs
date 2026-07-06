using UnityEngine;
using VInspector;

public class EnemyScanner : MonoBehaviour
{
    Enemy enemy;
    [Header("# Target")]
    [SerializeField] Collider2D[] targets;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask charmedTargetLayer;
    public Transform nearestTarget;
    public float range;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void FixedUpdate()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause || enemy.enemyEvent.isStunned || enemy.enemyMove.isBarrierTouched)
            return;

        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;

        LayerMask layerToUse = enemy.enemyEvent.isCharmed ? charmedTargetLayer : targetLayer;

        targets = Physics2D.OverlapCircleAll(rayOrigin, range, layerToUse);

        nearestTarget = enemy.enemyEvent.isCharmed ? GetCharmedTarget() : GetTarget();
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
            Unit unit = targetTransform.GetComponentInParent<Unit>();
            if (unit != null && unit.unitEvent.IsIgnored)
                continue;

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

    Transform GetCharmedTarget()
    {
        Transform result = null;
        float minSqrDistance = float.MaxValue;

        Vector3 myPosition = transform.position;
        GameObject myEnemyObject = enemy.gameObject;

        for (int i = 0; i < targets.Length; i++)
        {
            Collider2D col = targets[i];
            if (col == null) continue;

            Enemy targetEnemy = col.GetComponentInParent<Enemy>();
            if (targetEnemy.gameObject == myEnemyObject) continue;

            Vector3 diff = col.transform.position - myPosition;
            float sqrDistance = diff.sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                result = col.transform.parent;
            }
        }

        return result;
    }
}
