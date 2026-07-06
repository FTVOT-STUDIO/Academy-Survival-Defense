using UnityEngine;

[CreateAssetMenu(fileName = "SlowFieldSkill", menuName = "FieldSkill/SlowFieldSkill")]
public class SlowFieldSkill : ScriptableObject, IFieldOnEnablePassive
{
    public void ApplyFieldOnEnablePassive(FieldData fieldData, MonoBehaviour monoBehaviour)
    {
        GameStatManager.Inst.AddGameSlowValue(fieldData.value);
    }
}
