using System;
using UnityEngine;

public class InGameEventManager : MonoBehaviour
{
    public static InGameEventManager Inst;

    [HideInInspector] public JointOperationConditionChecker jointOperationConditionChecker;
    public Action<Unit> UnitMergeEvent;
    public Action<Unit> UnitSwappedEvent;
    public Action<Unit> UnitPlacedEvent;
    public Action<Unit> UnitRemovedEvent;
    public Action<Unit> UnitSkillEvent;
    public Action<Unit> UnitAttackEvent;
    public Action<SephiraUnit> SephiraAttackEvent;
    public Action<Enemy> OnHit;
    public Action GetCoin;
    public Action OnMainHUDToggled;
    public bool isDamageShared = false;
    public float myShareRatio;
    public int damageSharedCount = 0;

    public float unitMergeStarLevelUpgradeAmount;

    void Awake()
    {
        Inst = this;
        jointOperationConditionChecker = GetComponent<JointOperationConditionChecker>();
        UnitMergeEvent += OnMerge;
        UnitPlacedEvent += Placed;
    }

    public void ApplyDamageShared(float myRatio, bool active)
    {
        if (active)
        {
            damageSharedCount++;
            myShareRatio += myRatio;
            isDamageShared = true;
        }
        else
        {
            damageSharedCount--;
            myShareRatio = Mathf.Max(myShareRatio - myRatio, 0f);

            if (damageSharedCount <= 0)
            {
                damageSharedCount = 0;
                isDamageShared = false;
            }
        }
    }

    public void Initialized()
    {
        unitMergeStarLevelUpgradeAmount = 0;
    }

    void OnMerge(Unit unit)
    {

    }

    void Placed(Unit unit)
    {

    }
}
