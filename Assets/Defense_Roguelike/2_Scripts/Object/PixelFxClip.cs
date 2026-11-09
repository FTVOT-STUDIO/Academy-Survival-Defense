using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Pixel FX Clip")]
public class PixelFxClip : ScriptableObject
{
    public Texture2D atlas;
    public int frameWidth, frameHeight, columns, frameCount;
    public float fps = 24;
    public bool loop;
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
    Sprite[] sprites;
    public float Duration => frameCount / Mathf.Max(1, fps);

    public Rect FrameUV(int index)
    {
        index = Mathf.Clamp(index, 0, frameCount - 1);
        return new Rect((index % columns) * frameWidth / (float)atlas.width,
        1 - (index / columns + 1) * frameHeight / (float)atlas.height,
        frameWidth / (float)atlas.width, frameHeight / (float)atlas.height);
    }

    public Sprite Frame(int index)
    {
        index = Mathf.Clamp(index, 0, frameCount - 1);
        if (sprites == null || sprites.Length != frameCount) sprites = new Sprite[frameCount];
        if (sprites[index] == null)
        {
            Rect uv = FrameUV(index);
            sprites[index] = Sprite.Create(atlas, new Rect(uv.x * atlas.width, uv.y * atlas.height, frameWidth, frameHeight), pivot, 100);
            sprites[index].name = name + " " + index;
        }
        return sprites[index];
    }

    void OnDisable()
    {
        if (sprites == null) return;
        foreach (var sprite in sprites)
        {
            if (sprite == null) continue;
            if (Application.isPlaying) Destroy(sprite);
            else DestroyImmediate(sprite);
        }
        sprites = null;
    }
}
