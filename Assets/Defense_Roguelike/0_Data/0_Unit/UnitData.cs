using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;

public interface IUnitBaseStat
{
    public float BaseDamage { get; set; }
    public float BaseSkillValue { get; set; }
    public float BaseDefense { get; set; }
    public float BaseMaxHealth { get; set; }
    public float BaseMaxShield { get; set; }
    public float BaseCoolTime { get; set; }
    public float BaseRange { get; set; }
    public float BaseAttackSpeed { get; set; }

    public float BaseCriticalPer { get; set; }
    public float BaseCriticalDamage { get; set; }
    public float BaseHitChance { get; set; }
    public float BaseSkillTriggerChance { get; set; }
    public float BaseAreaDamage { get; set; }
    public float BaseSingleTargetDamage { get; set; }
    public float BaseEvasion { get; set; }
    public float BaseDefensePenetration { get; set; }
    public float BaseAttributeResistancePenetration { get; set; }
    public float BaseAttributeDamageMultiplier { get; set; }

    public float BaseHealingMultiplier { get; set; }
    public float BaseHealingReceivedMultiplier { get; set; }

    public float BaseShieldMultiplier { get; set; }
    public float BaseShieldReceivedMultiplier { get; set; }

    public float BaseNormalAttackMultiplier { get; set; }
    public float BaseSkillMultiplier { get; set; }
    public float BaseAbilityMultiplier { get; set; }

    public float BaseManaReceivedMultiplier { get; set; }

    public float BaseDamageTakenIncrease { get; set; }
}

public enum UnitClass { Common, Rare, Epic, Legendary, Apostle, Sephira }
public enum Type { Melee, Ranged, Tank, Support, Healer }
public enum Morality { Good, Evil }
public enum Faction
{
    None, DL_UniverseAcademy, Project_P, Qliphoth_Trinity, City_Of_Chesed, GPS_Shock_Troop, SepherIgnusLab, Four_Horsemen_Of_The_Apocalypse, Golden_Grace
, Arcade_Ranger, Void_Stalker, YeanSeFriend, OksanHighSchool, A_Strange_And_Wonderful_Country, Lumenesis_School, Throne_Room, Garden_of_Flesh, Void_Observer, Faust_Church, Slaughterhouse, Elemental_Busters, Trail_Render_Corporation, Aquatic_LimenCity
}
public enum UnitSupportType { None, Single, Mult }

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitData", order = 0)]
public class UnitData : ScriptableObject
{
    [Tab("Core Data")]
    [Header("# Name")]
    public string unitName;
    [Header("# Core")]
    public ScriptableObject unitSkill;
    public int unitID;
    [Space(10)]
    [Header("# UI Info")]
    public Sprite portrait;
    public float portraitScaleFactor = 1;
    public float portraitPosXFactor = 1;
    public float portraitPosYFactor = 1;
    [Space(10)]
    [Header("# Anim")]
    public RuntimeAnimatorController animController;
    [Space(10)]
    [TextArea]
    public string startDialogue;
    [TextArea]
    public string unitDesc;
    [Space(10)]
    public int cardLevel = 1;
    public int curLevelExp;
    public int maxLevelExp;

    public bool isUnlock;
    public bool isNew;
    public bool isEquipped;
    public bool isEquipLocked;

    [Space(20)]
    [Header("# In Game State")]
    [HideInInspector] public double damageAmount;
    [HideInInspector] public float skillAmount;
    [HideInInspector] public float defenseAmount;
    [HideInInspector] public float criticalPer;
    [HideInInspector] public float criticalDamage;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public float range;
    [HideInInspector] public float coolTime;
    public EffectManager.PrefabType effectPrefab;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float maxShield;
    [Tab("Skill")]
    [Header("# Skill Desc")]
    public SkillActivationInfo[] skillActivationInfo;

    [Tab("Type Data")]
    [Header("# Type")]
    public SephiraPathData sephiraPathData;
    public UnitClass unitClass;
    public Type type;
    public bool isHybrid;
    [Space]
    public AttributeData attribute;
    public UnitSupportType unitSupportType = UnitSupportType.None;
    public float supportDetectRadius = 1.4f;
    public Morality morality;
    public Faction faction = Faction.None;
    public ResourcesData upgradeResourcesData;
    public ResourcesData buyResourcesData;
    [Tab("Special Option")]
    [Header("# Special Option")]
    public RuntimeAnimatorController overrideAnimator;
    [Space]
    public bool isTrans = false;
    public bool isBaseForm = false;
    public bool isTransTrigger = false;
    [Space]
    public UnitData transData;
    public UnitData[] summonedUnits;
    [Tab("Base Data")]
    [Header("# Base Data")]
    public double baseDamage;
    public float baseSkillAmount;
    public float skillAmountIncrement = 0.01f;
    public float baseHealth;
    public float baseShield;
    public float baseDefence;
    public float baseCriticalPer;
    public float baseCriticalDamage;
    public float baseAttackSpeed;
    public float baseCoolTime;

