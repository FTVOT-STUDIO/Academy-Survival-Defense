using UnityEngine;
using DG.Tweening;
using VInspector;
using Unity.VisualScripting;
using UnityEngine.EventSystems;


public class UnitDrag : MonoBehaviour
{
    Vector3 offset;
    Camera mainCamera;

    Unit unit;
    [Tab("Index")]
    [Header("# Current Index")]
    public int curIndex;
    public Unit curUnit;
    [Tab("Flag")]
    [Header("# Flag")]
    public bool isDragLocked;
    [Space]
    public bool isPressed;
    public bool isClicked = false;
    public bool isDragged;
    public bool isInputLocked = false;
    [Space]
    public bool isPlaced;
    public bool isResetLocate;

    Vector3 initialMousePosition;
    Vector3 initialObjectPosition;

    public FieldPlacementTrigger curBatchFieldTrigger;


    [HideInInspector] public Vector3 curPos;
    public GameObject exiUnit;
    [HideInInspector] public GameObject mergeUnit;

    void Start()
    {
        mainCamera = Camera.main;

        unit = GetComponent<Unit>();
    }

    void LateUpdate()
    {
        if (!isDragged)
        {
            unit.sp.transform.localPosition = Vector3.zero;
        }
    }

    public void MouseDown()
    {
        if (GameManager.Inst.isPause || !GameManager.Inst.isGameStart || InGameUIManager.Inst.gameStatePanel.activeSelf || unit.unitEvent.IsIgnored || isInputLocked || unit.unitEvent.IsSilenced)
            return;

        if (Time.timeScale == 0)
        {
            ResetLoaction();
            return;
        }

        if (isPressed || isResetLocate)
            return;

        isClicked = !isClicked;

        if (FieldManager.Inst.selectUnit != null)
            FieldManager.Inst.lastUnit = FieldManager.Inst.selectUnit;

        FieldManager.Inst.selectUnit = gameObject;

        unit.unitInfo.OnToggleChangeTarget();

        isDragLocked = false;
        isPressed = true;
        isPlaced = false;
        isDragged = false;

        initialMousePosition = GetMouseWorldPosition();
        initialObjectPosition = transform.position;


        curPos = transform.position;
        offset = transform.position - GetMouseWorldPosition();

        FieldManager.Inst.ResetFieldState();

        for (int j = 0; j < FieldManager.Inst.fieldUnits.Length; j++)
        {
            GameObject placeMarker = FieldManager.Inst.placementMarker[j];
            GameObject levelUpMarker = FieldManager.Inst.levelUpMarker[j];
            GameObject supportMarker = FieldManager.Inst.supportMarker[j];

            placeMarker.SetActive(false);
            levelUpMarker.SetActive(false);
            supportMarker.SetActive(false);

            GameObject fieldObj = FieldManager.Inst.fieldUnits[j];

            if (fieldObj == null)
            {
                placeMarker.SetActive(true);
                continue;
            }

            Unit fieldUnit = fieldObj.GetComponent<Unit>();

            if (unit.gameObject == fieldUnit.gameObject || fieldUnit.unitEvent.IsIgnored || isInputLocked)
                continue;

            if (unit.starLevel == fieldUnit.starLevel)
            {
                if (unit.starLevel != unit.unitLevel.maxLevel && fieldUnit.starLevel != unit.unitLevel.maxLevel && unit.unitData == fieldUnit.unitData)
                {
                    levelUpMarker.SetActive(true);
                }
                else if (unit.unitData.type == Type.Support && unit.unitData.unitSupportType == UnitSupportType.Single && unit.unitData.type != fieldUnit.unitData.type)
                {
                    supportMarker.SetActive(true);
                }
                else if (unit.unitSkill is ISupportDetectable)
                {
                    supportMarker.SetActive(true);
                }
                else
                {
                    placeMarker.SetActive(true);
                }
            }
            else
            {
                placeMarker.SetActive(true);
            }
        }
        unit.sp.sortingOrder = 120;
        unit.unitInfo.SetAttackRange(true);

        unit.unitInfo.supportTargetLine.SetActive(true);

        unit.anim.SetTrigger("Attack");

        UnitMovePopAnimation(unit.visualGroup.transform);

        curIndex = GetUnitIndex(unit.gameObject);
    }

