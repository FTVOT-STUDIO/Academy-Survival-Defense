using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

[System.Serializable]
public class SkillCardDefinition
{
    [Header("# Main Info")]
    [TextArea]
    public string cardSummary;
    public CardData data;
    public ScriptableObject cardEffect;
    public CardData.CardClass cardClass;
    public CardData.CardCategory cardCategory;
    public CardData.CardVariety cardVariety;
    [Space]
    public SephiraPathData sephiraPathData;
    [Space]
    public string card_Name;
    [TextArea]
    public string Desc;

    [HideInInspector] public int level = 0;
    public int maxLevel = 1;
    public float Amount;


}
[System.Serializable]
public class SkillCardCollection
{
    public string collectionName;
    [HideInInspector] public CardData.CardVariety cardVariety;
    [Space]
    public List<SkillCardDefinition> skillCardDefinitions;
}

public class SkillCardManager : MonoBehaviour
{
    public List<SkillCardCollection> skillCardCollections;
    [HideInInspector] public List<CardController> skillCards;

    [HideInInspector] public int selectCardCount;
    [SerializeField] private GameObject skillCardPrefab;
    [SerializeField] private GameObject skillCardGroup;
    public Dictionary<CardData, int> cardTable = new Dictionary<CardData, int>();
    private Dictionary<CardData.CardClass, List<CardData>> cardCache;
    public List<CardData> cardDatas;
    public CardData[] datas;
    Dictionary<int, int[]> baseProbabilities = new Dictionary<int, int[]>()
    {
        { 0, new int[] { 60, 25, 10, 5, 0 } },
        { 1, new int[] { 45, 30, 18, 5, 2 } },
        { 2, new int[] { 35, 25, 25, 10, 5 } },
        { 3, new int[] { 25, 20, 30, 18, 7 } },
        { 4, new int[] { 18, 15, 30, 20, 17 } },
        { 5, new int[] { 12, 12, 30, 18, 28 } }
    };

    void Awake()
    {
        for (int i = 0; i < skillCardCollections.Count; i++)
        {
            for (int j = 0; j < skillCardCollections[i].skillCardDefinitions.Count; j++)
            {
                cardDatas.Add(skillCardCollections[i].skillCardDefinitions[j].data);
            }
        }

        cardCache = cardDatas
              .GroupBy(d => d.cardClass)
              .ToDictionary(g => g.Key, g => g.ToList());

        for (int i = 0; i < 3; i++)
        {
            var skillCard = Instantiate(skillCardPrefab, skillCardGroup.transform, false);
            skillCards.Add(skillCard.GetComponent<CardController>());
        }
    }

    void Start()
    {
        for (int i = 0; i < skillCards.Count; i++)
        {
            skillCards[i].GetComponent<Button>().onClick.AddListener(() => InGameUIManager.Inst.stateCardUIManager.CardInit());
        }

        for (int i = 0; i < cardDatas.Count; i++)
        {
            cardTable.TryAdd(cardDatas[i], 0);
        }
    }

