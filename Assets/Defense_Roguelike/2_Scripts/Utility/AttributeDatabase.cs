using System.Collections.Generic;
using UnityEngine;

public class AttributeDatabase : MonoBehaviour
{
    public static AttributeDatabase Inst;
    public List<AttributeData> attributeDatas;

    void Awake()
    {
        Inst = this;
    }

    public AttributeData GetData(AttributeData.AttributeType type)
    {
        foreach (var data in attributeDatas)
        {
            if (data.attributeType == type)
                return data;
        }
        return null;
    }

    public float GetAttributeMultiplier(AttributeData.AttributeType attributeType, AttributeModifierData[] attributeMultiplierDatas)
    {
        foreach (AttributeModifierData multiplierData in attributeMultiplierDatas)
        {
            if (multiplierData.data == null)
                continue;
            float mulitiplier = multiplierData.value;

            if (multiplierData.data.attributeType == attributeType)
                return Mathf.Max(0f, mulitiplier);
        }

        return 1f;
    }

}

[System.Serializable]
public class AttributeModifierData
{
    public AttributeData data;
    public float value = 1f;
}
