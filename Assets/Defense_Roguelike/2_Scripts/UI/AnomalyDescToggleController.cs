using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyDescToggleController : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text descText;
    [SerializeField] GameObject descPanel;
    [SerializeField] Toggle[] toggles;

    void OnDisable()
    {
        foreach (var toggle in toggles)
        {
            if (toggle.isOn)
            {
                toggle.isOn = false;
                break;
            }
        }
    }

    public void SetUpKnight(bool isOn)
    {
        descPanel.SetActive(isOn);
        nameText.text = "종말을 선고하는 자";
        descText.text = "도전 시작 시 암흑 기사의 공격력과 체력이 증가합니다.";
    }

    public void SetUpBeast(bool isOn)
    {
        descPanel.SetActive(isOn);
        nameText.text = "종말을 인도하는 자";
        descText.text = "도전 시작 시 암흑 괴수의 공격 속도와 이동 속도가 증가합니다.";
    }

    public void SetUpLightHouse(bool isOn)
    {
        descPanel.SetActive(isOn);
        nameText.text = "최후의 등불을 지키는 자";
        descText.text = "도전 시작 시 자신을 포함한 모든 몬스터의 이동속도를 증가시키고, 보호막을 부여합니다";
    }
}
