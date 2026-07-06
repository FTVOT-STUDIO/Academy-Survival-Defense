using UnityEngine;

[CreateAssetMenu(fileName = "Card_EnemyInstantDeath", menuName = "Card Effect/Card_EnemyInstantDeath")]
public class Card_EnemyInstantDeath : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        if (!WeaponManager.Inst.isInsDeath)
            WeaponManager.Inst.isInsDeath = true;

        WeaponManager.Inst.insDeathProb += 0.01f;
    }
}
