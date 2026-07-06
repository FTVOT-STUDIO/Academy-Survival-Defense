using UnityEngine;

[CreateAssetMenu(fileName = "JointOperationData", menuName = "JointOperationData/JointOperationData")]
public class JointOperationData : ScriptableObject, IMapActivationHandler
{
    public enum OpertaionType { Metatron, Chokmah, Hod }
    public OpertaionType operationType;
    public int operationId;
    public string jointOperationName;
    [TextArea] public string JointOperationDesc;
    public EnemyData jointOperationBossData;
    public Sprite titleSprite;
    public GameObject mapPrefab;

    public void ActivateMap()
    {
        Instantiate(mapPrefab, GameManager.Inst.mapManager.transform);
    }
}
