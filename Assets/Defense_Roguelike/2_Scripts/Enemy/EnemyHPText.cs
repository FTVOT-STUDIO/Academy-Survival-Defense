using TMPro;
using UnityEngine;

public class EnemyHPText : MonoBehaviour
{
    Enemy enemy;

    TMP_Text text;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        text = GetComponent<TextMeshPro>();
    }

    void LateUpdate()
    {
        text.text = enemy.health.ToString("F0");
    }
}
