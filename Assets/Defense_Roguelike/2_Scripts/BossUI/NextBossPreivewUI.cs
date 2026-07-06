using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NextBossPreivewUI : MonoBehaviour
{
    public enum UIType { Intro, Preview, Alert }
    public UIType uiType;
    [SerializeField] List<GameObject> bossSlots;
    [SerializeField] Text nextBossSpawnWaveText;
    [SerializeField] TMP_Text curWaveText;
    [SerializeField] RectTransform previewUiRect;
    [Space]
    [SerializeField] LayoutGroup bossLayout;
    [SerializeField] Sprite bossPreivewSilhouetteSprtie;
    [SerializeField] GameObject bossSlotPrefab;
    [SerializeField] Transform bossSlotGroup;

    void Awake()
    {
        if (uiType == UIType.Intro)
        {
            for (int i = 0; i < 2; i++)
            {
                var slot = Instantiate(bossSlotPrefab, bossSlotGroup.transform, false);
                bossSlots.Add(slot);
            }
        }

    }

    void OnEnable()
    {
        switch (uiType)
        {
            case UIType.Preview:
            case UIType.Alert:
                Init();
                break;
        }
    }

    public void Init()
    {
        foreach (GameObject objects in bossSlots)
        {
            objects.SetActive(false);
        }

        for (int i = 0; i < EnemySpawnManager.Inst.nextBossData.Count; i++)
        {
            EnemyData bossData = EnemySpawnManager.Inst.nextBossData[i].enemyData;
            bossSlots[i].SetActive(true);

            Sprite portrait;
            string name;

            if (EnemySpawnManager.Inst.nextBossData[i].onSilhouette)
            {
                portrait = bossPreivewSilhouetteSprtie;
                name = "???";
            }
            else
            {
                portrait = bossData.portrait[0];
                name = bossData.enemyName;
            }
            bossSlots[i].GetComponent<Image>().sprite = portrait;

            switch (uiType)
            {
                case UIType.Intro:
                case UIType.Alert:
                    bossSlots[i].GetComponent<BossSlot>().nameText.text = name;
                    break;
            }
        }

        switch (uiType)
        {
            case UIType.Preview:
                int currentWave = GameManager.Inst.wave;
                int nextBossWave = ((currentWave / 10) + 1) * 10;
                nextBossSpawnWaveText.text = "WAVE " + nextBossWave;
                if (EnemySpawnManager.Inst.nextBossData.Count > 1)
                    previewUiRect.sizeDelta = new Vector2(400, previewUiRect.sizeDelta.y);
                else
                    previewUiRect.sizeDelta = new Vector2(300, previewUiRect.sizeDelta.y);
                break;
            case UIType.Alert:
                curWaveText.text = "WAVE " + GameManager.Inst.wave.ToString();
                break;
        }
        GameUIManager.Inst.RefreshLayoutOnce(bossLayout);
    }

    void OnDisable()
    {
        for (int i = 0; i < bossSlots.Count; i++)
        {
            bossSlots[i].SetActive(false);
        }
    }
}
