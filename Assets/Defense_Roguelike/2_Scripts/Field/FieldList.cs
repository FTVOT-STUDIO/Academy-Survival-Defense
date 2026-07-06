using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldList : MonoBehaviour
{
    public FieldTabUIInitializer.InfoType infoType;
    [HideInInspector] public FieldData fieldData;
    [Space]
    public TMP_Text nameText;
    public TMP_Text levelText;
    [Space(10)]
    public Image portrait;

    public void Init(FieldData fieldData)
    {
        this.fieldData = fieldData;

        if (portrait != null)
            portrait.sprite = fieldData.fieldPortrait;
        if (nameText != null)
            nameText.text = fieldData.fieldName;
        if (levelText != null)
            levelText.text = "Lv " + fieldData.fieldLevel;

        if (GetComponentInChildren<Button>() != null)
            EquipmentManager.Inst.AddFieldButtonClickListener(GetComponentInChildren<Button>(), fieldData, infoType);

        transform.DOScale(Vector3.one, 0.4f).
           SetEase(Ease.OutBack).
           SetUpdate(true).
           From(new Vector3(1.25f, 1.25f, 1.25f));
    }


}
