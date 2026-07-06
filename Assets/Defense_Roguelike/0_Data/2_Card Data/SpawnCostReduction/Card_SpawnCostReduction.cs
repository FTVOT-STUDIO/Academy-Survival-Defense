using UnityEngine;

[CreateAssetMenu(fileName = "Card_SpawnCostReduction", menuName = "Card Effect/Card_SpawnCostReduction")]
public class Card_SpawnCostReduction : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameManager.Inst.unitSpawnCost -= (int)Amount;
    }
}
