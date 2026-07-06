using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public ScriptableObject skillData;
    public enum EnemyType { Normal, Elite, Boss, JointOperationBoss }
    public string enemyName;
    [Space]
    public EnemyType enemyType;
    public EnemyAttributeStats[] attributeStats;
    [Space]
    public RuntimeAnimatorController[] animatorController;
    [Space]
    public int[] coinDropPerDifficulty;
    public int[] darkMatteCoinPerDifficulty;
    [Space]
    public Sprite[] portrait;
    [Space]
    public float[] damage;
    public double[] maxHealth;
    public double[] maxShield;
    public float[] defenseAmount;
    public float[] speed;
    public float[] attackSpeed;
    public float range;
    public float coolTime;
}

[System.Serializable]
public class EnemyAttributeStats
{
    public AttributeModifierData[] resistances;
    public AttributeModifierData[] weaknesses;
}