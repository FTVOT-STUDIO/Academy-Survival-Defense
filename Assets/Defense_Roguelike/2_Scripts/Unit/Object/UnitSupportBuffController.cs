using System.Collections.Generic;
using UnityEngine;

public class UnitSupportBuffController : MonoBehaviour
{
    Unit unit;
    public List<GameObject> curBuffFieldSlots = new List<GameObject>();
    public float supportDetectRadius;
    public LayerMask fieldLayer;
    public Dictionary<int, GameObject> activeParticles = new Dictionary<int, GameObject>();
    public GameObject supportBuffRangeCircle;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void DisableMultSupportBuffEffect()
    {
        foreach (var particle in activeParticles.Values)
        {
            if (particle != null)
                particle.SetActive(false);
        }
        activeParticles.Clear();
    }

    public void ApplySupportBuffToNearbyFields()
    {
        if (unit.unitEvent.IsSilenced)
            return;

        if (unit.unitData.type == Type.Support && unit.unitData.unitSupportType == UnitSupportType.Mult)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, supportDetectRadius, fieldLayer);

            if (curBuffFieldSlots != null && curBuffFieldSlots.Count != 0)
            {
                ResetBuffFieldSlot();
                DisableMultSupportBuffEffect();
            }

            foreach (var hit in hits)
            {
                if (hit.gameObject == unit.unitDrag.curBatchFieldTrigger.gameObject)
                    continue;
                curBuffFieldSlots.Add(hit.gameObject);
            }

            foreach (var hit in curBuffFieldSlots)
            {
                FieldPlacementTrigger slot = hit.GetComponent<FieldPlacementTrigger>();

                if (slot != null)
                {
                    slot.RegisterUnitPlacedEvent(unit.unitEvent.onUnitPlacedAction);
                    slot.RegisterUnitRemovedEvent(unit.unitEvent.onUnitRemovedAction);

                    if (slot.curUnit != null)
                    {
                        slot.curUnit.anim.SetTrigger("Attack");
                    }
                }
            }
            unit.anim.SetTrigger("Attack");
        }
    }

    public void ResetBuffFieldSlot()
    {
        foreach (var hit in curBuffFieldSlots)
        {
            FieldPlacementTrigger slot = hit.GetComponent<FieldPlacementTrigger>();
            slot.RemoveUnitPlacedEvent(unit.unitEvent.onUnitPlacedAction, unit.unitEvent.onUnitRemovedAction);
            if (slot.curUnit != null)
            {
                slot.curUnit.anim.SetTrigger("Attack");
            }

        }
        curBuffFieldSlots.Clear();
    }

    public void UpdateSupportRangeCircle()
    {
        if (unit.unitData.type == Type.Support && unit.unitData.unitSupportType == UnitSupportType.Mult)
        {
            supportDetectRadius = unit.unitData.supportDetectRadius;
            supportBuffRangeCircle.SetActive(true);
            SetSupportBuffRangeCircleSacle();
        }
        else
        {
            supportBuffRangeCircle.SetActive(false);
        }
    }

    public void SetSupportRangeCircle(bool isActive)
    {
        supportBuffRangeCircle.SetActive(isActive);
    }

    void SetSupportBuffRangeCircleSacle()
    {
        supportBuffRangeCircle.transform.localScale = new Vector3(supportDetectRadius / 4f, supportDetectRadius / 4f, 1);
    }
}
