using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardBoxButton : MonoBehaviour
{
    [SerializeField] int rewardCount;
    [SerializeField] TMP_Text rewardCountText;
    public Button rewardBoxButton;
    public GameObject rewardClaimedImage;
    [SerializeField] GameObject getRewardImage;
    public bool isQuestRewardClaimed;

    void Awake()
    {
        rewardCountText.text = "x" + rewardCount;
    }

    public void SetClaimState(bool isClaimed)
    {
        rewardBoxButton.interactable = isClaimed;
        getRewardImage.SetActive(isClaimed);
    }

    public void SetRewardClaimed()
    {
        isQuestRewardClaimed = true;
        rewardBoxButton.interactable = false;
        rewardClaimedImage.SetActive(true);
        getRewardImage.SetActive(false);
        DataManager.Inst.SaveData();
    }

    public void GetReward()
    {
        GachaManager.Inst.StartGacha(rewardCount);
    }
}
