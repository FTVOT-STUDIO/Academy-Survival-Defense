using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaleButton : MonoBehaviour
{
    [SerializeField] ResourcesData[] resourceTypeInfos;
    [SerializeField] TMP_Text resourcesText;
    [SerializeField] Image resourcesImage;

    public void Init(string resourcesAmount, ResourcesData.ResourcesType resourceType)
    {
        resourcesText.text = resourcesAmount;

        foreach (var info in resourceTypeInfos)
        {
            if (info.resourcesType == resourceType)
            {
                resourcesImage.sprite = info.icon;
                return;
            }
        }
    }
}
