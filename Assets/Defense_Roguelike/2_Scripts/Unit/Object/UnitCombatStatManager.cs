using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UnitStatEntry
{
    public UnitData unitData;
    public double damage;
    public double heal;
    public float takenDamage;
    public UnitCombatStatList unitCombatStatList;
}

public class UnitCombatStatManager : MonoBehaviour
{
    public static UnitCombatStatManager Inst;
    public enum StatType { damage, heal, takenDamage }
    public StatType statType;
    [SerializeField] TMP_Text statTypeText;
    [SerializeField] Transform unitCombatStatParent;
    [SerializeField] Transform unitCombatListGroupParent;
    public Toggle[] statChangeToggles;
    public List<UnitStatEntry> unitStatEntries = new();
    void Awake()
    {
        Inst = this;
    }

    public void Init()
    {
        var rect = unitCombatStatParent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0f);

        unitStatEntries.Clear();

        foreach (var unit in DeckManager.Inst.currentInGameDeck.unitdatas)
        {
            unitStatEntries.Add(new UnitStatEntry
            {
                unitData = unit,
                damage = 0,
                heal = 0,
                unitCombatStatList = CreateUnitCombatStatList(unit)
            });
        }

        var sephira = DeckManager.Inst.currentInGameDeck.sephiraUnit;
        if (sephira != null)
        {
            unitStatEntries.Add(new UnitStatEntry
            {
                unitData = sephira,
                damage = 0,
                heal = 0,
                unitCombatStatList = CreateUnitCombatStatList(sephira)
            });
        }

        statChangeToggles[0].isOn = true;
    }

    public void AddDamage(UnitData targetData, double value)
    {
        var entry = GetOrCreateEntry(targetData);
        entry.damage += value;

        entry.unitCombatStatList?.UpdateStat(StatType.damage);
    }

    public void AddHeal(UnitData targetData, double value)
    {
        var entry = GetOrCreateEntry(targetData);
        entry.heal += value;

        entry.unitCombatStatList?.UpdateStat(StatType.heal);
    }

    public void AddTakenDamage(UnitData targetData, float value)
    {
        var entry = GetOrCreateEntry(targetData);
        entry.takenDamage += value;

        entry.unitCombatStatList?.UpdateStat(StatType.takenDamage);
    }

    UnitStatEntry GetOrCreateEntry(UnitData targetData)
    {
        var entry = unitStatEntries.Find(e => e.unitData == targetData);

        if (entry == null)
        {
            entry = new UnitStatEntry
            {
                unitData = targetData,
                damage = 0,
                heal = 0,
                unitCombatStatList = CreateUnitCombatStatList(targetData)
            };
            unitStatEntries.Add(entry);
        }

        return entry;
    }

    public UnitCombatStatList CreateUnitCombatStatList(UnitData data)
    {
        UnitCombatStatList unitCombat = PoolManager.Inst.Get(PoolManager.PrefabType.UnitCombatStatList, transform.position).GetComponent<UnitCombatStatList>();
        unitCombat.transform.SetParent(unitCombatListGroupParent, false);
        var rect = unitCombatStatParent.GetComponent<RectTransform>();
        rect.sizeDelta += new Vector2(0, 160);
        unitCombat.Init(data, statType);
        unitCombat.UpdateStat(statType);

        return unitCombat;
    }

    // ui 리스트 추가 및 콘텐츠 사이즈 조정 함수 만들기

    public double GetMaxDamage()
    {
        return unitStatEntries.Count > 0 ? unitStatEntries.Max(entry => entry.damage) : 0;
    }

    public double GetMaxHeal()
    {
        return unitStatEntries.Count > 0 ? unitStatEntries.Max(entry => entry.heal) : 0;
    }

    public double GetMaxTakenDamage()
    {
        return unitStatEntries.Count > 0 ? unitStatEntries.Max(entry => entry.takenDamage) : 0;
    }

    public void SortUnitCombatStatList(StatType stat)
    {
        switch (stat)
        {
            case StatType.damage:
                unitStatEntries.Sort((a, b) => b.damage.CompareTo(a.damage));
                break;
            case StatType.heal:
                unitStatEntries.Sort((a, b) => b.heal.CompareTo(a.heal));
                break;
            case StatType.takenDamage:
                unitStatEntries.Sort((a, b) => b.takenDamage.CompareTo(a.takenDamage));
                break;
        }

        for (int i = 0; i < unitStatEntries.Count; i++)
        {
            unitStatEntries[i].unitCombatStatList.transform.SetSiblingIndex(i);

        }
    }

    public void ChangeStat(int index)
    {
        statType = (StatType)index;
        for (int i = 0; i < unitStatEntries.Count; i++)
        {
            var statList = unitStatEntries[i];
            statList.unitCombatStatList.UpdateStat(statType);
        }

        switch (index)
        {
            case 0:
                statTypeText.text = "누적 피해량";
                break;
            case 1:
                statTypeText.text = "누적 회복량";
                break;
            case 2:
                statTypeText.text = "받은 피해량";
                break;
        }
    }
}