using UnityEngine;
using UnityEngine.UI;

public class ScrollingUIImage : MonoBehaviour
{
    Material material;
    [SerializeField] Image image;
    [SerializeField] bool ignoreTimeScale = false;
    [SerializeField] float x;
    [SerializeField] float y;
    Vector2 offset;

    void Awake()
    {
        InitMaterial(image.material);
    }

    public void InitMaterial(Material mat)
    {
        material = mat;
        image.material = mat;
        offset = material.mainTextureOffset;
    }

    void Update()
    {
        float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        offset.x += delta * x;
        offset.y += delta * y;

        material.mainTextureOffset = offset;
    }

    void OnDisable()
    {
        offset.x = 0;
        offset.y = 0;

        material.mainTextureOffset = offset;
    }

}
