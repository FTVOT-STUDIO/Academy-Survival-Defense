using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SayaRollVisual : MonoBehaviour
{
    static readonly List<SayaRollVisual> activeViews = new List<SayaRollVisual>();
    SayaDice dice;
    SayaDiceRollingUI panel;
    Canvas canvas;
    RectTransform card;
    Transform diceHolder;
    Unit caster;
    Rect occupied;
    public Rect ScreenBounds => occupied;

    public SayaDiceRollingUI Prepare(SayaCharacterManager settings, int value, Unit owner)
    {
        caster = owner;
        if (canvas == null)
        {
            var ui = new GameObject("Saya Screen UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            ui.transform.SetParent(transform, false);
            canvas = ui.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 250;
            var scaler = ui.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            var obj = new GameObject("Saya Compact Result", typeof(RectTransform));
            obj.transform.SetParent(ui.transform, false);
            card = obj.GetComponent<RectTransform>();
            card.sizeDelta = new Vector2(192, 104);
            panel = obj.AddComponent<SayaDiceRollingUI>();
            panel.Initialize(settings.rollingPanel, settings.rollingFont);
        }
        if (diceHolder == null && settings.diceModel != null)
        {
            diceHolder = new GameObject("Centered Dice").transform;
            diceHolder.SetParent(transform, false);
            var spin = new GameObject("Dice Rotation").transform;
            spin.SetParent(diceHolder, false);
            var model = Instantiate(settings.diceModel, spin);
            Bounds bounds = ModelBounds(model);
            float size = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            model.transform.localScale *= 1 / Mathf.Max(0.001f, size);
            bounds = ModelBounds(model);
            model.transform.position -= bounds.center - spin.position;
            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
            {
                if (settings.diceMaterial != null) renderer.sharedMaterial = settings.diceMaterial;
                renderer.sortingOrder = 260;
            }
            // 메시 원점 대신 중앙 피벗을 회전하여 궤도를 벗어나지 않습니다.
            dice = spin.gameObject.AddComponent<SayaDice>();
        }
        if (dice != null)
        {
            dice.transform.localRotation = Quaternion.identity;
            dice.Play(settings.rollingDuration);
        }
        if (!activeViews.Contains(this)) activeViews.Add(this);
        panel.PlayRandomTextAnimation(value, settings.rollingDuration);
        LateUpdate();
        return panel;
    }

    static Bounds ModelBounds(GameObject model)
    {
        Bounds result = new Bounds(model.transform.position, Vector3.zero);
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>()) result.Encapsulate(renderer.bounds);
        return result;
    }

    public static Rect ClampScreenRect(Vector2 center, Vector2 size, Rect safe)
    {
        size.x = Mathf.Min(size.x, safe.width);
        size.y = Mathf.Min(size.y, safe.height);
        center.x = Mathf.Clamp(center.x, safe.xMin + size.x * 0.5f, safe.xMax - size.x * 0.5f);
        center.y = Mathf.Clamp(center.y, safe.yMin + size.y * 0.5f, safe.yMax - size.y * 0.5f);
        return new Rect(center - size * 0.5f, size);
    }

    bool OverlapsPrevious(Rect candidate)
    {
        foreach (var view in activeViews)
        {
            if (view == this) break;
            if (view != null && view.gameObject.activeInHierarchy && view.occupied.Overlaps(candidate)) return true;
        }
        return false;
    }

    void LateUpdate() { Layout(Camera.main, Screen.safeArea); }

    public void Layout(Camera camera, Rect viewport)
    {
        if (canvas == null || camera == null || caster == null || !caster.gameObject.activeInHierarchy) return;
        Vector3 point = camera.WorldToScreenPoint(caster.transform.position);
        if (point.z <= 0) return;
        float factor = Mathf.Max(0.1f, canvas.scaleFactor);
        Vector2 size = new Vector2(200, 112) * factor;
        Rect safe = viewport;
        safe.xMin += 8; safe.xMax -= 8; safe.yMin += 8; safe.yMax -= 8;
        Vector2 desired = (Vector2)point + Vector2.up * 88 * factor;
        Rect candidate = ClampScreenRect(desired, size, safe);
        for (int attempt = 0; OverlapsPrevious(candidate) && attempt < 120; attempt++)
        {
            Vector2 center;
            if (attempt < 32)
            {
                int ring = attempt / 8 + 1;
                int direction = attempt % 8;
                Vector2 offset = direction == 0 ? Vector2.up : direction == 1 ? Vector2.right
                    : direction == 2 ? Vector2.left : direction == 3 ? Vector2.down
                    : direction == 4 ? Vector2.one : direction == 5 ? new Vector2(-1, 1)
                    : direction == 6 ? new Vector2(1, -1) : -Vector2.one;
                center = desired + Vector2.Scale(offset, size) * ring;
            }
            else
            {
                int slot = attempt - 32;
                int columns = Mathf.Max(1, Mathf.FloorToInt(safe.width / size.x));
                center = new Vector2(safe.xMin + size.x * (0.5f + slot % columns), safe.yMax - size.y * (0.5f + slot / columns));
            }
            candidate = ClampScreenRect(center, size, safe);
        }
        occupied = candidate;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, candidate.center, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out Vector2 local);
        card.anchoredPosition = local;
        if (diceHolder == null) return;
        Vector2 diceScreen = candidate.center + new Vector2(-67, 12) * factor;
        Vector3 centerWorld = camera.ScreenToWorldPoint(new Vector3(diceScreen.x, diceScreen.y, point.z));
        Vector3 edgeWorld = camera.ScreenToWorldPoint(new Vector3(diceScreen.x + 38 * factor, diceScreen.y, point.z));
        diceHolder.position = centerWorld;
        diceHolder.localScale = Vector3.one * Vector3.Distance(centerWorld, edgeWorld);
    }

    void OnDisable()
    {
        activeViews.Remove(this);
        caster = null;
        occupied = default;
    }
}
