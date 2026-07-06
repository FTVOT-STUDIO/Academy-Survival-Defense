using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionBonusUIContoroller : MonoBehaviour
{
    List<FactionBonusCalculator.FactionBonusInfo> factionBonusInfos = new List<FactionBonusCalculator.FactionBonusInfo>();

    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;

    public int curIndex;
    Color bonusColor = new(0.2f, 1f, 0.8f, 1f);
    [SerializeField] Color buttonActiveColor = new Color(0.6f, 0.6f, 0.9f, 1);
    [SerializeField] Color buttonDeActiveColor = new Color(0.3f, 0.3f, 0.6f, 1);
    public TMP_Text factionBonusText;

    private void Refresh()
    {
        if (factionBonusInfos.Count <= 0)
        {
            factionBonusText.text = "소속 보너스 없음";
            factionBonusText.color = Color.white;
            curIndex = 0;
            return;
        }

        FactionBonusCalculator.FactionBonusInfo currentInfo =
            factionBonusInfos[curIndex];

        factionBonusText.text = currentInfo.bonusText;
        factionBonusText.color = bonusColor;

        RefreshButtonState();
    }

    public void Init(List<FactionBonusCalculator.FactionBonusInfo> factionBonusInfos)
    {
        this.factionBonusInfos.Clear();
        this.factionBonusInfos = factionBonusInfos;
        Refresh();
    }

    public void ShowNext()
    {
        curIndex = Mathf.Min(factionBonusInfos.Count - 1, curIndex + 1);
        Refresh();
    }

    public void ShowPrevious()
    {
        curIndex = Mathf.Max(0, curIndex - 1);
        Refresh();
    }

    private void RefreshButtonState()
    {
        bool isFirstIndex = curIndex <= 0;
        bool isLastIndex = curIndex >= factionBonusInfos.Count - 1;

        SetButtonColor(previousButton, isFirstIndex);
        SetButtonColor(nextButton, isLastIndex);
    }

    private void SetButtonColor(Button button, bool isDisabled)
    {
        Image buttonImage = button.image;

        buttonImage.color = isDisabled
            ? buttonDeActiveColor
            : buttonActiveColor;
    }
}
