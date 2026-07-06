using UnityEngine;

[CreateAssetMenu(fileName = "Card_FreeSpawnProvision", menuName = "Card Effect/Card_FreeSpawnProvision")]
public class Card_FreeSpawnProvision : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        GameManager.Inst.unitSpawn.ModifyCoinFreeSpawnCount((int)Amount);
    }
}
