using UnityEngine;

[CreateAssetMenu(fileName = "Card_SkillCardDualPickRarityBoost", menuName = "Card Effect/Card_SkillCardDualPickRarityBoost")]
public class Card_SkillCardDualPickRarityBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddShowSkillCardCount(-1);
        GameStatManager.Inst.AddCardSpawnProbabilityBonus(5);
    }
}
