using UnityEngine;

public class PixelFxPlayer : MonoBehaviour
{
    public Material material;
    SpriteRenderer spriteRenderer;
    PixelFxClip clip;
    float elapsed, duration;
    bool repeat;

    public void Play(PixelFxClip animation, float width, float seconds, bool loop, Color color, int order = 50)
    {
        if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if (material != null) spriteRenderer.sharedMaterial = material;
        clip = animation;
        elapsed = 0;
        duration = Mathf.Max(0.05f, seconds);
        repeat = loop;
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = order;
        if (clip == null || clip.atlas == null) { spriteRenderer.enabled = false; return; }
        spriteRenderer.enabled = true;
        transform.localScale = Vector3.one * (width * 100 / clip.frameWidth);
        Sample(0);
    }

    public void Sample(float time)
    {
        if (clip == null) return;
        float phase = repeat ? Mathf.Repeat(time, clip.Duration) / clip.Duration : Mathf.Clamp01(time / duration);
        spriteRenderer.sprite = clip.Frame(Mathf.Min(clip.frameCount - 1, Mathf.FloorToInt(phase * clip.frameCount)));
    }

    void Update()
    {
        if (GameManager.Inst != null && GameManager.Inst.isPause) return;
        elapsed += Time.deltaTime;
        Sample(elapsed);
    }

    void OnDisable()
    {
        clip = null;
        if (spriteRenderer != null) spriteRenderer.sprite = null;
    }
}
