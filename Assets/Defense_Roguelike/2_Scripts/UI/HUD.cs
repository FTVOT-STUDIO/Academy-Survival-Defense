using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    TMP_Text tMP_Text;
    TMP_Text[] tMP_Texts;
    Text text;
    Animator anim;
    Toggle toggle;
    Slider slider;
    Scrollbar scrollbar;
    Button button;
    TMP_InputField inputField;
    bool isCoroutineActive;

    public enum HudType
    {
        WaveText, TimeText, coinText, curCoinText, DarkMatteCoinText, curDarkMatteCoinText,
        JOP_UnlockConditionCount, speedToggle, EXP, levelText, health, HpText, AbiltySlider, AbiltyCountText,
        AnomalyLevel, goldText, gemText, playerNameText, playerLevel, playerLevelSlider, GameDamage, GameSkillValue, GameDefense,
        GameHealth, GameShield, GameCriticalPer, GameCriticalDamage, GameAttackSpeed, GameRange, GameCoolTime, GameGetCoin, GameCoinDamage,
        GameWeakness, GameExp, SpentAnomalyLevel, SpentCoin, SpentDarkMatteCoin, SpentSpawn, SpentAblity, DeadUnit, NickNameInputField,
        BoxOpenCountSlider, BoxOpenCountText, OpenGachaAgainText, GachaBoxCount, rewardListSlider, GameSlow, GameBossSkillCoolTime,
        FactionBonus, DailyCounter
    }

    public HudType type;

    void Awake()
    {
        tMP_Text = GetComponentInChildren<TextMeshProUGUI>();
        text = GetComponent<Text>();
        anim = GetComponent<Animator>();
        slider = GetComponent<Slider>();
        button = GetComponent<Button>();
        inputField = GetComponent<TMP_InputField>();
        scrollbar = GetComponent<Scrollbar>();
    }

    void OnEnable()
    {
        switch (type)
        {
            case HudType.FactionBonus:
                DeckManager deck = DeckManager.Inst;

                deck.factionBonusCalculator.
        SetFactionBonusInfo(deck.curDeckUnitDatas.ToArray(), deck.curDeckSephiraData, FactionBonusCalculator.FactionBonusView.StatPanel);

                break;
        }
    }

    void LateUpdate()
    {
        switch (type)
        {
            case HudType.TimeText:
                text.text = "00:" + GameManager.Inst.timer.ToString("F1");
                break;
            case HudType.AbiltySlider:

                break;
            case HudType.BoxOpenCountSlider:
                int maxSliderValue = PlayerManager.Inst.voidTicket / 30;
                if (30 * (int)slider.value > PlayerManager.Inst.voidTicket || 30 * (int)slider.value % PlayerManager.Inst.voidTicket != 0)
                {
                    slider.value = (int)slider.value;
                }
                if (slider.value > maxSliderValue)
                {
                    slider.value = maxSliderValue;
                }
                tMP_Text.text = 30 * (int)slider.value + "/" + PlayerManager.Inst.voidTicket;
                GachaManager.Inst.boxOpenCount = (int)slider.value;
                break;
            case HudType.BoxOpenCountText:
                tMP_Text.text = "x" + GachaManager.Inst.boxOpenCount;
                break;
            case HudType.OpenGachaAgainText:
                break;
            case HudType.GachaBoxCount:
                tMP_Text.text = 30 + "/" + PlayerManager.Inst.voidTicket;
                int ticketCount = PlayerManager.Inst.voidTicket;
                float sliderValue = Mathf.Clamp01(ticketCount / 30f);
                slider.value = sliderValue;
                break;
            case HudType.DailyCounter:
                DateTime midnight = DateTime.Today.AddDays(1);
                TimeSpan remaining = midnight - DateTime.Now;
                tMP_Text.text = $"새로고침 까지 남은 시간\n{remaining.Hours}:{remaining.Minutes}:{remaining.Seconds}";
                break;

        }
    }

    public void NickNameInputField(GameObject parent)
    {
        if (type == HudType.NickNameInputField)
        {
            string nickNamePrompt = inputField.text;

            if (string.IsNullOrWhiteSpace(nickNamePrompt))
            {
                if (!isCoroutineActive)
                {
                    gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1].enabled = true;
                    StartCoroutine(ChangeTextColor(gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1], 1f));
                }

                isCoroutineActive = true;
                return;
            }
            PlayerManager.Inst.SetPlayerName(nickNamePrompt);
            PlayerManager.Inst.isTutorial = false;
            parent.SetActive(false);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select3);
            DataManager.Inst.SaveData();
        }
    }

    IEnumerator ChangeTextColor(TMP_Text textComponent, float duration)
    {
        const string DefaultPrompt = "이름을 입력하세요. (최대 8자)";
        const string ErrorPrompt = "올바른 이름을 입력해주세요!";

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            textComponent.text = ErrorPrompt;
            textComponent.color = Color.Lerp(Color.red, Color.white, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textComponent.color = Color.white;
        textComponent.text = DefaultPrompt;
        isCoroutineActive = false;
    }

}
