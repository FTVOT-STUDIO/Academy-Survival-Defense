using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "CardData", menuName = "CardData", order = 1)]
public class CardData : ScriptableObject
{
    public enum CardClass { common, rare, epic, legendary, unique }
    public enum CardCategory { InstantUse, Upgrade }
    public enum CardVariety { StatUpgrade, Wave, EnemyKill, UnitDead, Ability, EnemyDebuff, Resource, UnitSpawn, Health, CoolTime, UnitSail, Defense, SkillCard, Risk, Composite, Field, LevelUp, NormalAttack, StarLevel, UnitMerge }

    [Header("# Main Info")]
    [TextArea]
    public string cardSummary;
    public ScriptableObject cardEffect;
    public CardClass cardClass;
    public CardCategory cardCategory;
    public CardVariety cardVariety;
    [Space]
    public SephiraPathData sephiraPathData;
    [Space]
    public string card_Name;
    [TextArea]
    public string Desc;

    [HideInInspector] public int level = 0;
    public int maxLevel = 1;
    public float Amount;

    public Dictionary<CardClass, string> CardClassToKorean = new Dictionary<CardClass, string>
    {
        { CardClass.common, "일반" },
        { CardClass.rare, "희귀" },
        { CardClass.epic, "에픽" },
        { CardClass.legendary, "전설" },
        { CardClass.unique, "유니크" }
    };

    public Dictionary<CardCategory, string> CardCategoryToKorean = new Dictionary<CardCategory, string>
    {
        { CardCategory.InstantUse, "즉시 사용" },
        { CardCategory.Upgrade, "강화형" }
    };

    public Dictionary<CardClass, Color> CardClassToColor = new Dictionary<CardClass, Color>
    {
        {CardClass.common, new Color(0.6f,0.6f,0.8f)},
        {CardClass.rare, new Color(0.4f, 0.6f, 1)},
        {CardClass.epic, new Color(0.5f, 0.25f, 1)},
        {CardClass.legendary, new Color(1,0.8f,0.1f)},
        {CardClass.unique, new Color(1,0.1f,0.35f)}
    };

    public void ApplyCardEffect()
    {
        if (cardEffect is ICardEffect effect)
        {
            effect.ApplyCardEffect(this, Amount);
        }
    }

    public float CalculateBaseValue(float amount)
    {
        return amount * 100;
    }

    public float CalculateLevelValue(float amount)
    {
        int level = GameManager.Inst.skillCardManager.GetCardLevel(this);
        return amount * 100 * level;
    }

    public Dictionary<CardVariety, string> varietyTable = new Dictionary<CardVariety, string>()
    {
        {CardVariety.Ability, "세피라 궁극기형"},
        {CardVariety.CoolTime, "쿨타임형"},
        {CardVariety.Defense, "방어형"},
        {CardVariety.EnemyDebuff, "적 디버프형"},
        {CardVariety.EnemyKill, "적 처치형"},
        {CardVariety.Field, "필드형"},
        {CardVariety.Health, "체력형"},
        {CardVariety.LevelUp, "레벨업형"},
        {CardVariety.NormalAttack, "평타형"},
        {CardVariety.Resource, "자원형"},
        {CardVariety.Risk, "리스크형"},
        {CardVariety.SkillCard, "스킬카드형"},
        {CardVariety.Composite, "복합형"},
        {CardVariety.StarLevel, "스타 레벨형"},
        {CardVariety.StatUpgrade, "스탯 업그레이드형"},
        {CardVariety.UnitDead, "유닛 분해형"},
        {CardVariety.UnitMerge, "유닛 합성형"},
        {CardVariety.UnitSail, "유닛 판매형"},
        {CardVariety.UnitSpawn, "유닛 스폰형"},
        {CardVariety.Wave, "웨이브형"},
    };
}