    public enum RangeType { None, Melee, MidRange, LongRange, Custom }
    [Space]
    public RangeType rangeType;
    public float customRange;
    [Space]
    public float abllityApplyTime;
    [Space]
    [Space]
    public int baseCardLevel = 1;
    public int baseCurLevelExp;
    public int baseMaxLevelExp;
    [Space]
    public bool baseIsUnlock;
    public bool baseIsEquipped;

    public void InitUnitState()
    {
        damageAmount = Math.Round(baseDamage * Mathf.Pow(1.28f, cardLevel - 1));
        skillAmount = baseSkillAmount + skillAmountIncrement * (cardLevel - 1);

        if (unitClass == UnitClass.Sephira)
            maxHealth = baseHealth + 25 * (cardLevel - 1);
        else
            maxHealth = Mathf.Round(baseHealth * Mathf.Pow(1.15f, cardLevel - 1));
        maxShield = Mathf.Round(baseShield * Mathf.Pow(1.1f, cardLevel - 1));
        criticalPer = Mathf.Round(baseCriticalPer * Mathf.Pow(1.1f, cardLevel - 1));
        criticalDamage = Mathf.Round(baseCriticalDamage * Mathf.Pow(1.1f, cardLevel - 1));

        defenseAmount = baseDefence;
        range = GetRange();
        attackSpeed = baseAttackSpeed;
        coolTime = baseCoolTime;

        if (isTrans)
        {
            transData.isUnlock = isUnlock;
            transData.isEquipped = isEquipped;
            transData.cardLevel = cardLevel;
            transData.curLevelExp = curLevelExp;
            transData.maxLevelExp = maxLevelExp;

            transData.damageAmount = Math.Round(transData.baseDamage * Mathf.Pow(1.28f, cardLevel - 1));
            transData.skillAmount = transData.baseSkillAmount + transData.skillAmountIncrement * (cardLevel - 1);
            if (unitClass == UnitClass.Sephira)
                maxHealth = baseHealth + 50 * (cardLevel - 1);
            else
                transData.maxHealth = Mathf.Round(transData.baseHealth * Mathf.Pow(1.15f, cardLevel - 1));
            transData.maxShield = Mathf.Round(transData.baseShield * Mathf.Pow(1.15f, cardLevel - 1));
            transData.criticalPer = Mathf.Round(transData.baseCriticalPer * Mathf.Pow(1.1f, cardLevel - 1));
            transData.criticalDamage = Mathf.Round(transData.baseCriticalDamage * Mathf.Pow(1.1f, cardLevel - 1));
        }
    }

