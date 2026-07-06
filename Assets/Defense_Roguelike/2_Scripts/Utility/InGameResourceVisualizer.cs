using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameResourceVisualizer : MonoBehaviour
{
    [SerializeField] ResourcesData[] resourceTypeInfos;
    [Space]
    [SerializeField] SpriteRenderer resourceSprite;

    public void Init(ResourcesData.ResourcesType resourceType)
    {
        foreach (var info in resourceTypeInfos)
        {
            if (info.resourcesType == resourceType)
            {
                resourceSprite.sprite = info.icon;
                return;
            }
        }
    }
}
