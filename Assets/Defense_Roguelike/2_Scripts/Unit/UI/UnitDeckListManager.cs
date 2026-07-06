using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitDeckListManager : MonoBehaviour
{
    public GameObject[] unitList;

    public int deckIndex;

    public bool isSephira;

    void Awake()
    {
        unitList = new GameObject[transform.childCount];

        for (int i = 0; i < unitList.Length; i++)
        {
            unitList[i] = transform.GetChild(i).gameObject;
            unitList[i].GetComponent<UnitList>().dataIndex = i;
        }

        isSephira = unitList.Length == 1;
    }

    void Start()
    {
        UnitListInit();
        SephiraListInit();
    }

    public void UnitListInit()
    {
        UnitList[] unitLists = GetComponentsInChildren<UnitList>();

        for (int i = 0; i < unitLists.Length; i++)
        {
            unitLists[i].Init(DeckManager.Inst.deckLists[deckIndex].unitDatas[unitLists[i].dataIndex]);
        }
    }

    public void SephiraListInit()
    {
        UnitList unitList = GetComponentInChildren<UnitList>();
        unitList.Init(DeckManager.Inst.deckLists[deckIndex].sephiraData);
    }
}
