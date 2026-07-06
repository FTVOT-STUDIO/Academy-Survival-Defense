using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUpManager : MonoBehaviour
{
    [SerializeField] TMP_Text levelUpText;
    [SerializeField] TMP_Text levelUpGoldText;
    [SerializeField] TMP_Text levelUpGemText;
    [SerializeField] TMP_Text levelUpGoldMultiplier;
    [SerializeField] TMP_Text levelUpGemMultiplier;
    [SerializeField] Button backgroundCloseButton;

    void OnEnable()
    {
        StartCoroutine(UpdateLevelUpUI());
    }

    IEnumerator UpdateLevelUpUI()
    {
        backgroundCloseButton.interactable = false;

        levelUpText.text = "Lv." + PlayerManager.Inst.level;
        levelUpGoldText.text = (40000 * PlayerManager.Inst.levelUpAmount).ToString();
        levelUpGemText.text = (300 * PlayerManager.Inst.levelUpAmount).ToString();

        levelUpGoldMultiplier.text = "X" + PlayerManager.Inst.levelUpAmount;
        levelUpGemMultiplier.text = "X" + PlayerManager.Inst.levelUpAmount;

        yield return new WaitForSeconds(0.5f);

        backgroundCloseButton.interactable = true;
    }

    public void HideLevelUpPanel(GameObject gameObject)
    {
        PlayerManager.Inst.isLevelUp = false;
        PlayerManager.Inst.ModifyGold(40000 * PlayerManager.Inst.levelUpAmount, true);
        PlayerManager.Inst.ModifyGem(300 * PlayerManager.Inst.levelUpAmount, true);
        RewardManager.Inst.rewardContainer.gameObject.SetActive(true);
        DataManager.Inst.SaveData();
        gameObject.SetActive(false);
    }
}
