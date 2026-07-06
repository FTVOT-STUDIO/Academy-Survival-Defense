using UnityEngine;

[CreateAssetMenu(fileName = "Card_DrainHealth", menuName = "Card Effect/Card_DrainHealth")]
public class Card_DrainHealth : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        if (!WeaponManager.Inst.isHealthDrainEnabled)
            WeaponManager.Inst.isHealthDrainEnabled = true;

        WeaponManager.Inst.drainvalue += Amount;
    }
}
