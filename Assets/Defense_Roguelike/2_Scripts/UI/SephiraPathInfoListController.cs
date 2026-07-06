using UnityEngine;

public class SephiraPathInfoListController : MonoBehaviour
{
    [SerializeField] SephiraPathInfoItem[] pathInfoItems;
    [SerializeField] SephiraPathData[] sephiraPathDatas;
    UnitData unitData;
    [SerializeField] RectTransform pathInfoListcontent;

    void Awake()
    {
        for (int i = 0; i < pathInfoItems.Length; i++)
        {
            pathInfoItems[i].Init(sephiraPathDatas[i]);
        }
    }
    void OnEnable()
    {
        int index = System.Array.IndexOf(sephiraPathDatas, unitData.sephiraPathData);
        float ItemHeight = 250;

        int step = Mathf.Clamp(index - 2, 0, 4);
        float targetY = ItemHeight * step;

        pathInfoListcontent.anchoredPosition = new Vector2(pathInfoListcontent.anchoredPosition.x, targetY);
    }

    void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public void Init(UnitData data)
    {
        unitData = data;
        for (int i = 0; i < sephiraPathDatas.Length; i++)
        {
            pathInfoItems[i].selectedImage.SetActive(sephiraPathDatas[i] == unitData.sephiraPathData);
        }
    }
}
