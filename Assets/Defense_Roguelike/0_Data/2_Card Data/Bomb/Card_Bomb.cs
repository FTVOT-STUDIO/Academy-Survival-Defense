using UnityEngine;

[CreateAssetMenu(fileName = "Card_Bomb", menuName = "Card Effect/Card_Bomb")]
public class Card_Bomb : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        WeaponManager.Inst.baseBoombDamage = Amount;
        if (!WeaponManager.Inst.isBoomb)
            WeaponManager.Inst.isBoomb = true;
    }
}
