using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Inst;

    public bool isRestarting;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    public enum SceneType
    {
        Title,
        AppStartup,
        GoToLobby,
        RestartGame
    }

    [SerializeField] SceneType defaultSceneType;

    /// <summary>
    /// 인스펙터에서 지정한 기본 씬 타입으로 씬 변경
    /// </summary>
    public void ChangeScene()
    {
        ChangeScene(defaultSceneType);
    }

    /// <summary>
    /// 원하는 씬 타입을 코드로 직접 지정해서 씬 변경
    /// </summary>
    public void ChangeScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Title:
                SceneManager.LoadScene("Title_Scene");
                break;
            case SceneType.AppStartup:
                SceneManager.LoadScene("Startup_Loading_Scene");
                break;
            case SceneType.GoToLobby:
                SceneManager.LoadScene("Lobby_Scene");
                break;
            case SceneType.RestartGame:
                SceneManager.LoadScene("RestartPlay_Loading_Scene");
                break;
            default:
                Debug.LogWarning("Unknown scene type");
                break;
        }
    }
}
