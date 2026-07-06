using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Inst;

    [SerializeField] ResourcesData goldData;
    [SerializeField] ResourcesData gemData;

    List<UnitData> unitDatas = new();
    List<UnitData> sephiraDatas = new();

    List<UnitData> common = new();
    List<UnitData> rare = new();
    List<UnitData> epic = new();
    List<UnitData> legendary = new();
    List<UnitData> apostle = new();
    List<UnitData> sephira = new();

    [SerializeField] GachaRewardItem GachaRewardItem;

    [HideInInspector] public int unitRewardIndex;
    int goodsRewardIndex;
    [SerializeField] GameObject gachaCanvas;
    public GameObject gachaRweardPanel;
    [SerializeField] GachaBox gachaBox;
    [SerializeField] GameObject gachaBoxParent;
    [SerializeField] Transform gachaRewardContentParent;
    [SerializeField] Scrollbar rewardListScrollBar;
    [SerializeField] Button gachaOpenButton;
    [SerializeField] Button gachaRetryButton;
    [SerializeField] Button gachaSkipButton;

    [SerializeField] TMP_Text remainingVoidTicketText;
    public GameObject rewardListContent;
    [HideInInspector] public bool isRewarding;
    [SerializeField] private bool isHighTierResult;
    [SerializeField] private bool isAnimationSkipped;
    int getGold;
    int getGem;
    [HideInInspector] public List<int> unitExpList = new List<int>();
    [Space]
    public int boxOpenCount;
    [HideInInspector] public List<UnitData> rewardList = new List<UnitData>();
    [HideInInspector] public List<UnitData> unlockNewUnitList = new List<UnitData>();
    [HideInInspector] public List<GameObject> rewardItemList = new List<GameObject>();

    [HideInInspector] public int[] unitsPerGradeFromBox = new int[] { 30, 20, 15, 10, 5, 20 };
    public int rewardGold = 500;
    public int rewardGem = 5;
    int totalRewardCount;
    [HideInInspector] public bool isSkip;

    List<(GachaRewardItem.ItemType type, ResourcesData resourcesData, int value)> goodsDataList = new List<(GachaRewardItem.ItemType type, ResourcesData resourcesData, int value)>();


    List<Coroutine> waitForAnimation = new List<Coroutine>();

    UnitData curData;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        unitDatas = DataManager.Inst.unitDatas;
        sephiraDatas = DataManager.Inst.sephiraUnitDatas;

        common = unitDatas.FindAll(unit => unit.unitClass == UnitClass.Common);
        rare = unitDatas.FindAll(unit => unit.unitClass == UnitClass.Rare);
        epic = unitDatas.FindAll(unit => unit.unitClass == UnitClass.Epic);
        legendary = unitDatas.FindAll(unit => unit.unitClass == UnitClass.Legendary);
        apostle = unitDatas.FindAll(unit => unit.unitClass == UnitClass.Apostle);
        sephira = sephiraDatas.FindAll(sephira => sephira.unitClass == UnitClass.Sephira);

    }

    void GenerateRewardList(int openCount)
    {
        unitRewardIndex = 0;
        goodsRewardIndex = 0;
        getGold = 0;
        getGem = 0;
        totalRewardCount = 0;
        unitExpList.Clear();
        rewardList.Clear();
        unlockNewUnitList.Clear();
        waitForAnimation.Clear();
        goodsDataList.Clear();

        getGold = Random.Range(rewardGold, rewardGold * PlayerManager.Inst.level) * openCount;
        getGem = Random.Range(0, rewardGem * PlayerManager.Inst.level) * openCount;

        goodsDataList =
        new List<(GachaRewardItem.ItemType type, ResourcesData resourcesData, int value)>
        {
        (GachaRewardItem.ItemType.gold, goldData, getGold),
        (GachaRewardItem.ItemType.gem, gemData, getGem)
        };

        PlayerManager.Inst.ModifyGold(getGold, true);
        PlayerManager.Inst.ModifyGem(getGem, true);

        goodsRewardIndex = 0;

        int[] probabilities = new int[] { 100, 80, 50, 10, 1, 5 };

        List<UnitData>[] unitLists = new List<UnitData>[] { common, rare, epic, legendary, apostle, sephira };

        for (int i = 0; i < openCount; i++)
        {
            int index = 0;
            while (index < probabilities.Length)
            {
                if (Random.Range(0, 100) < probabilities[index])
                {
                    if (unitLists[index].Count > 0)
                    {
                        UnitData chosenUnit = unitLists[index][Random.Range(0, unitLists[index].Count)];

                        if (!chosenUnit.isUnlock)
                        {
                            if (chosenUnit.unitClass == UnitClass.Sephira)
                                EquipmentManager.Inst.UnlockSephira(chosenUnit);
                            else
                                EquipmentManager.Inst.UnlockUnit(chosenUnit);


                            if (!unlockNewUnitList.Contains(chosenUnit))
                                unlockNewUnitList.Add(chosenUnit);

                        }

                        unitExpList.Add(unitsPerGradeFromBox[index] * PlayerManager.Inst.level * openCount);

                        PlayerManager.Inst.ModifyUnit(chosenUnit, unitExpList[rewardList.Count - unitRewardIndex], true);

                        probabilities[index] = (int)(probabilities[index] * 0.5f);

                        rewardList.Add(chosenUnit);

                        //  Debug.Log($"[{probabilities[index]}% 성공] 선택된 유닛: {chosenUnit.unitName} (등급: {index})");
                    }
                    else
                    {
                        // Debug.Log($"[등급 {index}] 유닛이 없습니다. 다음 등급으로 이동");
                        index++;
                    }
                }
                else
                {
                    //Debug.Log($"[{probabilities[index]}% 실패] 다음 등급으로 이동");
                    index++;
                }
            }
        }

        unitRewardIndex = rewardList.Count;

        totalRewardCount = goodsDataList.Count + unitRewardIndex;

        var sortedPairs = rewardList.Select((unit, index) => new { Unit = unit, Exp = unitExpList[index] }).
        OrderBy(pair => pair.Unit.unitClass).ToList();

        rewardList = sortedPairs.Select(pair => pair.Unit).ToList();
        unitExpList = sortedPairs.Select(pair => pair.Exp).ToList();

        unlockNewUnitList?.OrderBy(u => u.unitClass);


    }

    public void OpenVoidBox()
    {
        if (PlayerManager.Inst.voidTicket < 30 * boxOpenCount)
            return;
        gachaOpenButton.interactable = false;
        PlayerManager.Inst.ModifyVoidTicket(-30 * boxOpenCount, false);
        StartGacha(boxOpenCount);
        gachaRetryButton.gameObject.SetActive(true);

    }

    public void StartGacha(int openCount)
    {
        gachaBox.boxTabBtn.interactable = false;

        if (gachaBoxParent.activeSelf)
        {
            Debug.Log("!!!!!!!");
            gachaBox.StartGachaSFX();
            gachaBox.boxParticle.SetActive(false);
            gachaBox.anim.SetTrigger("Appear");
        }
        else
        {
            gachaBoxParent.SetActive(true);
        }

        gachaBox.StartGachaSFX();

        isSkip = false;
        isHighTierResult = false;
        isAnimationSkipped = false;
        GameUIManager.Inst.ShowFleshImage(gachaCanvas.transform, Color.purple);
        gachaCanvas.SetActive(true);
        GenerateRewardList(openCount);
        gachaBox.rewardIndexText.text = totalRewardCount.ToString();
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.OpenBox, 1);
        gachaRetryButton.gameObject.SetActive(false);
        StartCoroutine(SkipButtonDelayCoroutine());
        DataManager.Inst.SaveData();
    }

    IEnumerator SkipButtonDelayCoroutine()
    {
        gachaSkipButton.interactable = false;
        gachaSkipButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        gachaSkipButton.gameObject.SetActive(true);
        gachaSkipButton.interactable = true;
    }

    public void TabBox()
    {
        if (gachaRweardPanel.activeSelf)
        {
            gachaBox.rewardIndexText.text = 0.ToString();
            GachaRewardItem.gameObject.SetActive(false);
            return;
        }
        if (isHighTierResult)
        {
            OnTapSkip();
            isHighTierResult = false;
            return;
        }
        totalRewardCount--;
        gachaBox.rewardIndexText.text = Mathf.Max(0, totalRewardCount).ToString();

        GachaRewardItem.gameObject.SetActive(false);
        gachaBox.boxParticle.SetActive(false);

        if (goodsRewardIndex < goodsDataList.Count)
        {
            goodsRewardIndex++;
            GachaRewardItem.resourcesData = goodsDataList[goodsRewardIndex - 1].resourcesData;
            GachaRewardItem.Init(goodsDataList[goodsRewardIndex - 1].type);
            GachaRewardItem.itemValue.text = "x" + goodsDataList[goodsRewardIndex - 1].value;

            GachaRewardItem.gameObject.SetActive(true);
            gachaBox.boxParticle.SetActive(true);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.GetItem);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.select8);
            if (goodsRewardIndex != 1)
            {
                gachaBox.anim.SetTrigger("Tab");
            }
            return;
        }
        if (unitRewardIndex > 0)
        {
            curData = rewardList[rewardList.Count - unitRewardIndex];
            GachaRewardItem.unitdata = curData;

            GachaRewardItem.itemValue.text = "x" + unitExpList[rewardList.Count - unitRewardIndex].ToString();

            unitRewardIndex--;

            GachaRewardItem.Init(GachaRewardItem.ItemType.unit);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.GetItem);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.select8);

            switch (curData.unitClass)
            {
                case UnitClass.Common:
                case UnitClass.Rare:
                    GachaRewardItem.gameObject.SetActive(true);
                    gachaBox.specialUnitEffect.SetActive(false);
                    break;
                case UnitClass.Epic:
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn2);
                    GachaRewardItem.gameObject.SetActive(true);
                    gachaBox.specialUnitEffect.SetActive(false);
                    break;

                case UnitClass.Legendary:
                    waitForAnimation.Add(StartCoroutine(WaitForAnimation()));
                    gachaBox.specialPortrait.SetActive(true);
                    gachaBox.specialPortrait.GetComponentInChildren<ParticleColorChanger>().ChangeParticleColor(Color.yellow);

                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn2);
                    break;
                case UnitClass.Apostle:
                case UnitClass.Sephira:
                    waitForAnimation.Add(StartCoroutine(WaitForAnimation()));
                    gachaBox.specialPortrait.SetActive(true);

                    Color color = (curData.unitClass == UnitClass.Apostle) ? new Color(0.5f, 0, 1, 1) : curData.classColorType[curData.unitClass];
                    gachaBox.specialPortrait.GetComponentInChildren<ParticleColorChanger>().ChangeParticleColor(color);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn2);
                    break;
            }

            gachaBox.boxParticle.SetActive(true);

            switch (curData.unitClass)
            {
                case UnitClass.Epic:
                case UnitClass.Legendary:
                case UnitClass.Apostle:
                case UnitClass.Sephira:
                    Color color = (curData.unitClass == UnitClass.Epic) ? curData.classColorType[curData.unitClass] : Color.white;
                    GameUIManager.Inst.ShowFleshImage(gachaCanvas.transform, color, 0.5f);
                    break;
            }
            gachaBox.anim.SetTrigger("Tab");
            return;
        }
        ShowGachaRewards();
    }


    public void SkipGacha()
    {
        unitRewardIndex = rewardList.Count;
        gachaBox.anim.SetTrigger("Tab");
        isSkip = true;
        GachaRewardItem.gameObject.SetActive(false);

        ShowGachaRewards();
    }

    public void RetryGacha()
    {
        HideGachaCanvas();
        OpenVoidBox();
    }

    public void ExitGacha()
    {
        HideGachaCanvas();
        gachaBoxParent.gameObject.SetActive(false);
        RewardManager.Inst.rewardContainer.gameObject.SetActive(true);
    }

    public void ShowGachaRewards()
    {
        gachaRweardPanel.SetActive(true);

        if (waitForAnimation != null)
        {
            foreach (Coroutine coroutine in waitForAnimation)
                StopCoroutine(coroutine);
        }

        gachaBox.specialPortrait.SetActive(false);
        gachaBox.rewardIndexText.text = 0.ToString();
        StartCoroutine(GetReward());

        if (PlayerManager.Inst.voidTicket < 30 * boxOpenCount)
            remainingVoidTicketText.color = Color.red;
        else
            remainingVoidTicketText.color = Color.white;
        remainingVoidTicketText.text = "한 번 더\n" + 30 * boxOpenCount + "/" + PlayerManager.Inst.voidTicket;
    }

    public void HideGachaCanvas()
    {
        gachaCanvas.SetActive(false);
        gachaRweardPanel.SetActive(false);

        foreach (GameObject item in rewardItemList)
        {
            item.SetActive(false);
        }

        foreach (GameObject item in EquipmentManager.Inst.unlockUnitTabItems)
        {
            UnitTabUIInitializer unitTab = item.GetComponent<UnitTabUIInitializer>();
            unitTab.Init(unitTab.data, UnitTabUIInitializer.TabType.portrait, UnitTabUIInitializer.InfoType.readOnly);
        }

        foreach (GameObject item in EquipmentManager.Inst.unlockSephiraTabItems)
        {
            UnitTabUIInitializer sephiraTab = item.GetComponent<UnitTabUIInitializer>();
            sephiraTab.Init(sephiraTab.data, UnitTabUIInitializer.TabType.portrait, UnitTabUIInitializer.InfoType.readOnly);
        }

        rewardItemList.Clear();

        StopCoroutine(GetReward());

        gachaOpenButton.interactable = true;

        DataManager.Inst.SaveData();
    }

    IEnumerator WaitForAnimation()
    {
        isHighTierResult = true;
        isAnimationSkipped = false;
        Color color = (curData.unitClass == UnitClass.Apostle) ? new Color(0.5f, 0, 1, 1) : curData.classColorType[curData.unitClass];

        gachaBox.specialUnitEffect.SetActive(true);

        AudioManager.Inst.StopAllSfx();
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GetItem);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.aura);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.aura1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.KO);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.OnWaveChanged);

        float timer = 0f;
        float duration = 3.2f;

        while (timer < duration)
        {
            if (isAnimationSkipped)
            {
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        ApplySpecialGachaResult();
    }


    private void OnTapSkip()
    {
        AudioManager.Inst.StopAllSfx();
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GetItem);
        isAnimationSkipped = true;
        ApplySpecialGachaResult();
    }
    private void ApplySpecialGachaResult()
    {
        Color color = (curData.unitClass == UnitClass.Apostle) ? new Color(0.5f, 0, 1, 1) : curData.classColorType[curData.unitClass];
        switch (curData.unitClass)
        {
            case UnitClass.Legendary:
            case UnitClass.Apostle:
            case UnitClass.Sephira:
                GameUIManager.Inst.ShowFleshImage(gachaCanvas.transform, color, 0.5f);
                break;
        }
        // gachaBox.boxTabBtn.interactable = true;
        isHighTierResult = false;
        gachaBox.specialPortrait.SetActive(false);
        GachaRewardItem.gameObject.SetActive(true);
        gachaBox.specialUnitEffect.GetComponent<ParticleColorChanger>().ChangeParticleColor(color);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Special);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Girl);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScreenChange0);
    }

    IEnumerator GetReward()
    {
        AudioManager.Inst.StopAllSfx();

        isRewarding = true;

        StartCoroutine(AutoScrollRewardList());

        List<(GachaRewardItem.ItemType type, ResourcesData resourcesData, int value)> copyGoodsData = goodsDataList;

        foreach (var data in copyGoodsData)
        {
            if (!gachaRewardContentParent.gameObject.activeInHierarchy)
                break;

            GachaRewardItem goodsItem = PoolManager.Inst.Get(PoolManager.PrefabType.GachaRewardItem, transform.position).GetComponentInChildren<GachaRewardItem>();
            goodsItem.transform.parent.SetParent(gachaRewardContentParent, false);
            goodsItem.resourcesData = data.resourcesData;
            goodsItem.Init(data.type);
            goodsItem.itemValue.text = "x" + data.value;
            rewardItemList.Add(goodsItem.transform.parent.gameObject);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.select7);
            yield return new WaitForSeconds(0.1f);

        }

        for (int i = 0; i < rewardList.Count; i++)
        {
            if (!gachaRewardContentParent.gameObject.activeInHierarchy)
                break;

            GachaRewardItem goodsItem = PoolManager.Inst.Get(PoolManager.PrefabType.GachaRewardItem, transform.position).GetComponentInChildren<GachaRewardItem>();
            goodsItem.transform.parent.SetParent(gachaRewardContentParent, false);
            goodsItem.unitdata = rewardList[i];
            goodsItem.Init(GachaRewardItem.ItemType.unit);
            goodsItem.itemValue.text = "x" + unitExpList[i].ToString();
            rewardItemList.Add(goodsItem.transform.parent.gameObject);

            switch (rewardList[i].unitClass)
            {
                case UnitClass.Common:
                case UnitClass.Rare:
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.select7);
                    break;
                case UnitClass.Epic:
                    GameUIManager.Inst.ShowFleshImage(gachaCanvas.transform, rewardList[i].classColorType[rewardList[i].unitClass], 0.5f);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.select6);
                    break;
                case UnitClass.Legendary:
                case UnitClass.Sephira:
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.KO);
                    GameUIManager.Inst.ShowFleshImage(gachaCanvas.transform, rewardList[i].classColorType[rewardList[i].unitClass], 0.3f);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.select6);
                    break;
                case UnitClass.Apostle:
                    GameUIManager.Inst.ShowFleshImage(gachaCanvas.transform, Color.cyan, 0.5f);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.select6);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.KO);
                    break;

            }


            yield return new WaitForSeconds(0.1f);
        }

        isRewarding = false;
    }

    IEnumerator AutoScrollRewardList()
    {
        while (isRewarding)
        {
            rewardListScrollBar.value = Mathf.Lerp(rewardListScrollBar.value, 0, 0.5f);
            yield return null;
        }
    }
}
