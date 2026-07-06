using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Inst;
    public GameObject nickNameInputField;

    void Awake()
    {
        Inst = this;
    }

}
