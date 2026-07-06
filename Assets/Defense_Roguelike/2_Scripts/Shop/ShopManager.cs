using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VInspector;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Inst;
    [Tab("Core")]
    public ShopItem[] shopItems;
    public UnitData[] currentShopUnits = new UnitData[5];
    public ShopResourcesData[] currentShopResources = new ShopResourcesData[5];
    [SerializeField] GameObject itemSelectPanel;
    [SerializeField] Button itemBuyButton;
    [SerializeField] Button itemInfoButton;
    UnityAction currentBuyAction;
    UnityAction resourcesBuyAction;
    [SerializeField] Button itemRefreshButton;
    [SerializeField] TMP_Text itemRefreshCostText;
    [SerializeField] TMP_Text itemRefreshCountText;
    [SerializeField] GameObject itemRefreshLockImage;
    [SerializeField] Transform shopItemGroup;
    [SerializeField] GameObject resourcesBuyPanel;
    [SerializeField] Button resourcesBuyButton;
    [HideInInspector] public int refreshCount;
    [SerializeField] ShopResourcesData freeGoldData;
    [SerializeField] ShopResourcesData freeGemData;
    [SerializeField] ShopResourcesData freeVoidTicketData;
    [Tab("Gold Shop")]
    [Header("# Gold Shop")]
    public ShopResourcesData[] goldShopData;
    public ShopResourcesItem[] goldShopItems;
    [Tab("VoidTicket Shop")]
    [Header("# VoidTIcket Shop")]
    public ShopResourcesData[] voidTickethopData;
    public ShopResourcesItem[] voidTicketShopItems;
    public ShopResourcesData[] randomShopVoidTicketDatas;
    [Tab("Gem Shop")]
    [Header("# Gem Shop")]
    public ShopResourcesData[] gemhopData;
    public ShopResourcesItem[] gemShopItems;

    private UnityAction openUnitTabInfoAction;


    void Awake()
    {
        Inst = this;
        openUnitTabInfoAction = () =>
        {
            OpenUnitTabInfo();
            itemInfoButton.GetComponent<PopUpUI>().OnShowShrink(EquipmentManager.Inst.unitInfoWindow);
        };

        currentShopUnits = new UnitData[shopItems.Length - 1];
        currentShopResources = new ShopResourcesData[shopItems.Length - 1];
    }

    void Start()
    {
        for (int i = 0; i < goldShopItems.Length; i++)
        {
            int index = i;

            goldShopItems[index].shopResourcesData = goldShopData[index];
            goldShopItems[index].Init();
            goldShopItems[index].gameObject.GetComponent<Button>().onClick.AddListener(() => OnShopResourceClicked(goldShopData[index]));

            voidTicketShopItems[index].shopResourcesData = voidTickethopData[index];
            voidTicketShopItems[index].Init();
            voidTicketShopItems[index].gameObject.GetComponent<Button>().onClick.AddListener(() => OnShopResourceClicked(voidTickethopData[index]));

            gemShopItems[index].shopResourcesData = gemhopData[index];
            gemShopItems[index].Init();
            gemShopItems[index].gameObject.GetComponent<Button>().onClick.AddListener(() => OnShopResourceClicked(gemhopData[index]));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateShopItems();
        }
    }

    public void GenerateShopItems()
    {
        for (int i = 0; i < shopItems.Length - 1; i++)
        {
            currentShopUnits[i] = null;
            currentShopResources[i] = null;
        }

        List<UnitData> unitDatas = DataManager.Inst.unitDatas;
        List<UnitData> sephiraDatas = DataManager.Inst.sephiraUnitDatas;

        Dictionary<UnitClass, List<UnitData>> unitGroups = new Dictionary<UnitClass, List<UnitData>>
        {
               { UnitClass.Common, unitDatas.FindAll(u => u.unitClass == UnitClass.Common) },
                   { UnitClass.Rare, unitDatas.FindAll(u => u.unitClass == UnitClass.Rare) },
                      { UnitClass.Epic, unitDatas.FindAll(u => u.unitClass == UnitClass.Epic) },
                         { UnitClass.Legendary, unitDatas.FindAll(u => u.unitClass == UnitClass.Legendary) },
                            { UnitClass.Apostle, unitDatas.FindAll(u => u.unitClass == UnitClass.Apostle) },
                               { UnitClass.Sephira, sephiraDatas.FindAll(u => u.unitClass == UnitClass.Sephira) },
        };

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].unitData = null;
            shopItems[i].shopResourcesData = null;
        }

        float rand = Random.value;

        if (rand <= 0.55f)
        {
            shopItems[0].shopResourcesData = freeGoldData;
            shopItems[0].Init(ShopItem.ItemType.gold);
        }
        else if (rand <= 0.85f)
        {
            shopItems[0].shopResourcesData = freeGemData;
            shopItems[0].Init(ShopItem.ItemType.gem);
        }
        else
        {
            shopItems[0].shopResourcesData = freeVoidTicketData;
            shopItems[0].Init(ShopItem.ItemType.voidTicket);
        }

        for (int i = 1; i <= 3; i++)
        {
            float randValue = Random.value;

            if (randValue <= 0.3f)
            {
                shopItems[i].shopResourcesData = randomShopVoidTicketDatas[Random.Range(0, randomShopVoidTicketDatas.Length)];
                shopItems[i].Init(ShopItem.ItemType.voidTicket);
                currentShopResources[i - 1] = shopItems[i].shopResourcesData;
            }
            else
            {
                if (i == 3)
                    SetRandomUnit(i, shopItems[i], unitGroups[UnitClass.Rare]);
                else
                    SetRandomUnit(i, shopItems[i], unitGroups[UnitClass.Common]);
            }
        }

        SetRandomUnit(4, shopItems[4], unitGroups[UnitClass.Epic]);

        if (Random.value <= 0.25f)
        {
            List<UnitData> selectedList = null;

            if (Random.value <= 0.7f)
                selectedList = unitGroups[UnitClass.Legendary];
            else if (Random.value <= 0.95f)
                selectedList = unitGroups[UnitClass.Sephira];
            else
                selectedList = unitGroups[UnitClass.Apostle];

            SetRandomUnit(5, shopItems[5], selectedList);
        }
        else
        {
            SetRandomUnit(5, shopItems[5], unitGroups[UnitClass.Epic]);
        }

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].isPurchased = false;
            shopItems[i].itemID = i;
            shopItems[i].Init(shopItems[i].itemType);
        }
    }

    public void OnClickRefreshItem()
    {
        if (refreshCount < 4)
        {
            int currentCost = refreshCount * 1000;

            if (PlayerManager.Inst.gold >= currentCost)
            {
                refreshCount++;
                PlayerManager.Inst.ModifyGold(-currentCost, false);

                int nextCost = refreshCount * 1000;
                itemRefreshCostText.text = refreshCount < 4 ? nextCost.ToString() : (1000 * refreshCount).ToString();
                itemRefreshCostText.color = PlayerManager.Inst.gold < nextCost ? Color.red : Color.white;

                itemRefreshCountText.text = refreshCount + "/" + 4;


                itemRefreshLockImage.SetActive(refreshCount >= 4 || PlayerManager.Inst.gold < nextCost);
                itemRefreshButton.interactable = refreshCount < 4 && PlayerManager.Inst.gold >= nextCost;

                GenerateShopItems();

                DataManager.Inst.SaveData();
            }
        }

    }

    public void RefreshItemList()
    {
        shopItems[0].Init(shopItems[0].itemType);

        for (int i = 1; i < shopItems.Length; i++)
        {
            if (shopItems[i].itemType == ShopItem.ItemType.unit)
                shopItems[i].Init(ShopItem.ItemType.unit);
            else if (shopItems[i].itemType == ShopItem.ItemType.voidTicket)
                shopItems[i].Init(ShopItem.ItemType.voidTicket);
        }

        itemRefreshCostText.text = refreshCount == 0 ? "무료" : (refreshCount * 1000).ToString();
        itemRefreshCostText.color = PlayerManager.Inst.gold < refreshCount * 1000 ? Color.red : Color.white;

        itemRefreshCountText.text = refreshCount + "/" + 4;

        itemRefreshLockImage.SetActive(refreshCount > 3 || PlayerManager.Inst.gold < refreshCount * 1000);
        itemRefreshButton.interactable = refreshCount <= 3 && PlayerManager.Inst.gold >= refreshCount * 100;

        for (int i = 0; i < goldShopItems.Length; i++)
        {
            goldShopItems[i].priceText.color = (PlayerManager.Inst.gem < goldShopItems[i].shopResourcesData.price) ? Color.red : Color.white;
            voidTicketShopItems[i].priceText.color = (PlayerManager.Inst.gem < voidTicketShopItems[i].shopResourcesData.price) ? Color.red : Color.white;

            goldShopItems[i].GetComponent<Button>().interactable = !(PlayerManager.Inst.gem < goldShopItems[i].shopResourcesData.price);
            voidTicketShopItems[i].GetComponent<Button>().interactable = !(PlayerManager.Inst.gem < voidTicketShopItems[i].shopResourcesData.price);
        }
    }

    void SetRandomUnit(int index, ShopItem item, List<UnitData> datas)
    {
        UnitData randomUnit = datas[Random.Range(0, datas.Count)];
        item.unitData = randomUnit;
        currentShopUnits[index - 1] = randomUnit;
        item.Init(ShopItem.ItemType.unit);
    }

    public void GetFreeReward()
    {
        shopItems[0].isPurchased = true;
        shopItems[0].Init(shopItems[0].itemType);

        if (shopItems[0].itemType == ShopItem.ItemType.gold)
            PlayerManager.Inst.ModifyGold(shopItems[0].itemGetCount, true);
        else if (shopItems[0].itemType == ShopItem.ItemType.gem)
            PlayerManager.Inst.ModifyGem(shopItems[0].itemGetCount, true);
        else
            PlayerManager.Inst.ModifyVoidTicket(shopItems[0].itemGetCount, true);


        ItemBuyEffect(shopItems[0].transform, shopItemGroup.transform, shopItems[0].portrait.sprite);

        RefreshItemList();
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.ShopBuyItem, 1);
        DataManager.Inst.SaveData();

        RewardManager.Inst.rewardContainer.gameObject.SetActive(true);

    }

    public void OpenBuyPanel(int index)
    {
        if (openUnitTabInfoAction != null)
            itemInfoButton.onClick.RemoveListener(openUnitTabInfoAction);

        itemBuyButton.interactable = true;

        if (shopItems[index + 1].itemType == ShopItem.ItemType.unit)
        {
            itemSelectPanel.GetComponentInChildren<ShopItem>().unitData = currentShopUnits[index];
            itemSelectPanel.GetComponentInChildren<ShopItem>().Init(ShopItem.ItemType.unit);
            itemInfoButton.onClick.RemoveListener(openUnitTabInfoAction);
            itemInfoButton.onClick.AddListener(openUnitTabInfoAction);
        }
        else if (shopItems[index + 1].itemType == ShopItem.ItemType.voidTicket)
        {
            itemSelectPanel.GetComponentInChildren<ShopItem>().shopResourcesData = currentShopResources[index];
            itemSelectPanel.GetComponentInChildren<ShopItem>().Init(ShopItem.ItemType.voidTicket);
        }

        if (currentBuyAction != null)
            itemBuyButton.onClick.RemoveListener(currentBuyAction);

        currentBuyAction = () => BuySelectedItem(index);
        itemBuyButton.onClick.AddListener(currentBuyAction);
    }

    public void BuySelectedItem(int index)
    {
        if (shopItems[index + 1].itemType == ShopItem.ItemType.unit)
        {
            if (shopItems[index + 1].unitData.buyResourcesData.resourcesType == ResourcesData.ResourcesType.gold)
            {
                if (PlayerManager.Inst.gold < shopItems[index + 1].price)
                {
                    itemBuyButton.onClick.RemoveListener(currentBuyAction);
                    return;
                }
            }
            else
            {
                if (PlayerManager.Inst.gem < shopItems[index + 1].price)
                {
                    itemBuyButton.onClick.RemoveListener(currentBuyAction);
                    return;
                }
            }
        }
        else if (shopItems[index + 1].itemType == ShopItem.ItemType.voidTicket)
        {
            if (PlayerManager.Inst.gold < shopItems[index + 1].price)
            {
                itemBuyButton.onClick.RemoveListener(currentBuyAction);
                return;
            }

        }

        itemBuyButton.interactable = false;
        shopItems[index + 1].isPurchased = true;

        if (shopItems[index + 1].itemType == ShopItem.ItemType.unit)
        {
            shopItems[index + 1].Init(ShopItem.ItemType.unit);
            PlayerManager.Inst.ModifyUnit(currentShopUnits[index], shopItems[index + 1].itemGetCount, true);

            if (shopItems[index + 1].unitData.buyResourcesData.resourcesType == ResourcesData.ResourcesType.gold)
                PlayerManager.Inst.ModifyGold(-shopItems[index + 1].price, false);
            else
                PlayerManager.Inst.ModifyGem(-shopItems[index + 1].price, false);
        }
        else if (shopItems[index + 1].itemType == ShopItem.ItemType.voidTicket)
        {
            shopItems[index + 1].Init(ShopItem.ItemType.voidTicket);
            PlayerManager.Inst.ModifyVoidTicket(shopItems[index + 1].itemGetCount, true);

            if (shopItems[index + 1].shopResourcesData.currencyType == ShopResourcesData.CurrencyType.gold)
                PlayerManager.Inst.ModifyGold(-shopItems[index + 1].price, false);
            else
                PlayerManager.Inst.ModifyGem(-shopItems[index + 1].price, false);
        }

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.BuyItem);

        GameUIManager.Inst.ShowFleshImage(LobbyManager.Inst.lobbyUiCanvas.transform, Color.white);

        itemBuyButton.onClick.RemoveListener(currentBuyAction);
        RewardManager.Inst.rewardContainer.gameObject.SetActive(true);

        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.ShopBuyItem, 1);
        DataManager.Inst.SaveData();
    }

    public void OpenUnitTabInfo()
    {
        EquipmentManager.Inst.unitInfoWindow.GetComponent<UnitTabUIInitializer>().Init(itemSelectPanel.GetComponentInChildren<ShopItem>().unitData, UnitTabUIInitializer.TabType.info, UnitTabUIInitializer.InfoType.Shop);
    }

    public void OnShopResourceClicked(ShopResourcesData resourcesData)
    {

        resourcesBuyButton.interactable = true;
        resourcesBuyPanel.GetComponentInChildren<ShopResourcesItem>().shopResourcesData = resourcesData;
        resourcesBuyPanel.GetComponentInChildren<ShopResourcesItem>().Init();

        if (resourcesBuyAction != null)
            resourcesBuyButton.onClick.RemoveListener(resourcesBuyAction);

        resourcesBuyAction = () => BuyResourcesItem(resourcesData);
        resourcesBuyButton.onClick.AddListener(resourcesBuyAction);

    }

    public void BuyResourcesItem(ShopResourcesData resourcesData)
    {
        switch (resourcesData.currencyType)
        {
            case ShopResourcesData.CurrencyType.gold:
                if (PlayerManager.Inst.gold < resourcesData.price)
                    return;
                PlayerManager.Inst.ModifyGold(-resourcesData.price, false);
                break;
            case ShopResourcesData.CurrencyType.gem:
                if (PlayerManager.Inst.gem < resourcesData.price)
                    return;
                PlayerManager.Inst.ModifyGem(-resourcesData.price, false);
                break;
        }

        Transform itemPos = null;
        resourcesBuyButton.interactable = false;
        switch (resourcesData.itemType)
        {
            case ResourcesData.ResourcesType.gold:
                PlayerManager.Inst.ModifyGold(resourcesData.itemCount, true);
                for (int i = 0; i < goldShopItems.Length; i++)
                {
                    if (goldShopItems[i].shopResourcesData == resourcesData)
                    {
                        itemPos = goldShopItems[i].transform;
                        break;
                    }
                }
                break;
            case ResourcesData.ResourcesType.VoidTicket:
                PlayerManager.Inst.ModifyVoidTicket(resourcesData.itemCount, true);
                for (int i = 0; i < voidTicketShopItems.Length; i++)
                {
                    if (voidTicketShopItems[i].shopResourcesData == resourcesData)
                    {
                        itemPos = voidTicketShopItems[i].transform;
                        break;
                    }
                }
                break;
            case ResourcesData.ResourcesType.gem:
                PlayerManager.Inst.ModifyGem(resourcesData.itemCount, true);
                for (int i = 0; i < gemShopItems.Length; i++)
                {
                    if (gemShopItems[i].shopResourcesData == resourcesData)
                    {
                        itemPos = gemShopItems[i].transform;
                        break;
                    }
                }
                break;
        }

        RewardManager.Inst.rewardContainer.gameObject.SetActive(true);
        QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.ShopBuyItem, 1);

        ItemBuyEffect(itemPos, itemPos, resourcesData.resourcesData.icon);

    }


    void ItemBuyEffect(Transform generatePos, Transform parentPos, Sprite portrait)
    {
        ParticleSystem itemGetParticle = EffectManager.Inst.Get(EffectManager.PrefabType.ShopItemGetEffect, generatePos.position).GetComponent<ParticleSystem>();
        itemGetParticle.transform.SetParent(parentPos, false);
        itemGetParticle.transform.position = generatePos.position;
        itemGetParticle.textureSheetAnimation.RemoveSprite(0);
        itemGetParticle.textureSheetAnimation.AddSprite(portrait);
        itemGetParticle.GetComponent<ObjectLifecycleManager>().SetDelay(2);

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.BuyItem);
    }
}
