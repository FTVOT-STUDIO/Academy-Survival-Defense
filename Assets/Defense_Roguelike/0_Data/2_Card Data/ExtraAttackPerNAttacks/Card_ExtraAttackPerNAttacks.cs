using UnityEngine;

[CreateAssetMenu(fileName = "Card_ExtraAttackPerNAttacks", menuName = "Card Effect/Card_ExtraAttackPerNAttacks")]
public class Card_ExtraAttackPerNAttacks : ScriptableObject, ICardEffect, ICardEventRegister, ICardBaseStatCalculator, ICardLevelStatCalculator
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        if (GameManager.Inst.skillCardManager.GetCardLevel(cardData) > 1)
        {
            GameStatManager.Inst.AddExtraAttackTriggerInterval(-1);
        }
    }

    void ApplyUnitAttackEvent(Unit unit)
    {
        if (unit.hitCount > 0 && unit.hitCount % GameStatManager.Inst.ExtraAttackTriggerInterval == 0)
        {
            unit.unitWeapon.UnitAttack();
        }
    }

    void ApplySephiraAttackEvent(SephiraUnit unit)
    {
        if (unit.hitCount > 0 && unit.hitCount % GameStatManager.Inst.ExtraAttackTriggerInterval == 0)
        {
            unit.UnitAttack();
        }
    }

    public void Register()
    {
        InGameEventManager.Inst.UnitAttackEvent += ApplyUnitAttackEvent;
        InGameEventManager.Inst.SephiraAttackEvent += ApplySephiraAttackEvent;
        GameStatManager.Inst.SetExtraAttackTriggerInterval(7);
    }

    public float CalculateLevelStat(CardData cardData)
    {
        return GameStatManager.Inst.ExtraAttackTriggerInterval;
    }

    public float CalculateBaseStat(CardData cardData)
    {
        if (GameManager.Inst.skillCardManager.GetCardLevel(cardData) <= 0)
            return 7;

        else
            return GameStatManager.Inst.ExtraAttackTriggerInterval - 1;
    }
}
