using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyData", menuName = "DifficultyData/DifficultyData")]
public class DifficultyData : ScriptableObject, IMapActivationHandler
{
    public enum DifficultyType { Normal, Hard, Extreme }
    public DifficultyType difficultyType;
    public int difficultyId;
    public Sprite illust;
    public Sprite icon;
    public Material overlayPatternMaterial;
    public Color chapterColor;
    public Color backGroundColor;
    public Color lineColor;
    public Gradient gradient;
    [Space]
    public string difficultyName;
    public string chapterName;
    [TextArea]
    public string popupMessage;
    [Space]
    [TextArea]
    public string desc;
    public GameObject mapPrefab;

    public void ActivateMap()
    {
        Instantiate(mapPrefab, GameManager.Inst.mapManager.transform);
    }
}
