using UnityEngine;

public class FlipXPositionSync : MonoBehaviour
{
    public SpriteRenderer parentSpriteRenderer;
    public Vector3 baseLocalPosition;

    void Start()
    {
        baseLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        int dir = parentSpriteRenderer.flipX ? -1 : 1;
        transform.localPosition = new Vector3(baseLocalPosition.x * dir, baseLocalPosition.y, baseLocalPosition.z);
    }
}
