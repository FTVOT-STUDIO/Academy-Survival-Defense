using UnityEngine;

public class DataLoadManager : MonoBehaviour
{
    public static DataLoadManager Inst;
    public bool isSceneInitialized;
    void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
