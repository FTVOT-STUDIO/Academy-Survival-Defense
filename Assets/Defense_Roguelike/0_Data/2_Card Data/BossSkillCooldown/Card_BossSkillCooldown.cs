using UnityEngine;

[CreateAssetMenu(fileName = "Card_BossSkillCooldown", menuName = "Card Effect/Card_BossSkillCooldown")]
public class Card_BossSkillCooldown : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameStatManager.Inst.AddGameBossCoolTime(Amount);
    }
}
