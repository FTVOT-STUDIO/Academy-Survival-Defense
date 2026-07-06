using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StateCardUIManager : MonoBehaviour
{
    public CardData[] cardDatas;

    public List<GameObject> cardList = new List<GameObject>();

    [SerializeField] LevelUpPanelManger levelUpPanelManger;
    [SerializeField] GridLayoutGroup cardsLayoutGroup;
    [SerializeField] ContentSizeFitter cardsFitter;

    void OnEnable()
    {
        if (cardList == null || cardList.Count == 0)
        {
            InitializeCard();
        }
    }

    void InitializeCard()
    {
        cardDatas = new CardData[GameManager.Inst.skillCardManager.cardDatas.Count];
        cardList = new List<GameObject>(GameManager.Inst.skillCardManager.cardDatas.Count);

        for (int i = 0; i < GameManager.Inst.skillCardManager.cardDatas.Count; i++)
        {
            cardDatas[i] = GameManager.Inst.skillCardManager.cardDatas[i];
        }

        for (int i = 0; i < GameManager.Inst.skillCardManager.cardDatas.Count; i++)
        {
            GameObject card = PoolManager.Inst.Get(PoolManager.PrefabType.StateCardUI, transform.position);
            card.transform.SetParent(transform);
            card.transform.localScale = Vector3.one;

            card.GetComponent<StateCardUI>().cardData = cardDatas[i];
            card.GetComponent<StateCardUI>().Init();
            cardList.Add(card);
        }

        cardList = cardList.OrderBy(c => c.GetComponent<StateCardUI>().cardData.cardClass).ThenBy(c => c.GetComponent<StateCardUI>().cardData.sephiraPathData.sephirahPath).ToList();

        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.SetSiblingIndex(i);
        }

    }

    public void CardInit()
    {
        if (cardList == null || cardList.Count == 0)
        {
            InitializeCard();

        }
        for (int i = 0; i < GameManager.Inst.skillCardManager.cardDatas.Count; i++)
        {
            cardList[i].GetComponent<StateCardUI>().Init();
        }
    }

}
