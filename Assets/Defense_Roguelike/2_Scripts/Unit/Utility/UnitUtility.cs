using UnityEngine;

public static class UnitUtility
{
    public enum AttackType { None, NormalAttack, Skill, Ability }
    public enum AttackRangeType { Single, Area }
    public enum DamageType { Fixed, ByMaxHP, ByCurrentHP, ByLostHP }
    public static float CalculateSkillTrigger(float baseChance, float mainChance = 0f)
    {
        return Mathf.Clamp(baseChance + mainChance, 0f, 1f);
    }

    public static double CalculateDamage(DamageType damageType, double baseDamage, float ratio, Enemy enemy)
    {
        double finalDamage = baseDamage;

        switch (damageType)
        {
            case DamageType.ByMaxHP:
                finalDamage += enemy.maxHealth * ratio;
                break;

            case DamageType.ByCurrentHP:
                finalDamage += enemy.health * ratio;
                break;

            case DamageType.ByLostHP:
                finalDamage += (enemy.maxHealth - enemy.health) * ratio;
                break;

            case DamageType.Fixed:
            default:
                finalDamage = 0;
                break;
        }

        if (damageType != DamageType.Fixed)
        {
            finalDamage *= 1f - GameStatManager.Inst.EnemyHpRatioDamageReduction;
        }

        return finalDamage;
    }

    public static Vector3 GetEnemyColliderCenter(Enemy enemy)
    {
        return enemy.hitboxColl.bounds.center;
    }

    public static int GetRandomValue(int start, int end)
    {
        int ran = UnityEngine.Random.Range(start, end);
        return ran;
    }
}
