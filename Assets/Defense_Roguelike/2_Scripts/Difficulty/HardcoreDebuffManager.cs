using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HardcoreDebuffManager : MonoBehaviour
{
    [SerializeField] GameObject debuffCardPrefab;
    [SerializeField] GameObject hardModeSelectDebuffPanel;
    [SerializeField] GameObject hardModeSelectDebuffButton;
    [SerializeField] Transform cardGroupParent;
    [SerializeField] HardModeDebuffData[] hardModeDebuffDatas;
    [SerializeField] GameObject background;
    [SerializeField] List<DebuffCardList> debuffCardLists;
    [SerializeField] DebuffCardList selectDebuffPanel;
    [SerializeField] Button selectDebuffButton;

    void OnEnable()
    {
        StartCoroutine(ShowDebuffCardPanel());
    }

    IEnumerator ShowDebuffCardPanel()
    {
        selectDebuffButton = Instantiate(hardModeSelectDebuffButton, InGameUIManager.Inst.eventUiGroup.transform).GetComponentInChildren<Button>();

        selectDebuffPanel = Instantiate(hardModeSelectDebuffPanel, InGameUIManager.Inst.eventUiGroup.transform).GetComponent<DebuffCardList>();

        PopUpUI popUpUI = selectDebuffButton.GetComponentInChildren<PopUpUI>();
        Button debuffButton = selectDebuffButton.GetComponentInChildren<Button>();

        debuffButton.onClick.AddListener(() => popUpUI.OnShowShrink(selectDebuffPanel.gameObject));


        selectDebuffButton.transform.parent.gameObject.SetActive(false);
        selectDebuffPanel.gameObject.SetActive(false);

        List<HardModeDebuffData> debuffDatas = new List<HardModeDebuffData>();
        debuffDatas.AddRange(GeneratedDebuffDatas());
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < debuffDatas.Count; i++)
        {
            int capturedIndex = i;
            var data = debuffDatas[capturedIndex];

            DebuffCardList debuffCardList = Instantiate(debuffCardPrefab, cardGroupParent).GetComponent<DebuffCardList>();
            Button cardBtn = debuffCardList.GetComponent<Button>();

            debuffCardList.GetComponent<PopUpUI>().OnShowShrink(debuffCardList.gameObject);
            debuffCardList.Init(data);

            cardBtn.onClick.AddListener(() => { cardBtn.interactable = false; });
            cardBtn.onClick.AddListener(() => OnHidePanel());
            cardBtn.onClick.AddListener(() => InGameUIManager.Inst.bossIntroUI.SetActive(true));
            cardBtn.onClick.AddListener(() => debuffDatas[capturedIndex].ApplyDebuff());
            cardBtn.onClick.AddListener(() => selectDebuffPanel.Init(data));
            cardBtn.interactable = false;

            debuffCardLists.Add(debuffCardList);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Crash);
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < debuffDatas.Count; i++)
        {
            debuffCardLists[i].GetComponent<Button>().interactable = true;
        }

        InGameEventManager.Inst.OnMainHUDToggled += () => selectDebuffButton.transform.parent.gameObject.SetActive(true);
    }

    List<HardModeDebuffData> GeneratedDebuffDatas()
    {
        List<HardModeDebuffData> debuffDatas = new List<HardModeDebuffData>();

        HardModeDebuffData card1;
        HardModeDebuffData card2;
        HardModeDebuffData card3;

        while (true)
        {
            card1 = hardModeDebuffDatas[Random.Range(0, hardModeDebuffDatas.Length)];
            card2 = hardModeDebuffDatas[Random.Range(0, hardModeDebuffDatas.Length)];
            card3 = hardModeDebuffDatas[Random.Range(0, hardModeDebuffDatas.Length)];


            if (card1 != card2 && card2 != card3 && card1 != card3)
            {
                break;
            }
        }

        debuffDatas.Add(card1);
        debuffDatas.Add(card2);
        debuffDatas.Add(card3);

        return debuffDatas;
    }


    void OnHidePanel()
    {
        for (int i = 0; i < debuffCardLists.Count; i++)
        {
            debuffCardLists[i].GetComponent<PopUpUI>().OnHideFold(gameObject);
        }

        background.GetComponent<DOTweenAnimation>().DOPlayById("Hide");
    }
}
