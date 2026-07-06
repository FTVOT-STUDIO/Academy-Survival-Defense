using UnityEngine;
using UnityEngine.XR;

public class GameIntroGroup : MonoBehaviour
{
    public GameObject unitInfoUI;

    public GameObject hardModeIntroUI;
    public GameObject ApocalypseIntroUI;

    public GameObject bossIntroUI;


    public void ShowNextUI()
    {
        switch (GameManager.Inst.gameMode)
        {
            case GameManager.GameMode.Normal:
                switch (GameManager.Inst.curGameDifficulty)
                {
                    case DifficultyData.DifficultyType.Normal:
                        ToggleBossUI();
                        break;
                    case DifficultyData.DifficultyType.Hard:
                        GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0, 0.4f));
                        hardModeIntroUI.SetActive(true);
                        break;
                }
                break;
            case GameManager.GameMode.JointOperation:
                ToggleBossUI();
                break;
        }
        unitInfoUI.SetActive(false);
    }

    public void ToggleBossUI()
    {
        bossIntroUI.SetActive(!bossIntroUI.activeSelf);
    }

}
