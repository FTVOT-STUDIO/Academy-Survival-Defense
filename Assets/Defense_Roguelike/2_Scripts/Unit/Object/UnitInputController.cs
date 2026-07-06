using UnityEngine;
using UnityEngine.EventSystems;

public class UnitInputController : MonoBehaviour
{
    public GameObject selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] bool isDraggingAllowed = false;
    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject();
            //return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        else
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return EventSystem.current.IsPointerOverGameObject();
#else
        return false;
#endif
        }
    }


    bool IsUIBlockNeeded()
    {
        if (selectedUnit == null) return true;

        UnitDrag unitDrag = selectedUnit.GetComponent<UnitDrag>();
        if (unitDrag == null) return true;

        return !unitDrag.isDragged;
    }



    void Update()
    {
        Vector3 inputPosition = Input.mousePosition;

        if (selectedUnit != null)
        {
            if (selectedUnit.layer == LayerMask.NameToLayer("Unit"))
            {
                Unit unit = selectedUnit.GetComponent<Unit>();

                if (!unit.gameObject.activeInHierarchy || unit.unitEvent.IsSilenced || unit.unitDrag.isInputLocked)
                {
                    ForceReleaseSelectedUnit();
                    unit.unitDrag.ResetLoaction();
                    if (InGameUIManager.Inst.unitInfoUI.infoWindow.activeInHierarchy)
                    {
                        InGameUIManager.Inst.unitInfoUI.OnHideInfoUI();
                    }

                    selectedUnit = null;
                    return;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.Inst.isGameStart || (selectedUnit != null && !selectedUnit.activeInHierarchy))
            {
                selectedUnit = null;
                isDraggingAllowed = false;
                return;
            }

            if (IsPointerOverUI())
            {
                selectedUnit = null;
                isDraggingAllowed = false;
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(inputPosition);
            Vector2 origin = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 0f, unitLayerMask);
            if (hit.collider != null)
            {
                selectedUnit = hit.collider.gameObject.transform.parent.gameObject;

                if (selectedUnit.layer == LayerMask.NameToLayer("Unit"))
                {
                    isDraggingAllowed = true;
                    selectedUnit.GetComponent<UnitDrag>().MouseDown();
                }
                else if (selectedUnit.layer == LayerMask.NameToLayer("Sephira"))
                {
                    isDraggingAllowed = false;
                    selectedUnit.GetComponent<SephiraUnit>().MouseDown();
                }
            }
            else
            {

                if (InGameUIManager.Inst.unitInfoUI.infoWindow.activeInHierarchy)
                    InGameUIManager.Inst.unitInfoUI.OnHideInfoUI();

                selectedUnit = null;
                isDraggingAllowed = false;
            }
        }

        if (Input.GetMouseButton(0) && selectedUnit != null && isDraggingAllowed)
        {
            if (!GameManager.Inst.isGameStart || (selectedUnit != null && !selectedUnit.activeInHierarchy))
            {
                selectedUnit = null;
                isDraggingAllowed = false;
                return;
            }

            selectedUnit.GetComponent<UnitDrag>().MouseDrag();
        }

        if (Input.GetMouseButtonUp(0) && selectedUnit != null)
        {
            ForceReleaseSelectedUnit();
        }
    }

    public void ForceReleaseSelectedUnit()
    {
        if (!GameManager.Inst.isGameStart || (selectedUnit != null && !selectedUnit.activeInHierarchy))
        {
            selectedUnit = null;
            isDraggingAllowed = false;
            return;
        }


        if (isDraggingAllowed && selectedUnit.layer == LayerMask.NameToLayer("Unit"))
        {
            selectedUnit.GetComponent<UnitDrag>().MouseUp();
        }
        else if (selectedUnit.layer == LayerMask.NameToLayer("Sephira"))
        {
            selectedUnit.GetComponent<SephiraUnit>().MouseUp();
        }

        isDraggingAllowed = false;
        if (!InGameUIManager.Inst.unitInfoUI.infoWindow.activeInHierarchy)
        {
            selectedUnit = null;
        }
    }
}
