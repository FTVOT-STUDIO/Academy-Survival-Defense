using UnityEngine;

[CreateAssetMenu(fileName = "Card_SephiraSacrificeForDarkMatter", menuName = "Card Effect/Card_SephiraSacrificeForDarkMatter")]
public class Card_SephiraSacrificeForDarkMatter : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        SephiraUnit sephira = FieldManager.Inst.fieldSephira;

        float healthRatio = sephira.health / sephira.maxHealth;

        sephira.baseMaxHealth /= 2;
        // sephira.sephiraUnitEvent.ApplyHeal(sephira.maxHealth * healthRatio, 1, 1);
        sephira.health = Mathf.Min(sephira.health, sephira.maxHealth);

        GameManager.Inst.ModifyDarkMatteCoin(100);
    }
}
