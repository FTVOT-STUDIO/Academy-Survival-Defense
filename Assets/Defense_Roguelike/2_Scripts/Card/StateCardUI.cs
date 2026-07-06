using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class StateCardUI : MonoBehaviour
{
    public CardData cardData;
    [SerializeField] int cardLevel;
    [SerializeField] Image icon;
    [SerializeField] Image cardBackground;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text categoryText;
    [SerializeField] TMP_Text sephiraNameText;
    [SerializeField] Image outlineImage;
    [SerializeField] GameObject lockImage;

    public void Init()
    {
        int curLevel = GameManager.Inst.skillCardManager.GetCardLevel(cardData);

        var amount = 0f;
        if (cardData.cardEffect is ICardLevelStatCalculator calculator)
        {
            amount = calculator.CalculateLevelStat(cardData);
        }
        else
        {
            amount = cardData.CalculateLevelValue(cardData.Amount);
        }
        cardLevel = curLevel;
        icon.sprite = cardData.sephiraPathData.mainIcon;
        cardBackground.color = cardData.CardClassToColor[cardData.cardClass];
        nameText.text = cardData.card_Name;
        sephiraNameText.text = cardData.sephiraPathData.sephiraName;
        descText.text = string.Format(cardData.Desc, amount);
        levelText.gameObject.SetActive(cardData.cardCategory == CardData.CardCategory.Upgrade);
        categoryText.text = cardData.CardCategoryToKorean[cardData.cardCategory];
        if (cardData.cardCategory == CardData.CardCategory.Upgrade)
        {
            if (cardLevel == cardData.maxLevel)
            {
                levelText.text = "Max";
            }
            else
            {
                levelText.text = "Lv." + cardLevel.ToString();
                levelText.color = Color.green;
            }
        }
        lockImage.SetActive(cardLevel == 0 ? true : false);
        outlineImage.color = cardLevel == 0 ? Color.black : new Color(0, 1, 0.8f);
    }

}
