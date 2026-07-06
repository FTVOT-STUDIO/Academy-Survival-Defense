using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardController : MonoBehaviour
{
    [HideInInspector] public CardData cardData;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text classText;
    [SerializeField] TMP_Text categoryText;
    [SerializeField] TMP_Text sephiraNameText;
    [SerializeField] GameObject scrollingPatternBackground;
    [SerializeField] Image shadow;
    [SerializeField] Image cardPanelBackground;
    [SerializeField] Image icon;

    void OnEnable()
    {
        scrollingPatternBackground.SetActive(true);
    }

    public void Init()
    {
        int curLevel = GameManager.Inst.skillCardManager.GetCardLevel(cardData);

        var amount = 0f;
        if (cardData.cardEffect is ICardBaseStatCalculator calculator)
        {
            amount = calculator.CalculateBaseStat(cardData);
        }
        else
        {
            amount = cardData.CalculateBaseValue(cardData.Amount);
        }
        icon.sprite = cardData.sephiraPathData.mainIcon;
        nameText.text = cardData.card_Name;
        descText.text = string.Format(cardData.Desc, amount);
        sephiraNameText.text = cardData.sephiraPathData.sephiraName;
        categoryText.text = cardData.CardCategoryToKorean[cardData.cardCategory];
        classText.text = cardData.CardClassToKorean[cardData.cardClass];
        classText.color = cardData.CardClassToColor[cardData.cardClass];

        cardPanelBackground.color = cardData.CardClassToColor[cardData.cardClass];
        shadow.color = cardData.CardClassToColor[cardData.cardClass];
        shadow.gameObject.SetActive(true);
        sephiraNameText.gameObject.SetActive(false);
        categoryText.gameObject.SetActive(false);

        levelText.gameObject.SetActive(cardData.cardCategory == CardData.CardCategory.Upgrade);

        if (cardData.cardCategory == CardData.CardCategory.Upgrade)
        {
            if (curLevel == cardData.maxLevel)
            {
                levelText.text = "Max";
            }
            else
            {
                levelText.text = "Lv." + curLevel.ToString();
            }
        }
    }

    public void OnSelectCard()
    {
        int curLevel = GameManager.Inst.skillCardManager.GetCardLevel(cardData);

        if (cardData.cardEffect is ICardEventRegister register && GameManager.Inst.skillCardManager.GetCardLevel(cardData) < 1)
        {
            register.Register();
        }

        if (cardData.cardCategory == CardData.CardCategory.InstantUse || curLevel != cardData.maxLevel)
        {
            cardData.ApplyCardEffect();
        }
        else
        {
            GameManager.Inst.ModifyDarkMatteCoin(1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Reward);
        }


        FieldManager.Inst.ResetFieldUnitLocate();
        GameManager.Inst.skillCardManager.LevelUpCard(cardData);

        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.GetSkillCard, 1);
        GameManager.Inst.skillCardManager.ActiveLevelUpWindow();
        GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, Color.white);

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SelectCard);
    }
}