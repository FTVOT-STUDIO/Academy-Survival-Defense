using UnityEngine;

[CreateAssetMenu(fileName = "Card_InstantRecoverSephiraHp", menuName = "Card Effect/Card_InstantRecoverSephiraHp")]
public class Card_InstantRecoverSephiraHp : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        SephiraUnit sephiraUnit = FieldManager.Inst.fieldSephira;
        if (sephiraUnit.health >= sephiraUnit.maxHealth)
            return;
        sephiraUnit.sephiraUnitEvent.ApplyHeal(sephiraUnit.maxHealth, GameStatManager.Inst.GameHealingMultiplier, GameStatManager.Inst.GameHealingReceivedMultiplier);
    }
}
