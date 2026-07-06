using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JointOperationsLobbyManager : MonoBehaviour
{
    [SerializeField] JointOperationsManager jointOperationsManager;
    [SerializeField] TMP_Text jointOpertaionNameText;
    [SerializeField] TMP_Text jointOpertaionDescText;
    [SerializeField] TMP_Text jointOpertaionCycleText;
    [SerializeField] TMP_Text jointOpertaionDifficultyText;

    [SerializeField] GameObject diffcultyIncreaseButtonBlockedImage;
    [SerializeField] GameObject diffiultyDecreaseButtonBlockedImage;

    [SerializeField] Image titleImage;

    public DateTime CurrentStartDate { get; private set; }
    public DateTime CurrentEndDate { get; private set; }

    void Start()
    {
        DifficultyManager.Inst.OnJointOperationDifficultyChanged += UpdateJointOperationDifficultyUI;
    }

    public void Init(JointOperationData jointOperationData)
    {
        jointOpertaionNameText.text = "연합작전 : " + jointOperationData.jointOperationName;
        jointOpertaionDescText.text = jointOperationData.JointOperationDesc;
        titleImage.sprite = jointOperationData.titleSprite;
        CheckWeek(DateTime.Now);
        jointOpertaionCycleText.text = GetRangeText();

        UpdateJointOperationDifficultyUI(jointOperationsManager.curJointOperationDifficulty);

        UpdateBlocekdImage();
    }

    public void UpdateWeek(DateTime currentDate)
    {
        int diff = (int)currentDate.DayOfWeek - (int)DayOfWeek.Monday;

        if (diff < 0)
        {
            diff += 7;
        }

        CurrentStartDate = currentDate.Date.AddDays(-diff);
        CurrentEndDate = CurrentStartDate.AddDays(7);
    }

    public void CheckWeek(DateTime now)
    {
        UpdateWeek(now);

        if (string.IsNullOrEmpty(PlayerManager.Inst.lastLoginTime))
        {
            PlayerManager.Inst.lastLoginTime = now.ToString("O");
            return;
        }

        if (!DateTimeOffset.TryParse(
            PlayerManager.Inst.lastLoginTime,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out DateTimeOffset lastLoginOffset))
        {
            Debug.LogWarning("lastLoginTime 파싱 실패");
            PlayerManager.Inst.lastLoginTime = now.ToString("O");
            return;
        }

        DateTime lastLogin = lastLoginOffset.LocalDateTime;

        if (lastLogin < CurrentStartDate)
        {
            Debug.Log("일주일이 지났습니다! (주간 변경)");
            OnWeekChanged();
        }

        PlayerManager.Inst.lastLoginTime = now.ToString("O");
    }
    private void OnWeekChanged()
    {
        GameModeManager.Inst.jointOperationsManager.curJointOperationData = GameModeManager.Inst.jointOperationsManager.GetRandomJointOperationData();
    }

    public string GetRangeText()
    {
        return $"{CurrentStartDate:yyyy MM/dd} ~ {CurrentEndDate:yyyy MM/dd}";
    }

    public void IncreaseJointOperationDifficulty()
    {
        int maxIndex = Enum.GetValues(typeof(JointOperationsManager.JointOperationDifficulty)).Length - 1;

        var current = jointOperationsManager.curJointOperationDifficulty;

        diffcultyIncreaseButtonBlockedImage.SetActive((int)current >= maxIndex - 1);

        if ((int)current >= maxIndex)
            return;

        var nextDifficulty = (JointOperationsManager.JointOperationDifficulty)((int)current + 1);

        jointOperationsManager.curJointOperationDifficulty = nextDifficulty;

        DifficultyManager.Inst.SetJointOperationDifficulty(nextDifficulty);
    }

    public void DecreaseJointOperationDifficulty()
    {
        var current = jointOperationsManager.curJointOperationDifficulty;

        diffiultyDecreaseButtonBlockedImage.SetActive(current <= JointOperationsManager.JointOperationDifficulty.Normal);

        if (current <= JointOperationsManager.JointOperationDifficulty.Normal)
            return;

        var prevDifficulty = (JointOperationsManager.JointOperationDifficulty)((int)current - 1);

        DifficultyManager.Inst.SetJointOperationDifficulty(prevDifficulty);
    }

    public void UpdateJointOperationDifficultyUI(JointOperationsManager.JointOperationDifficulty difficulty)
    {
        jointOpertaionDifficultyText.text = jointOperationsManager.GetDifficultyKorean[jointOperationsManager.curJointOperationDifficulty];
    }

    public void UpdateBlocekdImage()
    {
        var current = jointOperationsManager.curJointOperationDifficulty;

        diffcultyIncreaseButtonBlockedImage.SetActive(current == JointOperationsManager.JointOperationDifficulty.Oblivion);
        diffiultyDecreaseButtonBlockedImage.SetActive(current == JointOperationsManager.JointOperationDifficulty.Normal);
    }
}
