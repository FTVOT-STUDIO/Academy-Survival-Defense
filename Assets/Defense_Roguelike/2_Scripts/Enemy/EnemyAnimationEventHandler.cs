using UnityEngine;

public class EnemyAnimationEventHandler : MonoBehaviour
{
    Enemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void TriggerMoveEventWait()
    {
        enemy.WaitForAnimationMoveEvent();
    }
}
