using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCombatStatList : MonoBehaviour
{

    [SerializeField] UnitData unitData;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text valueText;
    [SerializeField] Image portrait;
    [SerializeField] Image sliderFill;
    public Slider slider;
    [Space]
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;

    public void Init(UnitData data, UnitCombatStatManager.StatType stat)
    {
        unitData = data;
        UnitCombatStatManager.Inst.statType = stat;

        portrait.sprite = data.portrait;
        RectTransform portraitRect = portrait.GetComponent<RectTransform>();
        portraitRect.sizeDelta = new Vector2(portraitWidth * unitData.portraitScaleFactor, portraitHeight * unitData.portraitScaleFactor);
        portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * unitData.portraitPosXFactor, (1 + portraitPosY) * unitData.portraitPosYFactor);

        nameText.text = data.unitName;
        slider.value = 0;
        valueText.text = "0";
    }

    public void UpdateStat(UnitCombatStatManager.StatType stat)
    {
        for (int i = 0; i < UnitCombatStatManager.Inst.statChangeToggles.Length; i++)
        {
            if (UnitCombatStatManager.Inst.statChangeToggles[i].isOn && i == (int)stat)
            {
                float maxValue = 0;
                switch (stat)
                {
                    case UnitCombatStatManager.StatType.damage:
                        maxValue = (float)UnitCombatStatManager.Inst.GetMaxDamage();
                        break;
                    case UnitCombatStatManager.StatType.heal:
                        maxValue = (float)UnitCombatStatManager.Inst.GetMaxHeal();
                        break;
                    case UnitCombatStatManager.StatType.takenDamage:
                        maxValue = (float)UnitCombatStatManager.Inst.GetMaxTakenDamage();
                        break;
                }

                for (int j = 0; j < UnitCombatStatManager.Inst.unitStatEntries.Count; j++)
                {
                    var statList = UnitCombatStatManager.Inst.unitStatEntries[j];
                    double currentValue = 0;

                    switch (stat)
                    {
                        case UnitCombatStatManager.StatType.damage:
                            currentValue = statList.damage;
                            break;
                        case UnitCombatStatManager.StatType.heal:
                            currentValue = statList.heal;
                            break;
                        case UnitCombatStatManager.StatType.takenDamage:
                            currentValue = statList.takenDamage;
                            break;
                    }

                    statList.unitCombatStatList.slider.maxValue = maxValue;
                    statList.unitCombatStatList.slider.value = (float)currentValue;
                    statList.unitCombatStatList.valueText.text = currentValue.ToString("F0");
                    statList.unitCombatStatList.ChangeSliderColor(stat);
                }
                UnitCombatStatManager.Inst.SortUnitCombatStatList(UnitCombatStatManager.Inst.statType);
                break;
            }
        }
    }

    public void ChangeSliderColor(UnitCombatStatManager.StatType statType)
    {
        switch (statType)
        {
            case UnitCombatStatManager.StatType.damage:
                sliderFill.color = new Color(1, 0, 0.3f);
                break;
            case UnitCombatStatManager.StatType.heal:
                sliderFill.color = new Color(0.1f, 1, 0);
                break;
            case UnitCombatStatManager.StatType.takenDamage:
                sliderFill.color = new Color(0, 0.8f, 1);
                break;
        }
    }
}
