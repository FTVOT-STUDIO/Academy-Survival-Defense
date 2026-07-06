using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FactionBonusCalculator : MonoBehaviour
{
    public enum FactionBonusView { Normal, Arcade, JointOpertaion, StatPanel }

    public void SetFactionBonusInfo(UnitData[] curDeckUnitDatas, UnitData curSephiraData, FactionBonusView factionBonusView)
    {
        List<FactionBonusInfo> factionBonusInfos = GetFactionBonusInfos(curDeckUnitDatas, curSephiraData);

        switch (factionBonusView)
        {
            case FactionBonusView.Normal:
                DeckManager.Inst.normalDeckFactionController.Init(factionBonusInfos);
                break;
            case FactionBonusView.JointOpertaion:
                DeckManager.Inst.jointOpertaionDeckFactionController.Init(factionBonusInfos);
                break;
            case FactionBonusView.StatPanel:
                InGameUIManager.Inst.inGamefactionBonusController.Init(factionBonusInfos);
                break;
        }
        List<Faction> factions = new List<Faction>();

        for (int i = 0; i < factionBonusInfos.Count; i++)
        {
            factions.Add(factionBonusInfos[i].faction);
        }

        switch (factionBonusView)
        {
            case FactionBonusView.Normal:
                DeckManager.Inst.curDeckFactionBonus = factions;
                break;
            case FactionBonusView.JointOpertaion:
                DeckManager.Inst.jointOpertaionDeckFactionBonus = factions;
                break;
        }

    }

    public List<FactionBonusInfo> GetFactionBonusInfos(UnitData[] curDeckUnitDatas, UnitData curSephiraData)
    {
        List<FactionBonusInfo> factionBonusInfos = new();

        List<UnitData> totalUnits = new(curDeckUnitDatas)
        {
            curSephiraData
        };

        var groupedByFaction = totalUnits
            .GroupBy(unit => unit.faction)
            .ToDictionary(group => group.Key, group => group.Count());

        foreach (var group in groupedByFaction)
        {
            if (group.Value < 4)
                continue;

            string bonusText = string.Empty;

            switch (group.Key)
            {
                case Faction.None:
                    bonusText = "은하 어딘가, 당신과 여정을 함께할 멋진 동료들을 만나길";
                    break;
                case Faction.DL_UniverseAcademy:
                    bonusText = "델타루나 유니버스 아카데미 보너스 적용 : 공격력 20% 증가";
                    break;

                case Faction.City_Of_Chesed:
                    bonusText = "헤세드시 보너스 적용 : 방어력 10% 증가";
                    break;

                case Faction.Four_Horsemen_Of_The_Apocalypse:
                    bonusText = "종말의 네 기수 보너스 적용 : 치명타 확률 20% 증가";
                    break;

                case Faction.Project_P:
                    bonusText = "프로젝트 P 보너스 적용 : 소환 비용 20% 감소";
                    break;

                case Faction.Arcade_Ranger:
                    bonusText = "아케이드 레인저 보너스 적용 : 공격속도 5% 증가";
                    break;

                case Faction.Qliphoth_Trinity:
                    bonusText = "클리포트 트리니티 보너스 적용 : 치명타 피해 30% 증가";
                    break;

                case Faction.Void_Stalker:
                    bonusText = "공허 추적자 보너스 적용 : 적 약점 20% 증가";
                    break;

                case Faction.SepherIgnusLab:
                    bonusText = "세페르이그너스 랩 저장소 보너스 적용 : 쿨타임 20% 감소";
                    break;

                case Faction.GPS_Shock_Troop:
                    bonusText = "GPS 타격대 보너스 적용 : 치명타 피해 30% 증가";
                    break;

                case Faction.A_Strange_And_Wonderful_Country:
                    bonusText = "기묘하고 이상한 나라 보너스 적용 : EXP 획득량 10% 증가";
                    break;
            }

            FactionBonusInfo factionBonusInfo =
                new(group.Key, bonusText);

            factionBonusInfos.Add(factionBonusInfo);
        }

        return factionBonusInfos;
    }


    public void ApplyFactionBonusStat(Faction curFaction)
    {
        switch (curFaction)
        {
            case Faction.DL_UniverseAcademy:
                GameStatManager.Inst.AddGameDamage(0.2f);
                break;
            case Faction.City_Of_Chesed:
                GameStatManager.Inst.AddGameDefense(0.1f);
                break;
            case Faction.Four_Horsemen_Of_The_Apocalypse:
                GameStatManager.Inst.AddGameCriticalPer(0.2f);
                break;
            case Faction.Project_P:
                GameManager.Inst.unitSpawnCost -= (int)0.2f;
                break;
            case Faction.Arcade_Ranger:
                GameStatManager.Inst.AddGameAttackSpeed(0.05f);
                break;
            case Faction.Qliphoth_Trinity:
                GameStatManager.Inst.AddGameCriticalDamage(0.3f);
                break;
            case Faction.Void_Stalker:
                GameStatManager.Inst.AddGameWeakness(0.2f);
                break;
            case Faction.SepherIgnusLab:
                GameStatManager.Inst.AddGameCoolTime(0.2f);
                break;
            case Faction.GPS_Shock_Troop:
                GameStatManager.Inst.AddGameCriticalDamage(0.3f);
                break;
            case Faction.A_Strange_And_Wonderful_Country:
                GameStatManager.Inst.AddGameExpValue(0.1f);
                break;
        }
    }

    public class FactionBonusInfo
    {
        public Faction faction;
        public string bonusText;

        public FactionBonusInfo(Faction faction, string bonusText)
        {
            this.faction = faction;
            this.bonusText = bonusText;
        }
    }

}


