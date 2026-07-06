using UnityEngine;

public class GlobalManagerRoot : MonoBehaviour
{
    public static GlobalManagerRoot Instance;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 자신을 삭제해야 함!
        }

    }
}
