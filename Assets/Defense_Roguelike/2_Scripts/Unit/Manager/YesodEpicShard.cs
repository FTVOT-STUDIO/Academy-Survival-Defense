using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YesodEpicShard : MonoBehaviour
{
    public Material material;
    bool claimed;
    float elapsed;
    SpriteRenderer visual;
    readonly List<RaycastResult> uiHits = new List<RaycastResult>();
    void Awake() { InitializeVisual(); }
    public void InitializeVisual()
    {
        if (visual != null) return;
        visual = gameObject.AddComponent<SpriteRenderer>();
        visual.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 2, 2), Vector2.one * .5f, 2);
        visual.sharedMaterial = material;
        visual.sortingLayerName = "Particle";
        visual.sortingOrder = 200;
        gameObject.AddComponent<BoxCollider2D>().size = Vector2.one * 1.6f;
        transform.localScale = Vector3.one * .9f;
    }
    void OnEnable() { claimed = false; elapsed = 0; transform.localScale = Vector3.one * .9f; }
    void Update()
    {
        if (GameManager.Inst != null && GameManager.Inst.isPause) return;
        elapsed += Time.deltaTime;
        transform.localScale = Vector3.one * (.9f + Mathf.Sin(elapsed * 4) * .07f);
    }
    bool OverInteractiveUI()
    {
        if (EventSystem.current == null) return false;
        var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        uiHits.Clear();
        EventSystem.current.RaycastAll(pointer, uiHits);
        foreach (var hit in uiHits)
            if (hit.gameObject.GetComponentInParent<Selectable>() != null) return true;
        return false;
    }
    void OnMouseDown()
    {
        // 맵을 덮는 장식용 UI는 무시하고 실제 버튼·입력창 위에서만 수집을 막습니다.
        if (claimed || OverInteractiveUI()) return;
        if (YesodEpicController.Inst != null && YesodEpicController.Inst.Collect())
        { claimed = true; gameObject.SetActive(false); }
    }
    void OnDestroy()
    {
        if (visual != null && visual.sprite != null) Destroy(visual.sprite);
    }
}
