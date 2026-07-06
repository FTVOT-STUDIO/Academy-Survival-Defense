using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] ResourcesData[] resourceTypeInfos;

    [SerializeField] TMP_Text infoText;
    [SerializeField] TMP_Text subToolTipText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject iconGroup;

    public void Init(string mainTooltip, bool useGroup = false, string subToolTip = null, bool useImage = false, Sprite iconSprite = null)
    {
        infoText.text = mainTooltip;

        iconGroup.SetActive(useGroup);

        subToolTipText.text = subToolTip;
        iconImage.gameObject.SetActive(useImage);
        if (useImage)
        {
            iconImage.sprite = iconSprite;
        }
    }
}
