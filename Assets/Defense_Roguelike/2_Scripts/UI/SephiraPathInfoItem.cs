using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SephiraPathInfoItem : MonoBehaviour
{
    [SerializeField] Image pathIcon;
    [SerializeField] TMP_Text pathName;
    [SerializeField] TMP_Text pathDesc;
    public GameObject selectedImage;

    public void Init(SephiraPathData sephiraPathData)
    {
        pathIcon.sprite = sephiraPathData.mainIcon;
        pathIcon.color = sephiraPathData.sephiraColor;
        pathName.text = sephiraPathData.sephiraName;
        pathDesc.text = sephiraPathData.sephiraPathDesc;
    }
}
