using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeData", menuName = "AttributeData", order = 0)]
public class AttributeData : ScriptableObject
{
    public Sprite icon;
    public string attributeName;
    [TextArea]
    public string desc;
    public enum AttributeType { Fixed, Fire, Water, Natura, Electro, Ice, Wind, Poison, Light, Darkness }
    public AttributeType attributeType;
    public Color attributeColor;
    public TMP_ColorGradient tMP_ColorGradient;
}
