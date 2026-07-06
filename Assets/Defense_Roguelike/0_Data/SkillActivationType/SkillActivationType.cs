using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillActivationType", menuName = "SkillActivationType", order = 0)]
public class SkillActivationType : ScriptableObject
{
    public enum ActivationType { Passive, BasicAttack, CoolTime, Button, Event, Ability }
    public Sprite icon;
    [Space]
    public ActivationType activationType;

    public Dictionary<ActivationType, string> ActivationTypeToKorean = new Dictionary<ActivationType, string>
    {
        {ActivationType.Passive, "패시브" },
        {ActivationType.BasicAttack, "기본공격" },
        {ActivationType.CoolTime, "쿨타임" },
        {ActivationType.Button, "버튼" },
        {ActivationType.Event, "이벤트" },
        {ActivationType.Ability, "궁극기" },
    };
}