    public void GetSkillCard(int count = 0)
    {
        selectCardCount += count;
        InGameUIManager.Inst.levelUpPanel.SetActive(true);
        InGameUIManager.Inst.skillCardCountText.text = "선택 가능한 카드 : " + selectCardCount + " 개";

        GameManager.Inst.Pause();

        FieldManager.Inst.ResetFieldState();

        AudioManager.Inst.PlaySfx(AudioManager.Sfx.cheers);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Special);
    }

    public void ShowSkillCard(int count)
    {
        CardData.CardClass[] cardClasses = new CardData.CardClass[]
        {
        CardData.CardClass.common,
        CardData.CardClass.rare,
        CardData.CardClass.epic,
        CardData.CardClass.legendary,
        CardData.CardClass.unique
        };

        int[] probabilities = GetAdjustedProbabilities(GameFlowManager.Inst.cardSpawnLevel, GameStatManager.Inst.CardSpawnProbabilityBonus);

        HashSet<CardData> selectedCards = new HashSet<CardData>();
        int totalWeight = probabilities.Sum();

        while (selectedCards.Count < count)
        {
            int rand = UnityEngine.Random.Range(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (rand < cumulative && cardCache[cardClasses[i]].Count > 0)
                {
                    CardData candidate = cardCache[cardClasses[i]][UnityEngine.Random.Range(0, cardCache[cardClasses[i]].Count)];

                    if (!selectedCards.Contains(candidate) &&
                        !(candidate.cardCategory == CardData.CardCategory.Upgrade && GetCardLevel(candidate) >= candidate.maxLevel))
                    {
                        selectedCards.Add(candidate);
                    }
                    break; // 뽑기 끝났으니 루프 종료
                }
            }
        }

        CardData[] randomCard = selectedCards.ToArray();
        for (int i = 0; i < randomCard.Length; i++)
        {
            float xPos = 0;

            if (count == 3)
            {
                RectTransform rect = skillCards[i].GetComponent<RectTransform>();
                xPos = (i - 1) * 360;
                rect.anchoredPosition = new Vector2(xPos, rect.anchoredPosition.y);
            }
            if (count == 2)
            {
                RectTransform rect = skillCards[i].GetComponent<RectTransform>();
                xPos = (i * 2 - 1) * 260;
                rect.anchoredPosition = new Vector2(xPos, rect.anchoredPosition.y);
            }

            skillCards[i].transform.DOLocalMove(new Vector3(xPos, 200, 0), 0.5f)
            .SetEase(Ease.InBack)
            .From(new Vector3(0, 200, 0))
            .SetUpdate(true);

            skillCards[i].cardData = randomCard[i];
            skillCards[i].Init();
            skillCards[i].gameObject.SetActive(true);
        }
    }

    private int[] GetAdjustedProbabilities(int level, int probabilityBonus)
    {
        int[] adjusted = new int[5];

        for (int i = 0; i < 5; i++)
        {
            if (i < 2)
                adjusted[i] = baseProbabilities[level][i] - probabilityBonus;
            else
                adjusted[i] = baseProbabilities[level][i] + probabilityBonus;
        }

        return adjusted;
    }

    public void LevelUpCard(CardData data)
    {
        if (cardTable.ContainsKey(data))
        {
            cardTable[data]++;
        }
    }

    public int GetCardLevel(CardData data)
    {
        if (cardTable.ContainsKey(data))
        {
            return cardTable[data];
        }
        return 0;
    }

    public void InitializeCardUI()
    {
        for (int i = 0; i < skillCards.Count; i++)
        {
            skillCards[i].GetComponent<Button>().interactable = false;
        }
    }

    public void HideAllCards()
    {
        for (int i = 0; i < skillCards.Count; i++)
        {
            skillCards[i].gameObject.SetActive(false);
        }
    }

    public void ActiveLevelUpWindow()
    {
        if (selectCardCount > 1)
        {
            InGameUIManager.Inst.levelUpPanel.SetActive(false);
            InGameUIManager.Inst.levelUpPanel.SetActive(true);
            selectCardCount--;
            InGameUIManager.Inst.skillCardCountText.text = "선택 가능한 카드 : " + selectCardCount + " 개";
        }
        else
        {
            selectCardCount = 0;
            GameManager.Inst.Resume();
            InGameUIManager.Inst.levelUpPanel.SetActive(false);
        }
    }

    [ContextMenu("카드 데이터 저장하기")]
    [Button]
    private void SaveCardTable()
    {
        Debug.Log("<color=#00BFFF>데이터 저장</color>");
        //EditorUtility.DisplayDialog("알림", "카드 데이터 저장", "확인");
        for (int i = 0; i < skillCardCollections.Count; i++)
        {
            for (int j = 0; j < skillCardCollections[i].skillCardDefinitions.Count; j++)
            {
                CardData originData = skillCardCollections[i].skillCardDefinitions[j].data;
                SkillCardDefinition skillCardDef = skillCardCollections[i].skillCardDefinitions[j];

                originData.cardSummary = skillCardDef.cardSummary;
                originData.cardEffect = skillCardDef.cardEffect;
                originData.cardClass = skillCardDef.cardClass;
                originData.cardCategory = skillCardDef.cardCategory;
                originData.cardVariety = skillCardDef.cardVariety;
                originData.sephiraPathData = skillCardDef.sephiraPathData;
                originData.card_Name = skillCardDef.card_Name;
                originData.Desc = skillCardDef.Desc;
                originData.maxLevel = skillCardDef.maxLevel;
                originData.Amount = skillCardDef.Amount;
            }
        }

    }

    [ContextMenu("카드 데이터 불러오기")]
    [Button]
    private void LoadCardTable()
    {
        Debug.Log("<color=green>데이터 불러옴</color>");
        //EditorUtility.DisplayDialog("알림", "카드 데이터 불러옴", "확인");
        if (skillCardCollections.Count > 0)
        {
            skillCardCollections.Clear();
        }

        CardData.CardVariety[] cardVarieties = (CardData.CardVariety[])Enum.GetValues(typeof(CardData.CardVariety));

        for (int i = 0; i < cardVarieties.Length; i++)
        {
            CardData.CardVariety cardVariety = cardVarieties[i];

            SkillCardCollection collection = new SkillCardCollection();
            collection.cardVariety = cardVariety;
            collection.collectionName = datas[0].varietyTable[cardVariety];
            collection.skillCardDefinitions = new List<SkillCardDefinition>();

            for (int j = 0; j < datas.Length; j++)
            {
                if (datas[j].cardVariety != cardVariety)
                {
                    continue;
                }

                SkillCardDefinition definition = new SkillCardDefinition();
                definition.data = datas[j];

                collection.skillCardDefinitions.Add(definition);
            }

            skillCardCollections.Add(collection);
        }

        for (int i = 0; i < skillCardCollections.Count; i++)
        {
            for (int j = 0; j < skillCardCollections[i].skillCardDefinitions.Count; j++)
            {
                CardData originData = skillCardCollections[i].skillCardDefinitions[j].data;
                SkillCardDefinition skillCardDef = skillCardCollections[i].skillCardDefinitions[j];

                skillCardDef.cardSummary = originData.cardSummary;
                skillCardDef.cardEffect = originData.cardEffect;
                skillCardDef.cardClass = originData.cardClass;
                skillCardDef.cardCategory = originData.cardCategory;
                skillCardDef.cardVariety = originData.cardVariety;
                skillCardDef.sephiraPathData = originData.sephiraPathData;
                skillCardDef.card_Name = originData.card_Name;
                skillCardDef.Desc = originData.Desc;
                skillCardDef.maxLevel = originData.maxLevel;
                skillCardDef.Amount = originData.Amount;
            }
        }
    }
}

