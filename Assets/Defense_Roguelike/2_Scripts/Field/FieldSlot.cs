using UnityEngine;
using VInspector;

public class FieldSlot : MonoBehaviour
{
    public enum FormationRow
    {
        Front,
        Middle,
        Back
    }

    [Tab("Main")]
    [Space]
    public int sortingOrder;
    public FormationRow formationRow;
    public int x;
    public int y;

    [Tab("Marker")]
    [Space]
    public GameObject placementMarker;
    public GameObject levelUpMarker;
    public GameObject supportMarker;

    public void InitMarker()
    {
        FieldManager.Inst.placementMarker.Add(placementMarker);
        FieldManager.Inst.levelUpMarker.Add(levelUpMarker);
        FieldManager.Inst.supportMarker.Add(supportMarker);
    }
}
