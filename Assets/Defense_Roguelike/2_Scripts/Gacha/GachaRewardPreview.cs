using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaRewardPreview : MonoBehaviour
{
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text gemText;
    [SerializeField] TMP_Text commonText;
    [SerializeField] TMP_Text rareText;
    [SerializeField] TMP_Text epicText;
    [SerializeField] TMP_Text legendaryText;
    [SerializeField] TMP_Text apostleText;
    [SerializeField] TMP_Text sephiraText;

    void OnEnable()
    {
        goldText.text = "x" + (GachaManager.Inst.rewardGold * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        gemText.text = "x" + (GachaManager.Inst.rewardGem * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();

        commonText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[0] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        rareText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[1] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        epicText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[2] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        legendaryText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[3] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        apostleText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[4] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        sephiraText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[5] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
    }
    void Update()
    {
        UpdateRewardPreviewByBoxCount();
    }
    public void UpdateRewardPreviewByBoxCount()
    {
        goldText.text = "x" + (GachaManager.Inst.rewardGold * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        gemText.text = "x" + (GachaManager.Inst.rewardGem * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();

        commonText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[0] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        rareText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[1] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        epicText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[2] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        legendaryText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[3] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        apostleText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[4] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
        sephiraText.text = "x" + (GachaManager.Inst.unitsPerGradeFromBox[5] * PlayerManager.Inst.level * GachaManager.Inst.boxOpenCount).ToString();
    }

}
