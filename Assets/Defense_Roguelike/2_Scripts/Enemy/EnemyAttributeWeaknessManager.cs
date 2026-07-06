using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttributeWeaknessManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] attributeIcons;

    private Enemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void Init()
    {
        enemy.attributeWeakness = enemy.attributeWeakness.OrderBy(w => w.data.attributeType).ToList();
        RefreshAttributeIcon();
    }

    public void AddAttribute(AttributeData.AttributeType attributeType, float value)
    {
        List<AttributeModifierData> attributeWeakness = enemy.attributeWeakness;

        if (attributeWeakness.Any(w => w.data.attributeType == attributeType))
            return;

        AttributeModifierData newAttributeData = new AttributeModifierData();
        newAttributeData.data = AttributeDatabase.Inst.GetData(attributeType);
        newAttributeData.value = value;

        enemy.attributeWeakness.Add(newAttributeData);
        enemy.attributeWeakness = enemy.attributeWeakness.OrderBy(w => w.data.attributeType).ToList();
        RefreshAttributeIcon();
    }


    public void RemoveAttribute(AttributeData.AttributeType attributeType)
    {
        List<AttributeModifierData> attributeWeakness = enemy.attributeWeakness;

        for (int i = 0; i < attributeWeakness.Count; i++)
        {
            if (attributeType == attributeWeakness[i].data.attributeType)
            {
                enemy.attributeWeakness.RemoveAt(i);
                RefreshAttributeIcon();
                break;
            }
        }
    }

    private void RefreshAttributeIcon()
    {
        for (int i = 0; i < attributeIcons.Length; i++)
        {
            attributeIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < enemy.attributeWeakness.Count; i++)
        {
            attributeIcons[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < enemy.attributeWeakness.Count; i++)
        {
            attributeIcons[i].sprite = enemy.attributeWeakness[i].data.icon;
        }
    }
}
