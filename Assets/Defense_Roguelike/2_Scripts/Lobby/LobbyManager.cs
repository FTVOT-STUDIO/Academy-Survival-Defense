using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Inst;

    [Header("# Lobby")]
    public GameObject lobbyUiCanvas;
    public GameObject lobbyUiGroup;
    [Space]
    public Image lobbyMainBackground;
    public ScrollingUIImage lobbyPatternOverlay;
    [Space]
    [Header("# Reward List")]
    [SerializeField] GameObject rewardListContent;



    void Awake()
    {
        Inst = this;
    }

}
