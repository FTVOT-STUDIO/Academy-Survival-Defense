using UnityEngine;

public class SortingOrderController : MonoBehaviour
{
    SpriteRenderer targetRenderer;
    SpriteRenderer spriteRenderer;
    ParticleSystemRenderer particleRenderer;
    [SerializeField] bool isFront;
    [SerializeField] int sortingOffset = 1;

    public void Init(SpriteRenderer target)
    {
        targetRenderer = target;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
    }

    void LateUpdate()
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = targetRenderer.sortingOrder + (isFront ? sortingOffset : -sortingOffset);
        if (particleRenderer != null)
            particleRenderer.sortingOrder = targetRenderer.sortingOrder + (isFront ? sortingOffset : -sortingOffset);

    }
}
