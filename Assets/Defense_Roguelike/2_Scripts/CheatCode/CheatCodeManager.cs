using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] CheatCodeData[] cheatCodeDatas;
    [SerializeField] CheatCodeList[] cheatCodeLists;
    [SerializeField] Button cheatCodeApplyButton;
    [SerializeField] Image cheatInfoIcon;
    [SerializeField] TMP_Text cheatNameText;
    [SerializeField] TMP_Text cheatDescText;
    [SerializeField] TMP_Text cheatCostText;
    [SerializeField] TMP_Text cheatCoolTimeText;
    CheatCodeList selectList;
    public Action onCheatActivated;

    void Awake()
    {
        cheatCodeDatas = cheatCodeDatas.OrderBy(u => u.cheatId).ToArray();
        cheatCodeLists = cheatCodeLists.OrderBy(u => u.cheadId).ToArray();

        for (int i = 0; i < cheatCodeLists.Length; i++)
        {
            cheatCodeLists[i].Init(cheatCodeDatas[i], this);
        }

        SetSelectedCheat(cheatCodeLists[0]);

        cheatCodeApplyButton.onClick.AddListener(() => RequestCheatCode());

        for (int i = 0; i < cheatCodeLists.Length; i++)
        {
            cheatCodeLists[i].coolTimeSlider.value = 0;
        }

    }

    public void RequestCheatCode()
    {
        selectList.TryActivateCheatCode();
    }

    public void SetSelectedCheat(CheatCodeList list)
    {
        selectList = list;
        cheatInfoIcon.sprite = selectList.cheatCodeData.cheatInfoIcon;
        cheatNameText.text = selectList.cheatCodeData.cheatName;
        cheatDescText.text = selectList.cheatCodeData.cheatDesc;
        cheatCostText.text = "x" + selectList.cheatCodeData.applyCost;
        cheatCoolTimeText.text = selectList.cheatCodeData.coolTime + "초";
    }
}