    public void MouseDrag()
    {
        if (unit.unitEvent.IsUnitLocked || unit.unitEvent.IsSilenced)
        {
            ResetLoaction();
            GameManager.Inst.unitInputController.ForceReleaseSelectedUnit();
            GameManager.Inst.unitInputController.selectedUnit = null;
            return;
        }
        if (GameManager.Inst.isPause || !GameManager.Inst.isGameStart || unit.unitEvent.IsUnitLocked || unit.unitEvent.IsIgnored || unit.unitEvent.IsSilenced || InGameUIManager.Inst.gameStatePanel.activeSelf || unit.unitEvent.IsIgnored || isInputLocked)
            return;

        if (Time.timeScale == 0)
        {
            ResetLoaction();
            return;
        }

        if (isDragLocked || isResetLocate)
            return;

        if (!isDragged && Vector3.Distance(GetMouseWorldPosition(), initialMousePosition) > 0.2f)
        {
            isDragged = true;
            isClicked = true;
            unit.sp.sortingOrder = 120;
            unit.unitInfo.ToggleUnitUI(true, gameObject);
        }

        Vector3 mousePos = GetMouseWorldPosition() + offset;

        unit.sp.transform.position = mousePos;

        Vector3 direction = mousePos - curPos;

        if (direction.x != 0)
        {
            unit.sp.flipX = direction.x < 0;
        }

        if (isDragged)
        {
            unit.unitInfo.infoWindow.SetActive(false);
            unit.unitInfo.saleBtn.SetActive(false);

            if (unit.unitSkill is IButtonAttckPassive)
            {
                unit.unitInfo.skillBtn.SetActive(false);
            }

            if (unit.unitSkill is IButtonAttckSkill)
            {
                unit.unitInfo.skillBtn.SetActive(false);
            }

            if (unit.unitSkill is IButtonPassive)
            {
                unit.unitInfo.skillBtn.SetActive(false);
            }

        }
    }

