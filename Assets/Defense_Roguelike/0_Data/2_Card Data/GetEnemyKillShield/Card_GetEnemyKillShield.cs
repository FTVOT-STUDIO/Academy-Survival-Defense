using UnityEngine;

[CreateAssetMenu(fileName = "Card_GetEnemyKillShield", menuName = "Card Effect/Card_GetEnemyKillShield")]
public class Card_GetEnemyKillShield : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        if (!WeaponManager.Inst.isKillShieldEnabled)
            WeaponManager.Inst.isKillShieldEnabled = true;

        WeaponManager.Inst.shieldAmount += Amount;
    }
}
