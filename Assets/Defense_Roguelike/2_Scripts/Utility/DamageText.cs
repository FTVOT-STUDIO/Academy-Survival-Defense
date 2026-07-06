using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    TMP_Text text;
    [SerializeField] List<AttributeData> attributeDatas;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void InitializeText(AttributeData.AttributeType type, string value, bool isCritical)
    {
        var data = attributeDatas.Find(a => a.attributeType == type);

        text.enableVertexGradient = isCritical;

        text.text = value;

        if (isCritical)
        {
            text.colorGradientPreset = data.tMP_ColorGradient;
            text.fontSize = 7;
        }
        else
        {
            text.color = data.attributeColor;
            text.fontSize = 4;
        }

    }

    void OnDisable()
    {
        text.color = Color.white;
        text.enableVertexGradient = false;
    }
}
