using UnityEngine;

public class ResetScaleOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        transform.localScale = Vector3.one;
    }
}
