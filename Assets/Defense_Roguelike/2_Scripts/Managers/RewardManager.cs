using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Inst;
    public RewardContainer rewardContainer;

    public GameObject rewardContent;

    void Awake()
    {
        Inst = this;
    }

    public void AddReward(ResourcesData.ResourcesType itemType, int value, UnitData unitData = null)
    {
        Reward reward = new Reward { itemType = itemType, value = value, unitData = unitData };
        rewardContainer.rewardItems.Add(reward);
    }
}
