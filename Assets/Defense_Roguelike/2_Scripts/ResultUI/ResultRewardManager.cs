using UnityEngine;
using System.Collections;

public class ResultRewardManager : MonoBehaviour
{
    [SerializeField] Transform rewardListPerent;

    void OnEnable()
    {
        StartCoroutine(GetReward());
    }

    IEnumerator GetReward()
    {
        int curExp = 10 * GameManager.Inst.wave;
        int curGold = 100 * GameManager.Inst.wave;
        int curVoidTicket = 2 * GameManager.Inst.wave;

        PlayerManager.Inst.GetPlayerExp(curExp, true);

        PlayerManager.Inst.gold += curGold;
        PlayerManager.Inst.voidTicket += curVoidTicket;

        DataManager.Inst.SaveData();

        (ResourcesData.ResourcesType type, int value)[] rewards =
        {
            (ResourcesData.ResourcesType.gold, curGold),
            (ResourcesData.ResourcesType.Exp, curExp),
            (ResourcesData.ResourcesType.VoidTicket, curVoidTicket)
        };

        for (int i = 0; i < rewards.Length; i++)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            RewardItem item = PoolManager.Inst.Get(PoolManager.PrefabType.RewardItem, transform.position).GetComponent<RewardItem>();
            item.transform.SetParent(rewardListPerent);
            item.Init(rewards[i].type, rewards[i].value);
            item.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
            item.gameObject.SetActive(true);
        }

    }
}
