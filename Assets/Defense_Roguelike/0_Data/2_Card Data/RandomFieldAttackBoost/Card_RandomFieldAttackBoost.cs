using UnityEngine;

[CreateAssetMenu(fileName = "Card_RandomFieldAttackBoost", menuName = "Card Effect/Card_RandomFieldAttackBoost")]
public class Card_RandomFieldAttackBoost : ScriptableObject, ICardEffect
{
    public void ApplyCardEffect(CardData cardData, float Amount)
    {
        int ranIndex = Random.Range(0, FieldManager.Inst.fieldSlots.Length);
        FieldPlacementTrigger trigger = FieldManager.Inst.fieldSlots[ranIndex].GetComponent<FieldPlacementTrigger>();

        GameObject marker = PoolManager.Inst.Get(PoolManager.PrefabType.FieldAttackBoostMarker, trigger.gameObject.transform.position);

        marker.transform.SetParent(trigger.transform);

        trigger.RegisterUnitPlacedEvent(unit => unit.unitEvent.ApplyDamageStats(1f, true));
        trigger.RegisterUnitRemovedEvent(unit => unit.unitEvent.ApplyDamageStats(1f, false));
    }
}
