using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanelManger : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.Inst.skillCardManager.InitializeCardUI();
        GameManager.Inst.skillCardManager.ShowSkillCard(GameStatManager.Inst.ShowSkillCardCount);
    }

    void OnDisable()
    {
        GameManager.Inst.skillCardManager.HideAllCards();
    }
}