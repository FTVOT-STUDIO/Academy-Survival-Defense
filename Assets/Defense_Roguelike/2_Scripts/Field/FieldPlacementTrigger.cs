using System;
using UnityEngine;

public class FieldPlacementTrigger : MonoBehaviour
{
    public Unit curUnit;

    public Action<Unit> OnUnitPlaced;
    public Action<Unit> OnUnitRemoved;

    public FieldSlot.FormationRow formationRow;
    [HideInInspector] public FieldSlot fieldSlot;

    void Awake()
    {
        fieldSlot = GetComponent<FieldSlot>();
    }

    void Start()
    {
        formationRow = GetComponent<FieldSlot>().formationRow;
    }

    public void PlaceUnit(Unit unit)
    {
        curUnit = unit;
        OnUnitPlaced?.Invoke(unit);
        unit.unitDrag.curBatchFieldTrigger = this;
    }

    public void RemoveUnit()
    {
        OnUnitRemoved?.Invoke(curUnit);
        curUnit.unitDrag.curBatchFieldTrigger = null;
        curUnit = null;
    }

    public void RegisterUnitPlacedEvent(Action<Unit> callback)
    {
        OnUnitPlaced += callback;
        if (curUnit != null)
            callback.Invoke(curUnit);
    }

    public void RegisterUnitRemovedEvent(Action<Unit> callback)
    {
        OnUnitRemoved += callback;
    }

    public void RemoveUnitPlacedEvent(Action<Unit> placedCallback, Action<Unit> removedCallback)
    {
        OnUnitPlaced -= placedCallback;
        OnUnitRemoved -= removedCallback;
        if (curUnit != null)
            removedCallback.Invoke(curUnit);
    }


}
