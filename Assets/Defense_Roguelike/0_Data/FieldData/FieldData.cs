using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FieldData", menuName = "FieldData")]
public class FieldData : ScriptableObject
{
    public ScriptableObject fieldSkill;
    public string fieldName;
    [TextArea]
    public string desc;
    public float value;
    public int unlockCost;
    public Sprite fieldPortrait;
    public Sprite fieldBackGround;
    public Sprite fieldSlot;
    public int fieldID;
    public int fieldLevel;

    public bool isUnlock;
    public bool isEquipped;

    public float baseValue;
    public int baseLevel;
    public bool baseIsUnlock;
    public bool baseIsEquipped;

    public int[] upgradeCostTable = { 500, 1000, 2000, 5000, 10000, 15000, 20000, 25000, 50000, 100000 };
    [Space]
    public ResourcesData unlockResourceTypeInfo;
    public ResourcesData upgradeResourceTypeInfo;

    public void InitFieldState()
    {
        value = MathF.Round(baseValue * Mathf.Pow(1.5f, fieldLevel - 1), 2);
    }
}
