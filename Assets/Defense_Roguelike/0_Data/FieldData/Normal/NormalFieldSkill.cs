using UnityEngine;

[CreateAssetMenu(fileName = "NormalFieldSkill", menuName = "FieldSkill/NormalFieldSkill")]
public class NormalFieldSkill : ScriptableObject, IFieldOnEnablePassive
{
    public void ApplyFieldOnEnablePassive(FieldData fieldData, MonoBehaviour monoBehaviour)
    {
        GameStatManager.Inst.AddGameDamage(fieldData.value);
    }
}
