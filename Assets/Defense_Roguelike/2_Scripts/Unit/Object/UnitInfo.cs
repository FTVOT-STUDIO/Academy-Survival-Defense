using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    Unit unit;
    SephiraUnit sephiraUnit;

    public enum UnitType { Unit, SephiraUnit }
    public UnitType unitType;

    [HideInInspector] public GameObject supportTargetLine;
    public GameObject attackRangeSprite;

    [HideInInspector] public GameObject infoWindow;
    [HideInInspector] public GameObject saleBtn;
    [HideInInspector] public GameObject exitBtn;
    [HideInInspector] public GameObject skillBtn;
    void Awake()
    {
        switch (unitType)
        {
            case UnitType.Unit:
                unit = GetComponentInParent<Unit>();
                break;
            case UnitType.SephiraUnit:
                sephiraUnit = GetComponentInParent<SephiraUnit>();
                break;
        }
    }

    void Start()
    {
        infoWindow = InGameUIManager.Inst.unitInfoUI.infoWindow;
        saleBtn = InGameUIManager.Inst.unitInfoUI.saleBtn.gameObject;
        exitBtn = InGameUIManager.Inst.unitInfoUI.exitBtn.gameObject;
        skillBtn = InGameUIManager.Inst.unitInfoUI.skillBtn.gameObject;
    }

    void LateUpdate()
    {
        if (unitType == UnitType.Unit && unit.unitEvent.IsIgnored)
            return;

        UpdateAttackRangeSprite();
        DrawSupportLine();
    }


    void ShowInfoWindow()
    {
        InGameUIManager.Inst.unitInfoUI.InitInfo(unit, sephiraUnit, unitType);
    }

    void UpdateAttackRangeSprite()
    {
        float range = 0;

        if (unitType == UnitType.Unit && unit != null)
        {
            range = unit.range;
            attackRangeSprite.transform.position = unit.sp.transform.position;
        }
        else if (sephiraUnit != null)
        {
            range = sephiraUnit.range;
        }

        attackRangeSprite.transform.localScale = new Vector3(range / 5f, range / 5f, 1);
    }

    public void SetAttackRange(bool active)
    {
        if (!GameManager.Inst.isGameStart)
            return;

        attackRangeSprite.SetActive(active);
    }

    void DrawSupportLine()
    {
        if (unitType == UnitType.Unit)
        {
            if (unit.unitScanner.buffUnit != null)
            {
                supportTargetLine.GetComponent<LineRenderer>().SetPosition(0, unit.sp.transform.position);
                supportTargetLine.GetComponent<LineRenderer>().SetPosition(1, unit.unitScanner.buffUnit.GetComponent<Unit>().sp.transform.position);
            }
            else
            {
                supportTargetLine.GetComponent<LineRenderer>().SetPosition(0, unit.sp.transform.position);
                supportTargetLine.GetComponent<LineRenderer>().SetPosition(1, unit.sp.transform.position);
            }
        }


    }
    void SetSaleButton(bool active)
    {
        if (!GameManager.Inst.isGameStart)
            return;
        saleBtn.SetActive(active);
        saleBtn.GetComponent<SaleButton>().Init(unit.unitLevel.GetSaleRewardByLevel().ToString(), unit.unitLevel.GetSaleResourceByLevel());
        Vector3 offset = new Vector3(0, 1.5f, 0);
        saleBtn.GetComponent<Follower>().InitRenderType(Follower.RenderTyep.Camera, FieldManager.Inst.selectUnit.transform, offset);
    }

    void SetSkillButton(bool active)
    {
        if (!GameManager.Inst.isGameStart)
            return;
        skillBtn.SetActive(active);
        Vector3 offset = new Vector3(0, -0.5f, 0);
        skillBtn.GetComponent<Follower>().InitRenderType(Follower.RenderTyep.Camera, FieldManager.Inst.selectUnit.transform, offset);
    }

    public void ToggleUnitUI(bool active, GameObject gameObject)
    {
        if (!GameManager.Inst.isGameStart || gameObject != FieldManager.Inst.selectUnit)
            return;

        InGameUIManager.Inst.unitInfoUI.currentUnit = FieldManager.Inst.selectUnit;
        InGameUIManager.Inst.unitInfoUI.lastUnit = FieldManager.Inst.lastUnit;

        InGameUIManager.Inst.unitInfoUI.currentUnit = gameObject;
        ShowInfoWindow();

        float radius = unitType == UnitType.Unit ? unit.range : sephiraUnit.range;

        SetAttackRange(active);

        if (infoWindow != null)
            infoWindow.SetActive(active);

        if (unitType != UnitType.SephiraUnit)
        {
            supportTargetLine.SetActive(active);

            if (saleBtn != null)
                SetSaleButton(active);

            if (unit.unitSkill is IButtonAttckPassive || unit.unitSkill is IButtonAttckSkill || unit.unitSkill is IButtonPassive)
            {
                SetSkillButton(active);

                if (unit.unitSkill is IButtonInfoinitialize buttonInfoinitialize)
                {
                    buttonInfoinitialize.InitializeSkillButtonUI(unit);
                }
            }

        }
    }

    public void OnToggleChangeTarget()
    {
        if (FieldManager.Inst.selectUnit != FieldManager.Inst.lastUnit && FieldManager.Inst.lastUnit != null)
        {
            switch (FieldManager.Inst.lastUnit.GetComponentInChildren<UnitInfo>().unitType)
            {
                case UnitType.Unit:
                    FieldManager.Inst.lastUnit.GetComponent<Unit>().unitDrag.isClicked = false;
                    break;
                case UnitType.SephiraUnit:
                    FieldManager.Inst.lastUnit.GetComponent<SephiraUnit>().isClicked = false;
                    break;
            }

            FieldManager.Inst.lastUnit.GetComponentInChildren<UnitInfo>().ToggleUnitUI(false, gameObject.transform.parent.gameObject);
        }
    }

    public void InitializeSkillButton(string mainTooltip, bool useGroup = false, string subToolTip = null, bool useImage = false, Sprite iconSprite = null)
    {
        skillBtn.GetComponent<SkillButton>().Init(mainTooltip, useGroup, subToolTip, useImage, iconSprite);
    }
}

