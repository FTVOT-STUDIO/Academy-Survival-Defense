using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestList : MonoBehaviour
{
    [HideInInspector] public QuestData questData;
    [SerializeField] TMP_Text questName;
    [SerializeField] TMP_Text progressText;
    [SerializeField] TMP_Text rewardQuestExpText;

    [SerializeField] Slider progressSlider;
    [SerializeField] Button questListButton;

    [SerializeField] GameObject clearText;
    [SerializeField] GameObject clearOutline;
    [SerializeField] GameObject questClearPanel;

    public bool isClear = false;


    public void Init(QuestData data)
    {
        questData = data;
        UpdateQuestUI();
        questClearPanel.SetActive(isClear);
        questListButton.interactable = !isClear;
        questListButton.onClick.AddListener(() => CheckQuestAndReward());

    }

    public void UpdateQuestUI()
    {
        isClear = questData.isClear;

        questName.text = string.Format(questData.questName, questData.maxProgress);
        progressText.text = questData.progress + "/" + questData.maxProgress;
        rewardQuestExpText.text = questData.questPoint.ToString();
        progressSlider.value = (float)questData.progress / questData.maxProgress;

        clearText.SetActive(questData.progress >= questData.maxProgress);
        progressText.gameObject.SetActive(!(questData.progress >= questData.maxProgress));
        clearOutline.SetActive(questData.progress >= questData.maxProgress);
    }

    void CheckQuestAndReward()
    {
        if (questData.progress >= questData.maxProgress)
        {
            questListButton.interactable = false;
            questClearPanel.SetActive(true);
            questData.isClear = true;
            isClear = true;

            QuestManager.Inst.UpdateQuestPoint(questData.questPoint);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.QuestClaer);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.boxOpen);
            StartCoroutine(MoveToBottomRoutine());
        }
    }
    IEnumerator MoveToBottomRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        transform.SetSiblingIndex(transform.parent.childCount - 1);

        GameUIManager.Inst.RefreshLayoutOnce(QuestManager.Inst.questsLayout);
        GameUIManager.Inst.RefreshFitterOnce(QuestManager.Inst.questFitter);
    }


}
