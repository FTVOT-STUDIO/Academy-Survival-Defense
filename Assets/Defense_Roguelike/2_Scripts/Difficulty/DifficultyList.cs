using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DifficultyList : MonoBehaviour
{
    public Toggle toggle;
    public Toggle descToggle;
    [HideInInspector] public DifficultyData.DifficultyType difficultyType;
    [HideInInspector] public DifficultyData difficultyData;

    public Image difficultyIllust;
    public Image difficultyLineColor;
    public TMP_Text difficultyNameText;
    public TMP_Text chapterNameText;
    public TMP_Text descText;

    public void Init()
    {
        difficultyType = difficultyData.difficultyType;

        difficultyIllust.sprite = difficultyData.illust;
        difficultyLineColor.color = difficultyData.lineColor;

        difficultyNameText.text = difficultyData.difficultyName;

        chapterNameText.text = difficultyData.chapterName;
        chapterNameText.color = difficultyData.chapterColor;
        descText.text = difficultyData.desc;

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDisable()
    {
        descToggle.SetIsOnWithoutNotify(false);
    }

    void OnToggleChanged(bool isOn)
    {
        PlayerManager.Inst.difficultyType = difficultyType;
        DifficultyManager.Inst.difficultyChanger.gameObject.SetActive(true);
        DifficultyManager.Inst.difficultyChanger.InitMenu(difficultyData);
        DifficultyManager.Inst.difficultyMenu.GetComponent<PopUpUI>().OnHideFold(DifficultyManager.Inst.difficultyMenu);
        DifficultyManager.Inst.Invoke(nameof(DifficultyManager.UpdateBackgroundByDifficulty), 1.5f);
        toggle.interactable = !isOn;
        DifficultyManager.Inst.curDifficultyData = difficultyData;
    }

}
