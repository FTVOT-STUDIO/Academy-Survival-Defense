using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Inst;

    [Tab("Player Info")]
    [Header("# Player Info")]
    [Space]
    public string playerName;
    [Space]
    public int level;
    public int levelUpAmount;
    public int[] levelExpTable;

    [Space]
    public int curLevelExp;
    public int maxLevelExp;
    public int maxLevel = 100;

    public bool isLevelUp;
    public bool isJointOperationUnlocked = false;
    public DifficultyData.DifficultyType difficultyType;
    [Tab("Resource")]
    public int gold;
    public TMP_Text goldText;
    [Space]
    public int gem;
    public TMP_Text gemText;
    [Space]
    public int voidTicket;
    [Space]
    [HideInInspector] public bool isTutorial;
    [HideInInspector] public string lastLoginTime;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text playerLevelText;
    [SerializeField] Slider playerLevelSlider;
    [Tab("Level System")]
    public PlayerLevelUpManager playerLevelUpManager;


    void Awake()
    {
        Inst = this;
    }

    public void InitializedPlayerInfo()
    {
        playerNameText.text = playerName;
        playerLevelText.text = "Lv. " + level.ToString();
        playerLevelSlider.value = (float)curLevelExp / maxLevelExp;
        goldText.text = string.Format("{0:n0}", gold);
        gemText.text = string.Format("{0:n0}", gem);
    }

    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }

    public void GetPlayerExp(int exp, bool registerReward)
    {
        int initialLevel = level;
        curLevelExp += exp;

        while (level < maxLevel && curLevelExp >= levelExpTable[level])
        {
            curLevelExp -= levelExpTable[level];
            level++;
        }

        if (level > initialLevel)
        {
            Debug.Log($"Level UP: {initialLevel} → {level}");
            levelUpAmount = level - initialLevel;
            isLevelUp = true;
        }
        else
            levelUpAmount = 1;

        if (registerReward)
            RewardManager.Inst.AddReward(ResourcesData.ResourcesType.Exp, exp);

        DataManager.Inst.SaveData();
    }

    public void ModifyGold(int amount, bool registerReward)
    {
        int previousGold = gold;
        gold += amount;

        if (gold < 0)
            gold = 0;

        // Debug.Log($"골드 변경됨: {amount} → 현재 골드: {gold}");

        NumberCounter.StartCounting(this, gold, previousGold, 0.5f, goldText);
        goldText.GetComponent<PopUpUI>().PopUpText(goldText.gameObject);

        goldText.text = string.Format("{0:n0}", gold);

        StartCoroutine(RefreshItemList());

        if (registerReward)
        {
            RewardManager.Inst.AddReward(ResourcesData.ResourcesType.gold, amount);
        }

        ShopManager.Inst.RefreshItemList();
        DataManager.Inst.SaveData();
    }

    public void ModifyGem(int amount, bool registerReward)
    {
        int previousGem = gem;
        gem += amount;

        if (gem < 0)
            gem = 0;

        //  Debug.Log($"보석 변경됨: {amount} → 현재 보석: {gem}");

        NumberCounter.StartCounting(this, gem, previousGem, 0.5f, gemText);
        gemText.GetComponent<PopUpUI>().PopUpText(gemText.gameObject);
        gemText.text = string.Format("{0:n0}", gem);

        if (registerReward)
        {
            RewardManager.Inst.AddReward(ResourcesData.ResourcesType.gem, amount);
        }

        ShopManager.Inst.RefreshItemList();
        DataManager.Inst.SaveData();
    }

    public void ModifyVoidTicket(int amount, bool registerReward)
    {
        voidTicket += amount;

        if (voidTicket < 0)
            voidTicket = 0;

        //   Debug.Log($"공허 통신 티켓 변경됨: {amount} → 현재 공허 통신 티켓: {voidTicket}");


        if (registerReward)
        {
            RewardManager.Inst.AddReward(ResourcesData.ResourcesType.VoidTicket, amount);
        }

        StartCoroutine(RefreshItemList());
        DataManager.Inst.SaveData();
    }

    public void ModifyUnit(UnitData data, int amount, bool registerReward)
    {
        data.curLevelExp += amount;

        if (registerReward)
        {
            RewardManager.Inst.AddReward(ResourcesData.ResourcesType.none, amount, data);
        }

        if (!data.isUnlock)
        {
            if (data.unitClass == UnitClass.Sephira)
            {
                EquipmentManager.Inst.UnlockSephira(data);
            }
            else
            {
                EquipmentManager.Inst.UnlockUnit(data);
            }
        }

        ShopManager.Inst.RefreshItemList();
        DataManager.Inst.SaveData();
    }

    public void UnlockField(FieldData data)
    {
        if (!data.isUnlock)
        {
            EquipmentManager.Inst.UnlockField(data);
        }

        DataManager.Inst.SaveData();
    }

    IEnumerator RefreshItemList()
    {
        yield return null;
        ShopManager.Inst.RefreshItemList();
    }

}
