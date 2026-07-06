using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Range(1, 10)]
    [SerializeField] float swipeThreshold;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] Transform contentTrans;
    [SerializeField] RectTransform canvasScaler;
    [SerializeField] Slider tabSilder;
    [SerializeField] RectTransform tabRect;
    [Space]
    [SerializeField] RectTransform[] contentRect;
    [SerializeField] GameObject[] contentObject;
    [SerializeField] RectTransform[] btnRect;
    [SerializeField] RectTransform[] btnImageRect;
    [SerializeField] Sprite[] btnUnClickSprite;
    [SerializeField] Sprite[] btnClickSprite;
    const int SIZE = 5;
    float[] pos = new float[SIZE];
    float distance;
    float targetPos;
    float curPos;
    bool isDrag;
    int targetIndex;

    Vector2 prevCanvasSize;

    void Start()
    {
        for (int i = 0; i < SIZE; i++)
        {
            contentRect[i].sizeDelta = new Vector2(canvasScaler.sizeDelta.x, canvasScaler.sizeDelta.y - tabRect.sizeDelta.y);
        }


        distance = 1f / (SIZE - 1);
        for (int i = 0; i < SIZE; i++)
        {
            pos[i] = distance * i;
        }

        GetComponent<ScrollRect>().horizontalScrollbar.value = 0.5f;
        scrollbar.value = 0.5f;
        tabSilder.value = 0.5f;

        TabClick(2);

        // Force canvas update
        Canvas.ForceUpdateCanvases();
    }

    void Update()
    {
        Vector2 currentCanvasSize = canvasScaler.sizeDelta;
        if (currentCanvasSize != prevCanvasSize)
        {
            prevCanvasSize = currentCanvasSize;
            for (int i = 0; i < SIZE; i++)
            {
                contentRect[i].sizeDelta = new Vector2(canvasScaler.sizeDelta.x, canvasScaler.sizeDelta.y - tabRect.sizeDelta.y);
            }
        }

        tabSilder.value = scrollbar.value;

        if (!isDrag)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, 0.15f);
            for (int i = 0; i < SIZE; i++) btnRect[i].sizeDelta = new Vector2(i == targetIndex ? 360 : 180, btnRect[i].sizeDelta.y);
        }


        UpdateContentActiveState();
        UpdateButtonVisuals();

    }

    void UpdateContentActiveState()
    {
        float currentValue = scrollbar.value;
        float visibilityRange = distance * 0.9f;

        for (int i = 0; i < SIZE; i++)
        {
            bool shouldBeActive = Mathf.Abs(currentValue - pos[i]) <= visibilityRange;
            if (contentObject[i].activeSelf != shouldBeActive)
                contentObject[i].SetActive(shouldBeActive);
        }
    }

    void UpdateButtonVisuals()
    {
        for (int i = 0; i < SIZE; i++)
        {
            // btnRect[i].sizeDelta = new Vector2(i == targetIndex ? canvasWidth / (SIZE + 1) * 2 : canvasWidth / (SIZE + 1), btnRect[i].sizeDelta.y);

            Vector3 btnTargetPos = btnRect[i].anchoredPosition3D;
            Vector3 btnTargetScale = Vector3.one;
            Image btnTargetImage = btnImageRect[i].GetComponent<Image>();
            bool textActive = false;

            if (i == targetIndex)
            {
                btnTargetPos.y = -23f;
                btnTargetScale = new Vector3(1.2f, 1.2f, 1);
                btnTargetImage.sprite = btnClickSprite[i];
                textActive = true;
            }
            else
            {
                btnTargetImage.sprite = btnUnClickSprite[i];
            }

            btnImageRect[i].anchoredPosition3D = Vector3.Lerp(btnImageRect[i].anchoredPosition3D, btnTargetPos, 0.25f);
            btnImageRect[i].localScale = Vector3.Lerp(btnImageRect[i].localScale, btnTargetScale, 0.25f);
            btnImageRect[i].transform.GetChild(0).gameObject.SetActive(textActive);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        curPos = SetPos();

    }

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        targetPos = SetPos();

        if (curPos == targetPos)
        {
            if (eventData.delta.x > swipeThreshold && curPos - distance >= 0)
            {
                targetIndex--;
                targetPos = curPos - distance;
            }
            else if (eventData.delta.x < -swipeThreshold && curPos + distance <= 1.01f)
            {
                targetIndex++;
                targetPos = curPos + distance;
            }
        }
    }

    void VerticalScrollUp()
    {
        for (int i = 0; i < contentTrans.childCount; i++)
        {
            Transform second = contentTrans.GetChild(i);
            for (int j = 0; j < second.childCount; j++)
            {
                Transform third = second.GetChild(j);

                ScrollScript scrollScript = third.GetComponent<ScrollScript>();
                if (scrollScript != null)
                {
                    Scrollbar scrollbar = third.GetComponentInChildren<Scrollbar>();
                    if (scrollbar != null)
                    {
                        StartCoroutine(ScrollToTop(scrollbar));
                    }
                }
            }
        }
    }

    IEnumerator ScrollToTop(Scrollbar scrollbar)
    {
        float startValue = scrollbar.value;
        float targetValue = 1f;
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = Mathf.Clamp01(t);

            float smooth = 1 - Mathf.Pow(1 - t, 3);

            scrollbar.value = Mathf.Lerp(startValue, targetValue, smooth);
            yield return null;
        }

        scrollbar.value = targetValue;
    }


    float SetPos()
    {
        float minDist = float.MaxValue;
        int closestIndex = targetIndex;

        for (int i = 0; i < SIZE; i++)
        {
            float dist = Mathf.Abs(scrollbar.value - pos[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }

        targetIndex = closestIndex;
        return pos[closestIndex];
    }


    public void TabClick(int n)
    {
        curPos = SetPos();
        targetIndex = n;
        targetPos = pos[n];

        VerticalScrollUp();

        float currentValue = scrollbar.value;
        float visibilityRange = distance * 0.9f;

        for (int i = 0; i < SIZE; i++)
        {
            bool shouldBeActive = Mathf.Abs(currentValue - pos[i]) <= visibilityRange;

            if (contentObject[i].activeSelf != shouldBeActive)
                contentObject[i].SetActive(shouldBeActive);
        }
    }
}
