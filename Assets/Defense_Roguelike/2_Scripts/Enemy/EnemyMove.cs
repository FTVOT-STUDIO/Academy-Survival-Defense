using System;
using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Vector2 nextVec;
    public Transform[] wayPos;
    public bool isPathLoop;

    public bool isBelow = false;

    SpriteRenderer sp;
    public int PosNum = 0;

    Enemy enemy;
    public bool isMoving = true;
    public bool isBarrierTouched = false;
    public bool isInContactWithTarget = false;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        sp = GetComponentInChildren<SpriteRenderer>();
    }

    void OnDisable()
    {
        PosNum = 0;
        isBelow = false;
        isBarrierTouched = false;
    }

    void Update()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause || enemy.enemyEvent.isStunned || !isMoving || enemy.isDummy
            || enemy.enemyEvent.isGravityPull)
            return;

        if (!enemy.isLive)
            return;

        if (PosNum >= wayPos.Length)
            return;

        Movement();
    }

    void LateUpdate()
    {
        float moveY = nextVec.y;
        float moveX = nextVec.x;

        if (moveY > 0.01f)
        {
            isBelow = false;
            sp.sortingOrder = 100;
        }
        else if (moveY < -0.01f)
        {
            isBelow = true;
            sp.sortingOrder = enemy.startSortingOrder;
        }
        else
        {
            if (isBelow)
                sp.sortingOrder = 100;
            else
                sp.sortingOrder = enemy.startSortingOrder;
        }
    }

    void Movement()
    {
        isInContactWithTarget = false;
        if (!enemy.enemyEvent.isTerrified)
        {
            Transform target = enemy.enemyScanner.nearestTarget;

            if (target != null)
            {
                Vector3 diff = target.position - transform.position;

                if (diff.sqrMagnitude <= 0.8f * 0.8f)
                {
                    isInContactWithTarget = true;
                    return;
                }
            }
        }

        Vector3 targetPosition;
        Vector3 direction;

        if (enemy.enemyEvent.isTerrified || enemy.enemyEvent.isCharmed)
        {
            targetPosition = wayPos[Mathf.Max(PosNum - 1, 0)].transform.position;
        }
        else
        {
            targetPosition = wayPos[PosNum].transform.position;
        }

        direction = targetPosition - transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemy.speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            if (enemy.enemyEvent.isTerrified || enemy.enemyEvent.isCharmed)
            {
                PosNum = Mathf.Max(PosNum - 1, 0);
            }
            else
            {
                PosNum++;
                if (PosNum >= wayPos.Length)
                {
                    if (isPathLoop)
                        PosNum = 0;
                    else
                        return;
                }
            }
        }

        nextVec = direction;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            isMoving = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            isMoving = true;
        }
    }
}
