using UnityEngine;

public interface IMapActivationHandler
{
    void ActivateMap();
}

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject normalMap;
    [SerializeField] GameObject hardMap;
    [SerializeField] GameObject hardVolume;
    [SerializeField] GameObject jop_MetatronMap;

    public void Init()
    {
        switch (GameManager.Inst.gameMode)
        {
            case GameManager.GameMode.Normal:
                if (DifficultyManager.Inst.curDifficultyData is IMapActivationHandler handler)
                {
                    handler.ActivateMap();
                }
                break;
            case GameManager.GameMode.JointOperation:
                if (GameModeManager.Inst.jointOperationsManager.curJointOperationData is IMapActivationHandler handler1)
                {
                    handler1.ActivateMap();
                }

                break;
        }
    }
}
