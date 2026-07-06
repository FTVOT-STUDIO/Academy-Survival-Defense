using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResultUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text difficultyText;
    [SerializeField] TMP_Text waveText;
    DeckToggle deckToggle;

    void Awake()
    {
        deckToggle = GetComponent<DeckToggle>();
    }

    void OnEnable()
    {
        InGameUIManager.Inst.gameStatePanel.SetActive(false);
        ResultWindow();

        deckToggle.InitDeckListState();

    }
    void ResultWindow()
    {
        resultText.text = GameManager.Inst.isVictory ? resultText.text = "방어 성공" : resultText.text = "방어 실패";
        waveText.text = "Wave " + GameManager.Inst.wave.ToString();
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GameResult);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScreenChange1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Fail);

        for (int i = 0; i < DifficultyManager.Inst.difficultyList.Length; i++)
        {
            bool isCurrentDifficulty = DifficultyManager.Inst.difficultyList[i].difficultyType == GameManager.Inst.curGameDifficulty;
            if (isCurrentDifficulty)
            {
                difficultyText.text = DifficultyManager.Inst.difficultyList[i].difficultyData.difficultyName;
                break;
            }
        }

        InGameUIManager.Inst.ToggleMainHUD(false);
        GameManager.Inst.Pause();
        AudioManager.Inst.StopBgm();
        AudioManager.Inst.PlayBgm(AudioManager.Bgm.Result);
    }
}