    public Dictionary<UnitClass, int[]> expTable = new Dictionary<UnitClass, int[]>
    {
        { UnitClass.Common, new int[] { 5, 10, 20, 50, 100, 200, 400, 800, 1600, 3200, 5000, 8000, 12000, 18000, 25000,0 } },
        { UnitClass.Rare, new int[] { 10, 20, 30, 50, 100, 200, 400, 700, 1200, 2000, 3500, 5000, 8000, 12000, 18000 ,0} },
        { UnitClass.Epic, new int[] { 10, 30, 50, 100, 150, 250, 400, 700, 1000, 1600, 2500, 4000, 6000, 9000, 14000 ,0} },
        { UnitClass.Legendary, new int[] { 5, 20, 50, 100, 200, 350, 500, 700, 1000, 1500, 2200, 3000, 4000, 5500, 7500,0 } },
        { UnitClass.Apostle, new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 100, 200, 300, 400, 500, 1000 } },
        { UnitClass.Sephira, new int[] { 20, 50, 100, 200, 500, 800, 1200, 1600, 2000, 2500, 3000, 3500, 4000, 4500, 5000 ,0 } }
    };

    public Dictionary<UnitClass, int[]> upgradeCostTable = new Dictionary<UnitClass, int[]>
    {
        { UnitClass.Common, new int[] { 50, 100, 250, 500, 1000, 2000, 5000, 8000, 10000, 15000, 20000, 25000, 50000, 80000, 100000,0 } },
        { UnitClass.Rare, new int[] { 50, 150, 300, 1000, 2500, 5000, 10000, 15000, 20000, 25000, 35000, 50000, 10000, 150000, 200000 ,0} },
        { UnitClass.Epic, new int[] { 100, 400, 1000, 2000, 3000, 5000, 15000, 20000, 30000, 50000, 75000, 100000, 150000, 200000, 250000 ,0} },
        { UnitClass.Legendary, new int[] { 5000, 10000, 15000, 20000, 25000, 30000, 45000, 50000, 100000, 150000, 200000, 250000, 300000, 350000, 400000,0 } },
        { UnitClass.Apostle, new int[] { 500, 1000, 3000, 5000, 6000, 8000, 9000, 10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000 ,0} },
        { UnitClass.Sephira, new int[] { 1000, 5000, 10000, 15000, 20000, 25000, 30000, 35000, 40000, 45000, 50000, 55000, 60000, 65000, 70000 ,0 } }
    };

    public Dictionary<UnitClass, Color> classColorType = new Dictionary<UnitClass, Color>
    {
        {UnitClass.Common, new Color(0.7f, 0.7f, 0.7f)},
        {UnitClass.Rare , new Color(0.5f, 0.7f, 1f)},
        {UnitClass.Epic, new Color(0.6f, 0.4f, 1f)},
        {UnitClass.Legendary ,new Color(1f, 0.8f, 0.4f)},
        {UnitClass.Apostle, new Color(0.3f, 0.3f, 0.4f)},
        {UnitClass.Sephira, new Color(0,1,0.7f,1)}
    };


    public string TypeToKorean(Type unitType)
    {
        if (isHybrid)
            return "하이브리드";

        return typeToKorean[unitType];
    }

    Dictionary<Type, string> typeToKorean = new Dictionary<Type, string>()
    {
        {Type.Melee, "워리어"},
        {Type.Ranged, "레인저"},
        {Type.Tank, "탱커"},
        {Type.Healer, "힐러"},
        {Type.Support, "서포터"}
    };

    public Dictionary<UnitClass, string> classToKorean = new Dictionary<UnitClass, string>()
    {
        {UnitClass.Common, "일반"},
        {UnitClass.Rare, "희귀"},
        {UnitClass.Epic, "영웅"},
        {UnitClass.Legendary, "전설"},
        {UnitClass.Apostle, "사도"},
        {UnitClass.Sephira, "세피라"}
    };

    public Dictionary<Morality, string> moralityToKorean = new Dictionary<Morality, string>()
    {
        {Morality.Good, "선"},
        {Morality.Evil, "악"}
    };


    public Dictionary<Faction, string> factionToKorean = new Dictionary<Faction, string>()
    {
        {Faction.None, "소속 없음"},
        {Faction.SepherIgnusLab, "세페르이그너스 랩 저장소"},
        {Faction.Qliphoth_Trinity, "클리포트 트리니티"},
        {Faction.Project_P, "프로젝트 P"},
        {Faction.GPS_Shock_Troop, "GPS 돌격대"},
        {Faction.DL_UniverseAcademy, "델타루나 유니버스 아카데미"},
        {Faction.City_Of_Chesed, "헤세드시"},
        {Faction.Four_Horsemen_Of_The_Apocalypse, "종말의 네 기수"},
        {Faction.Golden_Grace, "골든 그레이스"},
        {Faction.Arcade_Ranger, "아케이드 레인저"},
        {Faction.Void_Stalker, "공허 추적자"},
        {Faction.YeanSeFriend, "연새와 친구들"},
        {Faction.Aquatic_LimenCity, "아쿠아틱 레민시티"},
        {Faction.A_Strange_And_Wonderful_Country, "기묘하고 이상한 나라"},
        {Faction.Lumenesis_School, "루미네시스 학파"},
        {Faction.Throne_Room, "알현실"},
        {Faction.Garden_of_Flesh, "육신의 정원"},
        {Faction.Void_Observer, "공허 감시자"},
        {Faction.Faust_Church, "파우스트 교"},
        {Faction.Slaughterhouse, "도살장"},
        {Faction.Elemental_Busters, "엘리멘탈 버스터즈"},
        {Faction.Trail_Render_Corporation, "트레일 렌더 코퍼레이션"}
    };

    public Dictionary<UnitClass, (int itemCount, int itemPrice)> unitItemValues = new Dictionary<UnitClass, (int itemCount, int itemPrice)>
    {
        {UnitClass.Common, (100 , 300)},
        {UnitClass.Rare, (50, 500)},
        {UnitClass.Epic, (10, 1000)},
        {UnitClass.Legendary, (5, 40000)},
        {UnitClass.Apostle, (1, 500)},
        {UnitClass.Sephira, (50, 300)},
    };

    [System.Serializable]
    public class SkillActivationInfo
    {
        public SkillActivationType skillActivationType;
        public string skillName;
        [TextArea]
        public string skillDesc;
    }

    public float GetRange()
    {
        switch (rangeType)
        {
            case RangeType.None:
                return 0;
            case RangeType.Melee:
                return 1.7f;
            case RangeType.MidRange:
                return 2.8f;
            case RangeType.LongRange:
                return 6.2f;
            case RangeType.Custom:
                return customRange;
            default:
                return 1.8f;
        }
    }

    public string ConvertHighlightText(string rawText)
    {
        char HighlightMarker = '&';
        string HighlightColor = "#00FFDE";
        if (string.IsNullOrEmpty(rawText))
            return string.Empty;

        bool isHighlighting = false;
        System.Text.StringBuilder builder = new System.Text.StringBuilder(rawText.Length);

        foreach (char character in rawText)
        {
            if (character == HighlightMarker)
            {
                builder.Append(isHighlighting ? "</color>" : $"<color={HighlightColor}>");
                isHighlighting = !isHighlighting;
                continue;
            }

            builder.Append(character);
        }

        if (isHighlighting)
        {
            builder.Append("</color>");
        }

        return builder.ToString();
    }

}

