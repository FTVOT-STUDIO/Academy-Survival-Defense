using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CheatCodeList : MonoBehaviour
{
    public CheatCodeData cheatCodeData;
    CheatCodeManager cheatCodeManager;
    public int cheadId;
    [SerializeField] Button cheatButton;
    [SerializeField] Image cheatButtonIcon;
    public Slider coolTimeSlider;
    bool isActive;

    public void Init(CheatCodeData data, CheatCodeManager manager)
    {
        cheatCodeData = data;
        cheatCodeManager = manager;
        cheatButtonIcon.sprite = cheatCodeData.cheatButtonIcon;

        cheatButton.onClick.AddListener(() => OnClickCheatCodeButton());
    }

    public void OnClickCheatCodeButton()
    {
        cheatCodeManager.SetSelectedCheat(this);

    }

    public void TryActivateCheatCode()
    {
        if (!isActive && GameManager.Inst.darkMatteCoin >= cheatCodeData.applyCost)
        {
            GameManager.Inst.ModifyDarkMatteCoin(-cheatCodeData.applyCost);
            cheatCodeData.ApplyCheatCode();
            CoroutineRunner.Inst.RunCoroutine(CooldownRoutine());
            cheatCodeManager.onCheatActivated?.Invoke();
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.AbiltyOn);
        }

    }

    IEnumerator CooldownRoutine()
    {
        isActive = true;

        float elapsed = 0f;
        float duration = cheatCodeData.coolTime;

        coolTimeSlider.value = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            coolTimeSlider.value = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }

        coolTimeSlider.value = 0f;
        isActive = false;
    }

}
