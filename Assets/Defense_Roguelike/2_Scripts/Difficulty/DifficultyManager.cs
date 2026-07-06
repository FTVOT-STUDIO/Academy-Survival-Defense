using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Inst;

    public DifficultyData[] difficultyData;
    public DifficultyList[] difficultyList;

    public GameObject difficultyMenu;
    public DifficultyChanger difficultyChanger;

    public DifficultyData curDifficultyData;

    public Action<JointOperationsManager.JointOperationDifficulty> OnJointOperationDifficultyChanged;
    void Awake()
    {
        Inst = this;

        for (int i = 0; i < difficultyList.Length; i++)
        {
            difficultyList[i].difficultyData = difficultyData[i];
            difficultyList[i].Init();
        }
    }

    public void InitializeDifficultyToggles()
    {
        for (int i = 0; i < difficultyList.Length; i++)
        {
            bool isCurrentDifficulty = difficultyList[i].difficultyType == PlayerManager.Inst.difficultyType;

            difficultyList[i].toggle.SetIsOnWithoutNotify(isCurrentDifficulty);
            difficultyList[i].toggle.interactable = !isCurrentDifficulty;
        }
    }

    public void UpdateBackgroundByDifficulty()
    {
        for (int i = 0; i < difficultyList.Length; i++)
        {
            bool isCurrentDifficulty = difficultyList[i].difficultyType == PlayerManager.Inst.difficultyType;
            if (isCurrentDifficulty)
            {
                LobbyManager.Inst.lobbyMainBackground.color = difficultyList[i].difficultyData.backGroundColor;
                LobbyManager.Inst.lobbyPatternOverlay.InitMaterial(difficultyList[i].difficultyData.overlayPatternMaterial);
                break;
            }
        }
    }

    public void SetJointOperationDifficulty(JointOperationsManager.JointOperationDifficulty difficulty)
    {
        GameModeManager.Inst.jointOperationsManager.curJointOperationDifficulty = difficulty;

        OnJointOperationDifficultyChanged?.Invoke(difficulty);
    }
}
