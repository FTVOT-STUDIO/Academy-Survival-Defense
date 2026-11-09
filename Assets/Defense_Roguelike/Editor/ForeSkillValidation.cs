#if UNITY_EDITOR
using UnityEditor;
public static class ForeSkillValidation
{
    [MenuItem("Tools/Fore/Validate Skills and Render Effect")]
    public static void Validate() { CharacterVfxValidation.Validate(); }
}
#endif
