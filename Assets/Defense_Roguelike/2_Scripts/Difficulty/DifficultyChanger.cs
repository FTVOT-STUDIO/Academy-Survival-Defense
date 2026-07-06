using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyChanger : MonoBehaviour
{
    [SerializeField] GameObject menuChange;
    [SerializeField] Image icon;
    [SerializeField] Image lightImage;
    [SerializeField] TMP_Text chapterNameText;
    [SerializeField] TMP_Text difficultyNameText;


    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(HideMenu), 2.8f);
    }

    public void InitMenu(DifficultyData difficultyData)
    {
        icon.sprite = difficultyData.icon;
        chapterNameText.text = difficultyData.popupMessage;
        difficultyNameText.text = difficultyData.difficultyName;
        difficultyNameText.color = difficultyData.chapterColor;
        lightImage.color = difficultyData.chapterColor;
    }

    void HideMenu()
    {
        menuChange.SetActive(false);
    }

    void ShowFlashImage()
    {
        GameUIManager.Inst.ShowFleshImage(LobbyManager.Inst.lobbyUiCanvas.transform, Color.white);
    }
}
