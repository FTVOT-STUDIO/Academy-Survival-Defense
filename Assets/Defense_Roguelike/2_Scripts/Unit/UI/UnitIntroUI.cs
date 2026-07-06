using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UnitIntroUI : MonoBehaviour
{
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;
    [Space]
    [SerializeField] Image portrait;
    [SerializeField] Image backgroundPanel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] Image classImage;
    [SerializeField] Image classBackground;
    [SerializeField] TMP_Text classTypeText;


    public void SetUp(UnitData data)
    {
        backgroundPanel.color = data.classColorType[data.unitClass];
        portrait.sprite = data.portrait;

        RectTransform portraitRect = portrait.GetComponent<RectTransform>();
        portraitRect.sizeDelta = new Vector2(portraitWidth * data.portraitScaleFactor, portraitHeight * data.portraitScaleFactor);
        portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * data.portraitPosXFactor, (1 + portraitPosY) * data.portraitPosYFactor);
        nameText.text = data.unitName;
        levelText.text = "Lv." + data.cardLevel;
        dialogText.text = data.startDialogue;
        classImage.color = data.classColorType[data.unitClass];
        classBackground.color = data.classColorType[data.unitClass];
        classTypeText.text = data.classToKorean[data.unitClass] + "/" + data.TypeToKorean(data.type);

    }
}
