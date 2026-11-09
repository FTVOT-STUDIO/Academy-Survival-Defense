#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Unity 재컴파일 뒤 실제 에셋 연결과 모든 주사위 경계를 검사합니다.
public static class SayaSkillValidation
{
    private const string ManagerPath = "Assets/Defense_Roguelike/1_Prefabs/UnitManager/SayaCharacterManager.prefab";
    public const string ReportPath = "/tmp/Defense_Roguelike_SayaValidation.txt";

    [InitializeOnLoadMethod]
    private static void ScheduleValidation()
    {
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) Validate();
        };
    }

    [MenuItem("Tools/Saya/Validate Skill Setup")]
    public static void Validate()
    {
        try
        {
            int[] counts = new int[7];
            for (int value = 0; value <= 100; value++)
            {
                int expected = value == 1 ? 0 : value <= 10 ? 1 : value <= 30 ? 2 : value <= 50 ? 3 : value <= 70 ? 4 : value <= 90 ? 5 : 6;
                Require(SayaCharacterManager.GetResultIndex(value) == expected, "주사위 경계: " + value);
                counts[expected]++;
                foreach (SayaCharacterManager.UnitType type in Enum.GetValues(typeof(SayaCharacterManager.UnitType)))
                    Require(!string.IsNullOrEmpty(SayaCharacterManager.GetResultName(type, value)), "결과 이름: " + type + " " + value);
            }
            Require(counts[0] == 1 && counts[1] == 10 && counts[2] == 20 && counts[3] == 20 && counts[4] == 20 && counts[5] == 20 && counts[6] == 10, "주사위 확률 분포");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ManagerPath);
            Require(prefab != null, "사야 매니저 프리팹");
            var manager = prefab.GetComponent<SayaCharacterManager>();
            var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Defense_Roguelike/Model/Saya_Dice/Dice.fbx");
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(model, out string modelGuid, out long modelId);
            Require(manager.diceModel == model, "주사위 모델 연결 (실제 fileID=" + modelId + ")");
            Require(manager.rollingPanel != null, "주사위 패널 연결");
            var importer = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(model));
            Require(!importer.importCameras && !importer.importLights, "주사위 카메라 및 조명 가져오기 차단");
            Require(model.GetComponentsInChildren<Camera>(true).Length == 0, "주사위 내장 카메라 제거");
            var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/Defense_Roguelike/Fonts/BlackHanSans-Regular SDF.asset");
            Require(font != null && manager.rollingFont == font, "BlackHanSans 결과 폰트 연결");
            Require(manager.diceMaterial != null && manager.diceMaterial.shader.isSupported, "주사위 Unlit 머티리얼");
            Require(manager.fieldData != null && manager.hunterData != null && manager.shamanData != null, "세 형태의 데이터");
            Require(manager.fieldData.unitSkill is SayaTreasureFieldSkill && manager.hunterData.unitSkill == manager.fieldData.unitSkill && manager.shamanData.unitSkill == manager.fieldData.unitSkill, "공유 스킬 데이터");
            Require(manager.fieldData.attribute.attributeType == AttributeData.AttributeType.Ice, "전방 결의 속성");
            Require(manager.hunterData.attribute.attributeType == AttributeData.AttributeType.Darkness, "중앙 혼돈 속성");
            Require(manager.shamanData.attribute.attributeType == AttributeData.AttributeType.Water, "후방 와류 속성");
            Require(manager.fieldData.baseCoolTime > 0 && manager.hunterData.baseCoolTime > 0 && manager.shamanData.baseCoolTime > 0, "행동 선언 기본 쿨타임");
            foreach (var controllers in new[] { manager.frontAnim, manager.middleAnim, manager.backAnim })
            {
                Require(controllers.Length == 5, "변신 애니메이션 단계 수");
                var appearances = new System.Collections.Generic.HashSet<Sprite>();
                foreach (var controller in controllers)
                {
                    Require(controller != null, "변신 애니메이션 연결");
                    var overrides = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<AnimationClip, AnimationClip>>();
                    controller.GetOverrides(overrides);
                    Sprite idleSprite = null;
                    foreach (var pair in overrides)
                    {
                        AnimationClip clip = pair.Value != null ? pair.Value : pair.Key;
                        if (clip == null || clip.name.IndexOf("Idle", StringComparison.OrdinalIgnoreCase) < 0) continue;
                        foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                        {
                            if (binding.type != typeof(SpriteRenderer) || binding.propertyName != "m_Sprite") continue;
                            var keys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                            if (keys.Length > 0) idleSprite = keys[0].value as Sprite;
                        }
                    }
                    Require(idleSprite != null, "변신 Idle 스프라이트 연결");
                    Require(appearances.Add(idleSprite), "단계별 외형 스프라이트 구분");
                }
            }
            File.WriteAllText(ReportPath, "PASS: 303 roll results, distribution 1/10/20/20/20/20/10, model/panel, no imported cameras/lights, BlackHanSans font, attributes, cooldowns, shared skill and 15 transformation controllers with distinct Idle sprites.\n" + DateTime.Now.ToString("O"));
            Debug.Log("[Saya] 스킬 설정 검증에 성공했습니다. " + ReportPath);
        }
        catch (Exception error)
        {
            File.WriteAllText(ReportPath, "FAIL: " + error);
            Debug.LogException(error);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("[Saya] " + message);
    }
}
#endif
