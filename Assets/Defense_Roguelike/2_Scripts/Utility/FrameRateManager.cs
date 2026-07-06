using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [SerializeField] int targetFrame;

    void Awake()
    {
        Application.targetFrameRate = targetFrame;
    }
}
