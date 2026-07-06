using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class RewardContainer : MonoBehaviour
{
    public List<Reward> rewardItems;
    [SerializeField] Scrollbar containerScrollBar;
    [SerializeField] HorizontalLayoutGroup rewardLayout;
    [SerializeField] ContentSizeFitter rewardFitter;

    void OnEnable()
    {
        GetComponent<PopUpUI>().OnShowShrink(gameObject);

        rewardItems = MergeDuplicateRewards(rewardItems);

        rewardItems = rewardItems
            .OrderBy(r => r.IsUnit ? 1 : 0)
            .ThenBy(r => r.IsUnit ? 0 : (int)r.itemType)
            .ThenBy(r => r.IsUnit ? r.unitData.unitClass : 0)
            .ToList();

        for (int i = 0; i < rewardItems.Count; i++)
        {
            RewardItem rewardItem = PoolManager.Inst
                .Get(PoolManager.PrefabType.RewardItem, transform.position)
                .GetComponent<RewardItem>();

            Reward reward = rewardItems[i];
            if (reward.IsUnit)
                rewardItem.Init(reward.unitData, reward.value);
            else
                rewardItem.Init(reward.itemType, reward.value);

            Transform itemTrans = rewardItem.transform;
            itemTrans.SetParent(RewardManager.Inst.rewardContent.transform, false);
            itemTrans.SetSiblingIndex(i);
        }

        StartCoroutine(SetScrollBarToBottomNextFrame());
        GameUIManager.Inst.RefreshLayoutOnce(rewardLayout);
        GameUIManager.Inst.RefreshFitterOnce(rewardFitter);

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Reward1);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        ClearRewards();
    }

    private List<Reward> MergeDuplicateRewards(List<Reward> rewards)
    {
        var mergedRewards = new List<Reward>();

        var unitGroups = rewards
            .Where(r => r.IsUnit)
            .GroupBy(r => r.unitData.unitID)
            .Select(g => new Reward
            {
                unitData = g.First().unitData,
                value = g.Sum(x => x.value)
            });

        var itemGroups = rewards
            .Where(r => !r.IsUnit)
            .GroupBy(r => r.itemType)
            .Select(g => new Reward
            {
                itemType = g.Key,
                value = g.Sum(x => x.value)
            });

        mergedRewards.AddRange(unitGroups);
        mergedRewards.AddRange(itemGroups);

        return mergedRewards;
    }

    private IEnumerator SetScrollBarToBottomNextFrame()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            RewardManager.Inst.rewardContent.GetComponent<RectTransform>()
        );

        Canvas.ForceUpdateCanvases();

        containerScrollBar.value = 0f;

        yield return new WaitForEndOfFrame();

        containerScrollBar.value = 0f;
    }

    public void ClearRewards()
    {
        Transform content = RewardManager.Inst.rewardContent.transform;

        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);

            ObjectLifecycleManager objectLifecycleManager = child.GetComponent<ObjectLifecycleManager>();
            objectLifecycleManager.Init(ObjectLifecycleManager.ObjectType.Object);
            objectLifecycleManager.DeActive();
        }
        rewardItems.Clear();

        LayoutRebuilder.ForceRebuildLayoutImmediate(RewardManager.Inst.rewardContent.GetComponent<RectTransform>());
    }
}




[System.Serializable]
public class Reward
{
    public UnitData unitData;
    public int value;
    public ResourcesData.ResourcesType itemType;

    public bool IsUnit => unitData != null;
}

