#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class YesodEpicValidation
{
    [InitializeOnLoadMethod]
    static void Schedule() { EditorApplication.delayCall += Validate; }
    [MenuItem("Tools/Yesod/Validate Epic")]
    public static void Validate()
    {
        try
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Require(YesodEpicText.Fragments.Length == 100, "100 fragments");
            for (int chapter = 0; chapter < 4; chapter++)
            {
                string text = "";
                for (int index = chapter * 25; index < (chapter + 1) * 25; index++)
                {
                    Require(!string.IsNullOrWhiteSpace(YesodEpicText.Fragments[index]), "nonempty fragment");
                    text += YesodEpicText.Fragments[index];
                }
                Require(text == YesodEpicText.Chapters[chapter], "exact original chapter " + chapter);
            }
            Require(YesodEpicText.Fragments[99] == "One of a menace", "final phrase");
            string folder = "Assets/Defense_Roguelike/0_Data/0_Unit/5_Sephira/1_Ranged/9_Yesod/";
            var normal = AssetDatabase.LoadAssetAtPath<UnitData>(folder + "9_Yesod.asset");
            var truth = AssetDatabase.LoadAssetAtPath<UnitData>(folder + "9_Yesod Trans.asset");
            Require(normal.unitSkill is YesodSkill && truth.unitSkill is YesodSkill, "skill links");
            Require(((ISephiraSkillBase)normal.unitSkill).GenerateRuntimeSkill() is IAttackPassive, "normal Yesod attack interface");
            Require(((ISephiraSkillBase)truth.unitSkill).GenerateRuntimeSkill() is IAttackPassive, "truth Yesod attack interface");
            Require(normal.transData == truth && ((YesodSkill)truth.unitSkill).truthForm, "manual transformation");
            var prefab = ((YesodSkill)normal.unitSkill).managerPrefab;
            var controller = prefab.GetComponent<YesodEpicController>();
            Require(controller.shardPrefab != null && controller.font != null && controller.hypnosisMaterial != null, "pooled visual references");
            Require(!ShaderUtil.ShaderHasError(controller.hypnosisMaterial.shader), "hypnosis shader");
            Require(controller.hypnosisMaterial.HasProperty("_MainTex"), "RawImage texture property");
            ValidateRawImage(controller.hypnosisMaterial);
            ValidateStartLifecycle(prefab);
            File.WriteAllText("/tmp/Defense_Roguelike_YesodValidation.txt", "PASS: 100 nonempty fragments; each 25-fragment chapter reconstructs the exact original including newlines; final phrase; both skill links; transformation; pooled prefab/font/material links; shader and actual RawImage rendering; pre-start wait and running-to-ended lifecycle; repeated Y setup remains at 99 percent with exactly one visible clickable foreground shard; both forms implement normal attack.\n" + DateTime.Now.ToString("O"));
            Debug.Log("[Yesod] 원문·100개 파편·변신·에셋 연결 검사를 통과했습니다.");
        }
        catch (Exception e) { File.WriteAllText("/tmp/Defense_Roguelike_YesodValidation.txt", "FAIL: " + e); Debug.LogException(e); }
    }
    static void ValidateStartLifecycle(GameObject prefab)
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
        var previous = GameManager.Inst;
        var previousField = FieldManager.Inst;
        var previousPool = PoolManager.Inst;
        try
        {
            var gameObject = new GameObject("Yesod Lifecycle Game");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, scene);
            var game = gameObject.AddComponent<GameManager>();
            GameManager.Inst = game;
            var ownerObject = new GameObject("Yesod Lifecycle Owner");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(ownerObject, scene);
            var unit = ownerObject.AddComponent<SephiraUnit>();
            unit.maxAbllityCount = 200;
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            var controller = instance.GetComponent<YesodEpicController>();
            controller.Bind(unit, false);
            var tick = typeof(YesodEpicController).GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            game.isGameStart = false;
            tick.Invoke(controller, null);
            Require(instance.activeSelf, "manager survives pre-start initialization");
            game.isGameStart = true;
            tick.Invoke(controller, null);
            Require(instance.activeSelf, "manager runs during battle");
            var fieldObject = new GameObject("Yesod Debug Field");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(fieldObject, scene);
            FieldManager.Inst = fieldObject.AddComponent<FieldManager>();
            FieldManager.Inst.fieldSlots = new[] { fieldObject.transform };
            var poolObject = new GameObject("Yesod Debug Pool");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(poolObject, scene);
            PoolManager.Inst = poolObject.AddComponent<PoolManager>();
            for (int repeat = 0; repeat < 3; repeat++)
            {
                Require(controller.PrepareLastShardForDebug(), "Y creates exactly one last shard");
                var markers = poolObject.GetComponentsInChildren<YesodEpicShard>(true);
                foreach (var marker in markers)
                {
                    if (!marker.gameObject.activeSelf) continue;
                    marker.InitializeVisual();
                    var renderer = marker.GetComponent<SpriteRenderer>();
                    Require(renderer.sortingLayerName == "Particle" && renderer.sortingOrder >= 200, "shard draws above map");
                    Require(marker.transform.localScale.x >= .8f && marker.GetComponent<BoxCollider2D>() != null, "visible clickable shard");
                }
                Require(controller.Collected == 99 && Mathf.Approximately(unit.abllityCount / unit.maxAbllityCount, .99f), "Y sets count and gauge to 99 percent");
                Require(!controller.Ready, "Y does not auto-complete the epic");
            }
            game.isGameStart = false;
            tick.Invoke(controller, null);
            Require(!instance.activeSelf, "manager stops after actual battle end");
        }
        finally
        {
            GameManager.Inst = previous;
            FieldManager.Inst = previousField;
            PoolManager.Inst = previousPool;
            UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(scene);
        }
    }

    static void ValidateRawImage(Material material)
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
        RenderTexture target = null;
        Camera camera = null;
        string textureError = null;
        Application.LogCallback listener = (message, stack, type) =>
        {
            if (message.Contains("_MainTex") && message.Contains("YesodHypnosis")) textureError = message;
        };
        Application.logMessageReceived += listener;
        try
        {
            var cameraObject = new GameObject("Yesod UI Validation Camera");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            camera = cameraObject.AddComponent<Camera>();
            camera.scene = scene;
            camera.orthographic = true;
            camera.orthographicSize = .6f;
            camera.transform.position = new Vector3(0, 0, -10);
            camera.cullingMask = 1 << 31;
            target = new RenderTexture(256, 256, 24);
            camera.targetTexture = target;
            var root = new GameObject("Yesod RawImage Validation", typeof(RectTransform), typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root, scene);
            root.layer = 31;
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(512, 512);
            root.transform.localScale = Vector3.one * .002f;
            var imageObject = new GameObject("Hypnosis", typeof(RectTransform), typeof(UnityEngine.UI.RawImage));
            imageObject.layer = 31;
            imageObject.transform.SetParent(root.transform, false);
            imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(512, 512);
            var image = imageObject.GetComponent<UnityEngine.UI.RawImage>();
            image.texture = Texture2D.whiteTexture;
            image.material = material;
            image.raycastTarget = false;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            Require(textureError == null, textureError ?? "RawImage rendering");
        }
        finally
        {
            Application.logMessageReceived -= listener;
            if (camera != null) camera.targetTexture = null;
            if (target != null) UnityEngine.Object.DestroyImmediate(target);
            UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(scene);
        }
    }

    static void Require(bool condition, string description) { if (!condition) throw new InvalidOperationException(description); }
}
#endif
