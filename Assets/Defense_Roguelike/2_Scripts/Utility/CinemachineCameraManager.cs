using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraManager : MonoBehaviour
{
    public static CinemachineCamera cinemachineCamera;

    void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }


    public static void SetCameraPosition(float size, float posY)
    {
        cinemachineCamera.Lens.OrthographicSize = size;
        cinemachineCamera.transform.position = new Vector3(cinemachineCamera.transform.position.x, posY, cinemachineCamera.transform.position.z);
    }

}
