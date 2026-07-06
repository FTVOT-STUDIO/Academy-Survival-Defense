using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Inst;
    [Space]
    [SerializeField] ScrollRect questListScrollRect;
    [SerializeField] GameObject questListPrefab;
    [SerializeField] GameObject alertIcon;
    [SerializeField] Transform questListContent;
    [SerializeField] TMP_Text questPointText;
    [SerializeField] TMP_Text questPointPreivewText;
    [SerializeField] Slider questPointProgressSlider;
    [SerializeField] Slider questPointPreivewSlider;
    public VerticalLayoutGroup questsLayout;
    public ContentSizeFitter questFitter;
    public QuestRewardBoxButton[] questRewardButtons;

    public int questPoint;
    int maxQuestPoint = 100;

    [HideInInspector] public List<QuestList> questLists;

    void Awake()
    {
        Inst = this;
        questLists = new List<QuestList>();
    }

    void InitializeQuestList()
    {
        for (int i = 0; i < DataManager.Inst.questDatas.Count; i++)
        {
            QuestList questList = Instantiate(questListPrefab, questListContent).GetComponent<QuestList>();
            questLists.Add(questList);
            questList.Init(DataManager.Inst.questDatas[i]);
        }
        TryUpdateQuestProgress(QuestData.QuestType.Login, 1);
    }

    void InitializeRewardButton()
    {
        for (int i = 0; i < questRewardButtons.Length; i++)
        {
            questRewardButtons[i].rewardClaimedImage.SetActive(questRewardButtons[i].isQuestRewardClaimed);
        }

        // UpdateAlertIcon();
    }

    public void TryUpdateQuestProgress(QuestData.QuestType type, int progress)
    {
        QuestList targetQuest = questLists
        .FirstOrDefault(q => q.questData.questType == type);

        if (!targetQuest.isClear)
        {
            targetQuest.questData.progress += progress;
            targetQuest.UpdateQuestUI();
        }

        for (int i = 0; i < questLists.Count; i++)
        {
            if (questLists[i].questData.progress >= questLists[i].questData.maxProgress && !questLists[i].isClear)
            {
                questLists[i].transform.SetSiblingIndex(0);
            }
            else if (questLists[i].questData.isClear)
            {
                questLists[i].transform.SetSiblingIndex(questListContent.childCount - 1);
                SetupQuestUILayout();
            }
        }
    }

    public void UpdateQuestPoint(int point)
    {
        questPoint += point;
        questPointText.text = questPoint + "/" + maxQuestPoint;
        questPointPreivewText.text = questPoint + "/" + maxQuestPoint;

        if (questPointText.gameObject.activeInHierarchy)
            questPointText.GetComponent<PopUpUI>().PopUpText(questPointText.gameObject);

        questPointProgressSlider.value = (float)questPoint / maxQuestPoint;
        questPointPreivewSlider.value = (float)questPoint / maxQuestPoint;

        for (int i = 0; i < questRewardButtons.Length; i++)
        {
            if (questPointProgressSlider.value >= 0.2f * (i + 1) && !questRewardButtons[i].isQuestRewardClaimed)
                questRewardButtons[i].SetClaimState(true);
            else
                questRewardButtons[i].SetClaimState(false);
        }

        //   UpdateAlertIcon();

    }

    public void SetupQuestUI()
    {
        InitializeQuestList();
        UpdateQuestPoint(0);
        InitializeRewardButton();
    }

    public void SetupQuestUILayout()
    {
        GameUIManager.Inst.RefreshLayoutOnce(questsLayout);
        GameUIManager.Inst.RefreshFitterOnce(questFitter);
    }

    public void ResetQuestProgress()
    {
        for (int i = 0; i < DataManager.Inst.questDatas.Count; i++)
        {
            DataManager.Inst.questDatas[i].progress = 0;
            DataManager.Inst.questDatas[i].isClear = false;
        }

        questPoint = 0;
        for (int i = 0; i < questRewardButtons.Length; i++)
        {
            questRewardButtons[i].isQuestRewardClaimed = false;
        }
        SetupQuestUI();
    }

    public void ResetScrollBar()
    {
        ScrollResetter.ResetVertical(this, questListScrollRect);
    }

    void UpdateAlertIcon()
    {
        bool hasUnclaimedQuest = questLists.Any(q => !q.isClear && q.questData.progress >= q.questData.maxProgress);

        bool hasRewardBoxAvailable = questRewardButtons.Any(r => !r.isQuestRewardClaimed && r.rewardBoxButton.interactable);

        alertIcon.SetActive(hasUnclaimedQuest || hasRewardBoxAvailable);
    }
}
