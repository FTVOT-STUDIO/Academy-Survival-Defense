using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitList : MonoBehaviour
{
    public UnitTabUIInitializer.InfoType infoType;
    [SerializeField] float portraitWidth;
    [SerializeField] float portraitHeight;
    [SerializeField] float portraitPosX;
    [SerializeField] float portraitPosY;
    [Space]
    public int dataIndex;
    public UnitData unitData;

    public TMP_Text nameText;
    public TMP_Text classTypeText;
    public TMP_Text levelText;
    [Space(10)]
    public Image portrait;
    public Image portraitShadow;
    public Image classImage;
    [SerializeField] Image attributeIcon;
    public GameObject equipArrowImage;


    public void Init(UnitData unitData)
    {
        this.unitData = unitData;

        if (unitData == null)
        {
            Debug.LogWarning("Data null, refresh!");
            DataManager.Inst.RefreshDeckList();
        }

        if (nameText != null)
            nameText.text = unitData.unitName;
        if (classTypeText != null)
            classTypeText.text = unitData.classToKorean[unitData.unitClass] + "/" + unitData.TypeToKorean(unitData.type);
        if (levelText != null)
            levelText.text = "Lv" + unitData.cardLevel;
        if (portrait != null)
        {
            portrait.sprite = unitData.portrait;
            RectTransform portraitRect = portrait.GetComponent<RectTransform>();
            portraitRect.sizeDelta = new Vector2(portraitWidth * unitData.portraitScaleFactor, portraitHeight * unitData.portraitScaleFactor);
            portraitRect.anchoredPosition = new Vector2((1 + portraitPosX) * unitData.portraitPosXFactor, (1 + portraitPosY) * unitData.portraitPosYFactor);
        }

        if (portraitShadow != null)
            portraitShadow.color = unitData.classColorType[unitData.unitClass];
        if (classImage != null)
            classImage.color = unitData.classColorType[unitData.unitClass];
        if (attributeIcon != null)
            attributeIcon.sprite = unitData.attribute.icon;

        if (GetComponentInChildren<Button>() != null)
            EquipmentManager.Inst.AddUnitButtonClickListener(GetComponentInChildren<Button>(), unitData, infoType);

        transform.DOScale(Vector3.one, 0.4f).
                SetEase(Ease.OutBack).
                SetUpdate(true).
                From(new Vector3(1.25f, 1.25f, 1.25f));

    }

    public void SelectDataInit()
    {
        if (EquipmentManager.Inst.unitTab.activeInHierarchy)
        {
            SelectUnit();
        }
        if (EquipmentManager.Inst.sephiraTab.activeInHierarchy)
        {
            SelectSephira();
        }
    }

    public void SelectUnit()
    {
        if (DeckManager.Inst.isSelected)
        {
            DeckManager deck = DeckManager.Inst;
            print("유닛 편성 왼료");

            UnitData curUnit = unitData;
            Debug.Log("교체된 유닛 : " + curUnit.unitName);
            Debug.Log("배치된 유닛 : " + DeckManager.Inst.selectData.unitName);
            int curIndex = 0;

            for (int i = 0; i < 5; i++)
            {
                if (DeckManager.Inst.selectData == DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].unitDatas[i])
                {
                    curIndex = i;
                    curUnit.isEquipped = true;
                    break;
                }
                else
                {
                    curUnit.isEquipped = false;
                    curIndex = dataIndex;
                }
            }


            DeckManager.Inst.selectData.isEquipped = true;

            DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].unitDatas[curIndex] = curUnit;
            DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].unitDatas[dataIndex] = DeckManager.Inst.selectData;

            DeckManager.Inst.curDeckUnitDatas[curIndex] = curUnit;
            DeckManager.Inst.curDeckUnitDatas[dataIndex] = DeckManager.Inst.selectData;

            DeckManager.Inst.InitializeAllDeckLists();

            EquipmentManager.Inst.UpdateAllEquippedStatus();

            // EquipmentManager.Inst.InitAllUnitAndFieldLists();
            // EquipmentManager.Inst.RefreshAllUnitAndFieldLists();
            deck.factionBonusCalculator.
        SetFactionBonusInfo(deck.deckLists[deck.deckListIndex].unitDatas, deck.deckLists[deck.deckListIndex].sephiraData, FactionBonusCalculator.FactionBonusView.Normal);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.select5);
        }
    }
    public void SelectSephira()
    {
        if (DeckManager.Inst.isSelected)
        {
            DeckManager deck = DeckManager.Inst;
            print("유닛 편성 왼료");

            UnitData curUnit = unitData;

            curUnit.isEquipped = false;

            DeckManager.Inst.selectData.isEquipped = true;

            Debug.Log("cur unit: " + curUnit.unitName);
            Debug.Log("select Unit : " + DeckManager.Inst.selectData.unitName);

            DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].sephiraData = curUnit;
            DeckManager.Inst.deckLists[DeckManager.Inst.deckListIndex].sephiraData = DeckManager.Inst.selectData;

            DeckManager.Inst.curDeckSephiraData = curUnit;
            DeckManager.Inst.curDeckSephiraData = DeckManager.Inst.selectData;

            DeckManager.Inst.InitializeAllDeckLists();

            EquipmentManager.Inst.UpdateAllEquippedStatus();

            //    EquipmentManager.Inst.InitAllUnitAndFieldLists();
            // EquipmentManager.Inst.RefreshAllUnitAndFieldLists();
            deck.factionBonusCalculator.
       SetFactionBonusInfo(deck.deckLists[deck.deckListIndex].unitDatas, deck.deckLists[deck.deckListIndex].sephiraData, FactionBonusCalculator.FactionBonusView.Normal);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Select1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.select5);
        }
    }
}