    public void MouseUp()
    {
        if (GameManager.Inst.isPause || !GameManager.Inst.isGameStart || InGameUIManager.Inst.gameStatePanel.activeSelf || unit.unitEvent.IsIgnored || isInputLocked)
            return;

        if (Time.timeScale == 0)
        {
            ResetLoaction();
            return;
        }

        unit.unitInfo.ToggleUnitUI(isClicked, gameObject);

        if (isResetLocate)
            return;

        for (int i = 0; i < FieldManager.Inst.fieldSlots.Length; i++)
        {
            Transform batchSlots = FieldManager.Inst.fieldSlots[i];

            GameObject batchUnit = FieldManager.Inst.fieldUnits[i];

            if (Vector2.Distance(unit.sp.transform.position, batchSlots.transform.position) < 0.5f)
            {
                if (batchUnit != null && batchUnit != gameObject)
                {
                    Unit unit = batchUnit.GetComponent<Unit>();

                    if (unit.unitEvent.IsUnitLocked || unit.unitEvent.IsSilenced)
                    {
                        ResetLoaction();
                        break;
                    }

                    bool isSameUnit = unit.unitData.unitName == this.unit.unitData.unitName;
                    bool isSameLevel = unit.starLevel == this.unit.starLevel;
                    bool isBothNotMaxLevel = this.unit.starLevel != unit.unitLevel.maxLevel && unit.starLevel != unit.unitLevel.maxLevel;

                    if (isSameUnit && isSameLevel && isBothNotMaxLevel)
                    {
                        // 유닛 병합 로직
                        Merge(unit, this.unit);
                        MergeEffect(this.unit);
                    }
                    else
                    {
                        // 병합 불가 → 위치 교체
                        exiUnit = batchUnit;
                        if (this.unit.unitData.type == Type.Support && isSameLevel && this.unit.unitData.unitSupportType == UnitSupportType.Single)
                        {
                            var targetUnit = exiUnit.GetComponent<Unit>();
                            bool canBuffSupport = targetUnit.unitData.type != Type.Support || this.unit.unitSkill is ISupportDetectable;

                            if (canBuffSupport)
                            {
                                this.unit.unitScanner.nearestUnit = targetUnit.transform;

                                if (this.unit.isBuffed || targetUnit.isBuffed)
                                {
                                    DragClearBuff();
                                }
                                else
                                {
                                    this.unit.unitWeapon.UnitAttack();
                                }
                                break;
                            }
                        }

                        curBatchFieldTrigger.RemoveUnit();
                        exiUnit.GetComponent<Unit>().unitDrag.curBatchFieldTrigger.RemoveUnit();

                        FieldManager.Inst.UnitBatch(exiUnit, curIndex, true);
                        FieldManager.Inst.UnitBatch(gameObject, i, true);

                        UnitMovePopAnimation(unit.visualGroup.transform);
                        UnitMovePopAnimation(exiUnit.GetComponent<Unit>().visualGroup.transform);
                        InGameEventManager.Inst.UnitSwappedEvent?.Invoke(this.unit);

                    }
                }
                else if (isDragged)
                {
                    if (curIndex == -1)
                    {
                        curBatchFieldTrigger.RemoveUnit();
                        FieldManager.Inst.fieldUnits[GetUnitIndex(unit.gameObject)] = null;
                        FieldManager.Inst.UnitBatch(gameObject, i, true);
                        InGameEventManager.Inst.UnitPlacedEvent?.Invoke(unit);
                    }
                    else if (batchSlots != FieldManager.Inst.fieldSlots[curIndex])
                    {
                        curBatchFieldTrigger.RemoveUnit();
                        FieldManager.Inst.fieldUnits[curIndex] = null;
                        FieldManager.Inst.UnitBatch(gameObject, i, true);
                        InGameEventManager.Inst.UnitPlacedEvent?.Invoke(unit);
                    }
                }
                if (!isClicked)
                {
                    unit.sp.sortingOrder = FieldManager.Inst.fieldSlots[i].GetComponent<FieldSlot>().sortingOrder;
                }

                if (isDragged)
                    UnitMovePopAnimation(unit.visualGroup.transform);

                if (isPlaced)
                {
                    FieldManager.Inst.UnitBatch(gameObject, i, true);
                }

                curPos = transform.position;
                unit.sp.transform.localPosition = Vector3.zero;
                isPlaced = true;
                break;

            }
        }

        if (!isPlaced)
        {
            unit.sp.transform.DOMove(curPos, 0.2f).
             SetEase(Ease.OutBack).
             OnComplete(() =>
             {
                 FieldManager.Inst.ResetFieldState();

                 isDragged = false;
                 isPressed = false;
                 isDragLocked = false;
                 isPlaced = true;
                 unit.buffIcon.GetComponent<SpriteRenderer>().sortingOrder = unit.sp.sortingOrder + 1;

             });
        }
        else
        {
            FieldManager.Inst.ResetFieldState();

            isDragged = false;
            isPressed = false;
            isDragLocked = false;
            isPlaced = true;
            unit.buffIcon.GetComponent<SpriteRenderer>().sortingOrder = unit.sp.sortingOrder + 1;
        }
        curIndex = GetUnitIndex(unit.gameObject);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void ResetLoaction()
    {
        if (isDragged)
        {
            isResetLocate = true;
            isDragLocked = true;
            isDragged = false;
            isPressed = false;
            isPlaced = true;
            isClicked = false;
            unit.unitInfo.ToggleUnitUI(false, gameObject);

            unit.sp.transform.DOMove(curPos, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                //gameObject.layer = 6;
                isResetLocate = false;
            });
            curPos = transform.position;
            unit.buffIcon.GetComponent<SpriteRenderer>().sortingOrder = unit.sp.sortingOrder + 1;
            FieldManager.Inst.ResetFieldState();
        }

    }

