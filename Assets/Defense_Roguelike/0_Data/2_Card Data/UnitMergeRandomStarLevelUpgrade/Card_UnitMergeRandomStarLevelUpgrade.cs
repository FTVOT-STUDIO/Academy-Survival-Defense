using UnityEngine;

[CreateAssetMenu(fileName = "Card_UnitMergeRandomStarLevelUpgrade", menuName = "Card Effect/Card_UnitMergeRandomStarLevelUpgrade")]
public class Card_UnitMergeRandomStarLevelUpgrade : ScriptableObject, ICardEffect, ICardEventRegister
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        if (!DebuffManager.Inst.isMergeRandomizedDebuff)
            DebuffManager.Inst.isMergeRandomizedDebuff = true;

        InGameEventManager.Inst.unitMergeStarLevelUpgradeAmount += Amount;


    }

    void ApplyMergeEvent(Unit unit)
    {
        if (Random.value <= InGameEventManager.Inst.unitMergeStarLevelUpgradeAmount)
        {
            unit.unitEvent.ApplyLevelUpUnit(1);
        }
    }

    public void Register()
    {
        InGameEventManager.Inst.UnitMergeEvent += ApplyMergeEvent;
    }
}
