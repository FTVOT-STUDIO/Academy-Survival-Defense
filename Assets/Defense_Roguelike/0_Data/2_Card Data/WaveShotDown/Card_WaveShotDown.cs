using UnityEngine;

[CreateAssetMenu(fileName = "Card_WaveShotDown", menuName = "Card Effect/Card_WaveShotDown")]
public class Card_WaveShotDown : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        if (GameManager.Inst.isBossActive)
            return;

        GameManager.Inst.timer = 0;
    }
}