    void DragClearBuff()
    {
        if (exiUnit == this.unit.unitScanner.buffUnit)
        {
            unit.unitEvent.ClearBuff(this.unit, this.unit.unitScanner.buffUnit.GetComponent<Unit>());
            //print("버프 제거");
            return;
        }
        else if (exiUnit != this.unit.unitScanner.buffUnit && unit.starLevel == exiUnit.GetComponent<Unit>().starLevel)
        {
            if (this.unit.isBuffed && exiUnit.GetComponent<Unit>().isBuffed)
            {
                GameObject thisBuffUnit = this.unit.unitScanner.buffUnit; // my target unit
                GameObject thisUnit = this.unit.gameObject;

                GameObject exiBuffUnit = this.exiUnit.GetComponent<Unit>().unitScanner.buffUnit;
                GameObject exiUnit = this.exiUnit;

                unit.unitEvent.ClearBuff(thisUnit.GetComponent<Unit>(), thisBuffUnit.GetComponent<Unit>());
                unit.unitEvent.ClearBuff(exiBuffUnit.GetComponent<Unit>(), exiUnit.GetComponent<Unit>());

                thisUnit.GetComponent<Unit>().unitDrag.exiUnit = exiUnit;
                exiBuffUnit.GetComponent<Unit>().unitDrag.exiUnit = thisBuffUnit;

                thisUnit.GetComponent<Unit>().unitScanner.nearestUnit = exiUnit.transform;
                exiBuffUnit.GetComponent<Unit>().unitScanner.nearestUnit = thisBuffUnit.transform;

                exiBuffUnit.GetComponent<Unit>().unitWeapon.UnitAttack();
                this.unit.unitWeapon.UnitAttack();
                return;
                //  print("서로 버프 교체");
            }
            else if (!this.unit.isBuffed && exiUnit.GetComponent<Unit>().isBuffed)
            {
                unit.unitEvent.ClearBuff(exiUnit.GetComponent<Unit>().unitScanner.buffUnit.GetComponent<Unit>(), exiUnit.GetComponent<Unit>());
                //print("버프 교체");
                this.unit.unitWeapon.UnitAttack();
                return;
            }
            else if (this.unit.isBuffed && exiUnit != this.unit.unitScanner.buffUnit)
            {
                GameObject changeTaget = exiUnit;
                unit.unitEvent.ClearBuff(this.unit, this.unit.unitScanner.buffUnit.GetComponent<Unit>());
                exiUnit = changeTaget;
                unit.unitScanner.nearestUnit = changeTaget.transform;
                // print("다른 유닛에게 버프 교체");
                this.unit.unitWeapon.UnitAttack();
                return;
            }
        }
    }

    void UnitMovePopAnimation(Transform transform)
    {
        transform.DOScale(Vector3.one, 0.3f).
                       SetEase(Ease.OutBack).
                      SetUpdate(true).
                      From(new Vector3(1.4f, 1.4f, 1.4f));
    }

    public bool Merge(Unit sacrificedUnit, Unit mainUnit, bool callOnBeforeTrans = false)
    {
        bool isRandomized = DebuffManager.Inst.isMergeRandomizedDebuff || sacrificedUnit.unitEvent.IsMergeRandomized;

        if (mainUnit.unitSkill is IOnDisablePassive onDisablePassive)
        {
            onDisablePassive.ApplyOnDisablePassive(mainUnit);
        }
        if (mainUnit.unitSkill is IOnDisableSkill onDisableSkill)
        {
            onDisableSkill.ApplyOnDisableSkill(mainUnit);
        }

        mergeUnit = sacrificedUnit.gameObject;
        if (isRandomized)
        {
            mainUnit.unitData = DeckManager.Inst.curGameDeckUnitDatas[
                Random.Range(0, DeckManager.Inst.curGameDeckUnitDatas.Count)
            ];
        }

        mainUnit.Init(mainUnit.starLevel, false, isRandomized);
        mainUnit.unitEvent.ApplyLevelUpUnit(1, sacrificedUnit.transform);
        curBatchFieldTrigger.RemoveUnit();
        sacrificedUnit.UnitDeActive();
        FieldManager.Inst.RemoveBatchUnit(mainUnit.gameObject);
        InGameUIManager.Inst.unitInfoUI.OnHideInfoUI();
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.UnitMerge, 1);

        InGameEventManager.Inst.UnitMergeEvent?.Invoke(mainUnit);

        return true;
    }

    public int GetUnitIndex(GameObject unit)
    {
        for (int i = 0; i < FieldManager.Inst.fieldUnits.Length; i++)
        {
            if (FieldManager.Inst.fieldUnits[i] == unit)
                return i;
        }
        return -1;
    }

    public void MergeEffect(Unit target)
    {
        target.SetUnitClassColor();
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Merge);
        UnitMovePopAnimation(target.transform);
    }
}
