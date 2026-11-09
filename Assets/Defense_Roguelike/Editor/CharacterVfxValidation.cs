#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CharacterVfxValidation
{
    const string Base = "Assets/Defense_Roguelike/";
    [InitializeOnLoadMethod]
    static void Schedule()
    {
        EditorApplication.delayCall += () => { if (!EditorApplication.isPlayingOrWillChangePlaymode) Validate(); };
    }

    [MenuItem("Tools/Effects/Validate Pixel FX")]
    public static void Validate()
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var views = new List<SayaRollVisual>();
        RenderTexture texture = null;
        Camera camera = null;
        try
        {
            foreach (string id in new[] { "saya-charge", "saya-result", "fore-rift", "malkuth-collapse" })
            {
                var clip = AssetDatabase.LoadAssetAtPath<PixelFxClip>(Base + "3_Sprites/Particle/PixelFX/" + id + ".asset");
                Require(clip != null && clip.atlas != null, id + " atlas reference");
                Require(clip.frameCount > 1 && clip.frameWidth > 0 && clip.frameHeight > 0, id + " frame metadata");
                Require(clip.atlas.width == clip.frameWidth * clip.columns, id + " atlas width");
                Require(clip.atlas.height >= Mathf.CeilToInt(clip.frameCount / (float)clip.columns) * clip.frameHeight, id + " atlas height");
                for (int i = 0; i < clip.frameCount; i++)
                {
                    Rect uv = clip.FrameUV(i);
                    Require(uv.xMin >= -0.001f && uv.yMin >= -0.001f && uv.xMax <= 1.001f && uv.yMax <= 1.001f, id + " UV " + i);
                }
                var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip.atlas));
                Require(importer.filterMode == FilterMode.Point && !importer.mipmapEnabled && importer.textureCompression == TextureImporterCompression.Uncompressed, id + " point / lossless import");
            }
            foreach (Vector2 size in new[] { new Vector2(1920, 1080), new Vector2(1280, 720), new Vector2(800, 600), new Vector2(2560, 1080), new Vector2(640, 360) })
            {
                Rect safe = new Rect(12, 12, size.x - 24, size.y - 24);
                foreach (Vector2 point in new[] { Vector2.zero, size, new Vector2(-999, 9999), size * 0.5f })
                {
                    Rect card = SayaRollVisual.ClampScreenRect(point, new Vector2(200, 112), safe);
                    Require(card.xMin >= safe.xMin && card.yMin >= safe.yMin && card.xMax <= safe.xMax && card.yMax <= safe.yMax, "Saya screen edge clamp");
                }
            }
            var manager = AssetDatabase.LoadAssetAtPath<GameObject>(Base + "1_Prefabs/UnitManager/SayaCharacterManager.prefab").GetComponent<SayaCharacterManager>();
            var fore = AssetDatabase.LoadAssetAtPath<GameObject>(Base + "1_Prefabs/UnitManager/CharacterRiftEffect.prefab");
            var malkuth = AssetDatabase.LoadAssetAtPath<GameObject>(Base + "1_Prefabs/UI/MalkuthCollapse.prefab");
            Require(manager.skillEffectPrefab == fore && manager.rollVisualPrefab != null, "Saya pool references");
            var skill = AssetDatabase.LoadAssetAtPath<MalkuthTransSkill>(Base + "0_Data/0_Unit/5_Sephira/1_Ranged/3_Malkuth/Malkuth Skill/MalkuthTransSkill.asset");
            Require(skill.collapseEffectPrefab == malkuth, "Malkuth finish trigger");
            Require(malkuth.GetComponent<MalkuthCollapseUI>().font == manager.rollingFont, "BlackHanSans UI font");
            Require(Mathf.FloorToInt(MalkuthCollapseUI.ImpactTime * 30) == 13, "impact frame sync");
            var urpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Base + "1_Prefabs/URP/Malkuth Self-Destruct.prefab");
            Require(urpPrefab.GetComponent<MalkuthBlackholeEffect>() != null, "existing Malkuth URP component");
            Require(MalkuthCollapseUI.Duration < 1.5f && MalkuthCollapseUI.ImpactTime < 0.5f, "fast forced collapse");
            var poolObject = new GameObject("Pixel FX Pool Test");
            SceneManager.MoveGameObjectToScene(poolObject, scene);
            var pool = poolObject.AddComponent<PoolManager>();
            var first = pool.Get(fore, Vector3.zero);
            var fx = first.GetComponent<ForeSkillEffect>();
            fx.Begin(4, Color.white, 99, 0.7f);
            var player = fx.GetComponentInChildren<PixelFxPlayer>();
            player.Sample(0.14f);
            int childCount = first.GetComponentsInChildren<Transform>(true).Length;
            first.SetActive(false);
            var reused = pool.Get(fore, Vector3.zero);
            Require(reused == first, "effect reuse");
            fx.Begin(4, Color.white, 99, 0.7f);
            player.Sample(0.14f);
            Require(first.GetComponentsInChildren<Transform>(true).Length == childCount, "no child growth after reuse");
            Require(player.transform.localScale.x * fx.riftClip.frameWidth / 100 <= 1.701f, "Fore effect width cap");
            Require(pool.Get(fore, Vector3.one) != first, "concurrent effects");
            var cameraObject = new GameObject("Effect Validation Camera");
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            camera = cameraObject.AddComponent<Camera>();
            camera.scene = scene;
            camera.orthographic = true;
            camera.orthographicSize = 1.15f;
            camera.transform.position = new Vector3(0, 0, -10);
            camera.cullingMask = 1 << 31;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.055f, 0.06f, 0.09f);
            texture = new RenderTexture(1280, 720, 24);
            camera.targetTexture = texture;
            Layers(first);
            Save(camera, texture, "/tmp/Fore_PixelFX.png");
            foreach (Transform child in poolObject.transform) child.gameObject.SetActive(false);
            var owner = new GameObject("Saya Anchor");
            SceneManager.MoveGameObjectToScene(owner, scene);
            var unit = owner.AddComponent<Unit>();
            for (int i = 0; i < 5; i++)
            {
                var view = pool.Get(manager.rollVisualPrefab, Vector3.zero).GetComponent<SayaRollVisual>();
                views.Add(view);
                var panel = view.Prepare(manager, 95 - i, unit);
                panel.ShowResult(95 - i, "올 세이브 챌린지 루트");
                var canvas = view.GetComponentInChildren<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 5;
                canvas.GetComponent<CanvasScaler>().enabled = false;
                canvas.scaleFactor = 1280f / 1920;
                Layers(view.gameObject);
                Canvas.ForceUpdateCanvases();
                view.Layout(camera, new Rect(0, 0, 1280, 720));
                Require(view.GetComponentsInChildren<Camera>(true).Length == 0, "dice has no camera");
            }
            for (int i = 0; i < views.Count; i++)
                for (int j = i + 1; j < views.Count; j++)
                {
                    Rect a = views[i].ScreenBounds, b = views[j].ScreenBounds;
                    float width = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin);
                    float height = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
                    Require(width <= 0.5f || height <= 0.5f, "five simultaneous cards do not overlap");
                }
            Canvas.ForceUpdateCanvases();
            Save(camera, texture, "/tmp/Saya_Compact_UI.png");
            foreach (var view in views) view.gameObject.SetActive(false);
            var collapse = pool.Get(malkuth, Vector3.zero).GetComponent<MalkuthCollapseUI>();
            collapse.Play();
            var collapseCanvas = collapse.GetComponentInChildren<Canvas>();
            collapseCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            collapseCanvas.worldCamera = camera;
            collapseCanvas.planeDistance = 5;
            Layers(collapse.gameObject);
            collapse.Sample(0.18f);
            Canvas.ForceUpdateCanvases();
            Save(camera, texture, "/tmp/Malkuth_Collapse_UI.png");
            File.WriteAllText("/tmp/Defense_Roguelike_PixelFXValidation.txt", "PASS: four lossless pixelfx atlases and every frame UV; five viewport edge clamps; pooled reuse/concurrency/child count; Fore width cap; BlackHanSans; camera-free dice; Saya five-roll UI rendering; Malkuth frame-13 impact and UI rendering.\n" + DateTime.Now.ToString("O"));
            Debug.Log("[PixelFX] 아틀라스·화면 경계·풀링·사야 UI·말쿠트 렌더 검사를 통과했습니다.");
        }
        catch (Exception error)
        {
            File.WriteAllText("/tmp/Defense_Roguelike_PixelFXValidation.txt", "FAIL: " + error);
            Debug.LogError("[PixelFX] " + error);
        }
        finally
        {
            foreach (var view in views)
            {
                if (view == null) continue;
                view.SendMessage("OnDisable", SendMessageOptions.DontRequireReceiver);
                foreach (var dice in view.GetComponentsInChildren<SayaDice>(true)) dice.SendMessage("OnDisable", SendMessageOptions.DontRequireReceiver);
            }
            if (camera != null) camera.targetTexture = null;
            if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
            EditorSceneManager.ClosePreviewScene(scene);
        }
    }

    static void Layers(GameObject obj)
    {
        foreach (var child in obj.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = 31;
    }

    static void Save(Camera camera, RenderTexture texture, string path)
    {
        var previous = RenderTexture.active;
        Texture2D output = null;
        try
        {
            camera.Render();
            RenderTexture.active = texture;
            output = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
            output.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            output.Apply();
            File.WriteAllBytes(path, output.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previous;
            if (output != null) UnityEngine.Object.DestroyImmediate(output);
        }
    }

    static void Require(bool condition, string description)
    {
        if (!condition) throw new InvalidOperationException(description);
    }
}
#endif
