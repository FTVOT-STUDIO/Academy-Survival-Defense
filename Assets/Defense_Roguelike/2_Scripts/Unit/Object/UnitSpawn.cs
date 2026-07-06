using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawn : MonoBehaviour
{
    int curUnitSpawnCoin;
    public int freeSpawnCount = 0;
    [SerializeField] TMP_Text curCoinText;
    [Space]
    [SerializeField] GameObject spawnUI;
    [SerializeField] GameObject freeSpawnUI;
    [SerializeField] Text freeSpawnCountText;

    void Awake()
    {
        curUnitSpawnCoin = 10;
        curCoinText.text = $"{curUnitSpawnCoin}";
    }

    void Update()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnUnit();
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            ModifyCoinFreeSpawnCount(1);
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            ModifyCoinFreeSpawnCount(-1);
        }
    }
    public void SpawnUnit()
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ButtonClick1);

        if (!GameManager.Inst.isPause && GameManager.Inst.isGameStart && FieldManager.Inst.fieldUnits.Any(unit => unit == null))
        {
            bool canSpawn = false;

            if (freeSpawnCount > 0)
            {
                ModifyCoinFreeSpawnCount(-1);
                canSpawn = true;
            }
            else if (curUnitSpawnCoin <= GameManager.Inst.coin)
            {
                GameManager.Inst.ModifyCoin(-curUnitSpawnCoin);
                curUnitSpawnCoin += GameManager.Inst.unitSpawnCost;
                curCoinText.text = curUnitSpawnCoin.ToString();
                canSpawn = true;
            }

            if (canSpawn)
            {
                List<UnitData> datas = DeckManager.Inst.currentInGameDeck.unitdatas;
                GenerateUnit(datas[Random.Range(0, datas.Count)]);
                InGameUIManager.Inst.TryBonusCardDrop();
                QuestManager.Inst.TryUpdateQuestProgress(QuestData.QuestType.UnitSpawn, 1);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.SpawnUnit);
            }
        }

    }

    public Unit GenerateUnit(UnitData unitData, bool callOnBeforeTrans = true)
    {
        GameObject[] fieldUnits = FieldManager.Inst.fieldUnits;
        Transform[] fieldSlots = FieldManager.Inst.fieldSlots;

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, fieldUnits.Length);
        }
        while (FieldManager.Inst.fieldUnits[randomIndex] != null);

        Transform randomSlot = fieldSlots[randomIndex].transform;

        UnitData generateData = unitData;

        GameObject unitObj = PoolManager.Inst.Get(PoolManager.PrefabType.Unit, randomSlot.position);
        Unit unit = unitObj.GetComponent<Unit>();

        unit.unitData = generateData;
        UnitDrag unitDrag = unitObj.GetComponent<UnitDrag>();
        unitDrag.curIndex = randomIndex;
        fieldUnits[randomIndex] = unitObj;
        unit.Init(callOnBeforeTrans: callOnBeforeTrans);
        unit.SetUnitClassColor();
        unitObj.GetComponent<Unit>().sp.color = Color.white;
        unitObj.GetComponent<Unit>().transform.localRotation = Quaternion.Euler(Vector3.zero);

        FieldManager.Inst.ResetFieldState();
        FieldManager.Inst.ModifySpawnedUnitCount(1);

        unit.anim.SetTrigger("Attack");

        unit.visualGroup.transform.DOScale(Vector3.one, 0.4f).
                      SetEase(Ease.OutBack).
                      SetUpdate(true).
                      From(new Vector3(1.7f, 1.7f, 1.7f));

        if (DebuffManager.Inst.isUnitSilenceDebuff)
        {
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Slience);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Silence1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn2);
            unit.unitEvent.ApplySilencedUnit(5);
        }
        return unit;
    }

    public void ModifyCoinFreeSpawnCount(int count)
    {
        freeSpawnCount = Mathf.Max(freeSpawnCount + count, 0);
        if (freeSpawnCount > 0)
        {
            freeSpawnUI.SetActive(true);
            spawnUI.SetActive(false);
            freeSpawnCountText.text = $"무료 소환 {freeSpawnCount}회";
        }
        else
        {
            freeSpawnUI.SetActive(false);
            spawnUI.SetActive(true);
            freeSpawnCountText.text = "무료 소환 0회";
        }
    }

    public void AutoFillFixedUnit(UnitData data)
    {
        int emptyCount = FieldManager.Inst.GetEmptyUnitSlots().Count;

        for (int i = 0; i < emptyCount; i++)
        {
            GenerateUnit(data);
        }
    }

    public void AutoFillRandomUnit(List<UnitData> unitDatas)
    {
        int emptyCount = FieldManager.Inst.GetEmptyUnitSlots().Count;
        List<UnitData> datas = new List<UnitData>();

        for (int i = 0; i < emptyCount; i++)
        {
            datas.Add(unitDatas[Random.Range(0, unitDatas.Count)]);
        }

        for (int i = 0; i < emptyCount; i++)
        {
            UnitData nextData = datas[i]; GenerateUnit(nextData);
        }
    }

}