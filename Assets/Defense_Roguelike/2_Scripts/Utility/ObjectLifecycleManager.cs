using System.Collections;
using UnityEngine;

public class ObjectLifecycleManager : MonoBehaviour
{
    [SerializeField] float delay = 0;
    [SerializeField] bool setDelay = false;
    public enum ObjectType { None, Object, Effect }
    [SerializeField] ObjectType objectType = ObjectType.None;

    void OnEnable()
    {
        transform.localScale = Vector3.one;

        if (setDelay)
        {
            SetDelay(delay);
        }
    }

    public void Init(ObjectType objectType)
    {
        this.objectType = objectType;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent.transform);
        transform.localScale = Vector3.one;
    }

    public void SetDelay(float delay)
    {
        Invoke("DeActive", delay);
    }

    public void DeActive()
    {
        Transform targetParent;

        if (objectType == ObjectType.None)
            Debug.Log("오브젝트 부모 초기화가 되지 않았습니다.", gameObject);

        if (objectType == ObjectType.Object)
            targetParent = PoolManager.Inst.transform;
        else
            targetParent = EffectManager.Inst.transform;

        transform.SetParent(targetParent);
        gameObject.SetActive(false);
        CancelInvoke();
    }

    void OnDisable()
    {
        gameObject.SetActive(false);
    }



}
