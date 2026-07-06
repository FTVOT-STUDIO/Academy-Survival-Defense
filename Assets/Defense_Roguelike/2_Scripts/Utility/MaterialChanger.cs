using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] Material originalMat;
    [SerializeField] Material replacementMat;
    [SerializeField] SpriteRenderer targetRenderer;

    public void SetReplacementMaterial()
    {
        targetRenderer.material = replacementMat;
    }
    public void SetOriginalMaterial()
    {
        targetRenderer.material = originalMat;
    }
}
