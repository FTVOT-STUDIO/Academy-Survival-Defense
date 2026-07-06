using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Inst;
    public GameManager.GameMode curGameMode;
    public JointOperationsManager jointOperationsManager;

    void Awake()
    {
        Inst = this;
    }
}