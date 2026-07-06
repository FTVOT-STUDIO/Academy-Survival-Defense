using UnityEngine;

public class UnitLevel : MonoBehaviour
{
    [Header("# Spacing")]
    public float spacing = 0.15f;
    [SerializeField] private float twoRowHeightOffset = 0.25f;

    int level = 1;
    [Header("# Level Group")]
    [SerializeField] GameObject[] levelSprites;
    [SerializeField] GameObject maxLevelSprite;

    [SerializeField] Transform starLevelSpriteGroup;
    public int maxLevel { get; } = 7;
    Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    void OnDisable()
    {
        foreach (GameObject sprite in levelSprites)
        {
            sprite.transform.localPosition = Vector3.zero;
        }
    }

    void UpdateUnitLevelSprite(int newLevel)
    {
        level = newLevel;

        if (levelSprites != null && levelSprites.Length > 0)
        {
            for (int i = 0; i < levelSprites.Length; i++)
            {
                levelSprites[i].SetActive(i < level);
            }
        }

        AlignSprites();
    }

    void AlignSprites()
    {
        if (level >= maxLevel)
        {
            SetAllStarsActive(false);
            maxLevelSprite.SetActive(true);

            return;
        }

        maxLevelSprite.SetActive(false);

        int activeCount = Mathf.Min(level, levelSprites.Length);

        SetAllStarsActive(false);

        if (level <= 4)
        {
            AlignRow(0, activeCount, 0f);
        }
        else
        {
            int topRowCount;
            int bottomRowCount;

            if (level == 5)
            {
                topRowCount = 2;
                bottomRowCount = 3;
            }
            else
            {
                topRowCount = 3;
                bottomRowCount = 3;
            }

            float offset = twoRowHeightOffset;

            AlignRow(0, topRowCount, offset);
            AlignRow(topRowCount, bottomRowCount, 0);
        }
    }

    private void AlignRow(int startIndex, int count, float yOffset)
    {
        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < count; i++)
        {
            int spriteIndex = startIndex + i;

            GameObject sprite = levelSprites[spriteIndex];
            sprite.SetActive(true);

            Vector3 pos = new Vector3(startX + i * spacing, yOffset, 0f);
            sprite.transform.localPosition = pos;
        }
    }


    private void SetAllStarsActive(bool active)
    {
        foreach (var sprite in levelSprites)
        {
            if (sprite != null)
                sprite.SetActive(active);
        }
    }

    public void AddLevel(int amount)
    {
        if (unit.starLevel >= maxLevel)
        {
            UpdateLevelState();
            return;
        }

        unit.starLevel = Mathf.Min(unit.starLevel + amount, maxLevel);

        unit.unitWeapon.skillTime = 0;
        unit.anim.SetTrigger("Attack");
        UpdateUnitLevelSprite(unit.starLevel);
        UpdateLevelState();
        unit.ResetUnitBuff();
        unit.ResetUnitState();
        FieldManager.Inst.UnitBatch(unit.gameObject, unit.unitDrag.curIndex, false);
        unit.unitDrag.ResetLoaction();
    }
    public void RemoveLevel(int amount)
    {
        unit.starLevel -= amount;

        if (unit.starLevel <= 0)
        {
            unit.UnitDeActive();
            return;
        }

        unit.unitWeapon.skillTime = 0;
        unit.anim.SetTrigger("Attack");
        UpdateUnitLevelSprite(unit.starLevel);
        UpdateLevelState();

        unit.ResetUnitBuff();
        unit.ResetUnitState();
        FieldManager.Inst.UnitBatch(unit.gameObject, unit.unitDrag.curIndex, false);
        unit.unitDrag.ResetLoaction();

        EffectManager.Inst.Get(EffectManager.PrefabType.StarLevelDownEffect, unit.transform.position);
    }
    public void SetLevel(int level, bool shouldExecuteBatch)
    {
        if (unit.starLevel >= maxLevel)
        {
            UpdateLevelState();
            return;
        }

        unit.starLevel = Mathf.Min(level, maxLevel);

        unit.unitWeapon.skillTime = 0;
        unit.anim.SetTrigger("Attack");
        UpdateUnitLevelSprite(unit.starLevel);
        UpdateLevelState();
        unit.ResetUnitBuff();
        unit.ResetUnitState();
        if (shouldExecuteBatch)
            FieldManager.Inst.UnitBatch(unit.gameObject, unit.unitDrag.curIndex, false);
        unit.unitDrag.ResetLoaction();
    }



    void UpdateLevelState()
    {
        unit.BaseDamage = Mathf.Pow(1.3f, level - 1);
        unit.BaseAttackSpeed = Mathf.Pow(1f / 0.9f, level - 1);
        unit.BaseMaxHealth = Mathf.Pow(1.2f, level - 1);

        unit.RecalculateStats();
        unit.health = unit.maxHealth;
    }


    public int GetSaleRewardByLevel()
    {
        int reward;

        switch (unit.starLevel)
        {
            case 1:
                reward = 10;
                break;
            case 2:
                reward = 20;
                break;
            case 3:
                reward = 30;
                break;
            case 4:
                reward = 2;
                break;
            case 5:
                reward = 3;
                break;
            case 6:
                reward = 4;
                break;
            case 7:
                reward = 5;
                break;
            default:
                reward = 10;
                break;
        }

        return reward * GameStatManager.Inst.UnitSaleRewardMultiplier;
    }

    public ResourcesData.ResourcesType GetSaleResourceByLevel()
    {
        ResourcesData.ResourcesType resourceType;

        switch (unit.starLevel)
        {
            case 1:
                resourceType = ResourcesData.ResourcesType.coin;
                break;
            case 2:
                resourceType = ResourcesData.ResourcesType.coin;
                break;
            case 3:
                resourceType = ResourcesData.ResourcesType.coin;
                break;
            case 4:
                resourceType = ResourcesData.ResourcesType.darkMatteCoin;
                break;
            case 5:
                resourceType = ResourcesData.ResourcesType.darkMatteCoin;
                break;
            case 6:
                resourceType = ResourcesData.ResourcesType.darkMatteCoin;
                break;
            case 7:
                resourceType = ResourcesData.ResourcesType.darkMatteCoin;
                break;
            default:
                resourceType = ResourcesData.ResourcesType.coin;
                break;
        }

        return resourceType;

    }



}
