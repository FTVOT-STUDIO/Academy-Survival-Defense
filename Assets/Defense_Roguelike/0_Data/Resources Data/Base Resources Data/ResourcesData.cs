using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesData", menuName = "ResourcesData")]
public class ResourcesData : ScriptableObject
{
    public enum ResourcesType { none, gold, gem, coin, darkMatteCoin, VoidTicket, Exp }
    public ResourcesType resourcesType;
    public int id;
    public string resourceName;
    public Sprite icon;
}
