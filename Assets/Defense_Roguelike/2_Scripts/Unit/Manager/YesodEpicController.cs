using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YesodEpicController : MonoBehaviour
{
    public static YesodEpicController Inst;
    public GameObject shardPrefab;
    public Material hypnosisMaterial;
    public TMP_FontAsset font;
    public int Collected { get; private set; }
    public string CollectedPoem => string.Concat(new List<string>(YesodEpicText.Fragments).GetRange(0, Collected));
    public bool Ready => Collected == 100 && !cinematic && !truth;
    SephiraUnit owner;
    bool truth, cinematic, ownsFreeze, sessionStarted;
    float previousScale, previousVolume, transient;
    readonly List<GameObject> shards = new List<GameObject>();
    TMP_Text phrase, progress;
    RawImage spiral;
    CanvasGroup overlay;
    Material screenMaterial, auraMaterial;
    RenderTexture worldCapture;
    GameObject aura;

    public static bool OwnsGauge(SephiraUnit unit)
    {
        return Inst != null && Inst.owner == unit && !Inst.truth;
    }
    public static bool BlocksAbility(SephiraUnit unit)
    {
        if (!(unit.skillSO is YesodSkill skill) || skill.truthForm) return false;
        var controller = skill.GetController();
        if (controller == null) return true;
        controller.Bind(unit, false);
        return !controller.Ready;
    }
    public static void Notify(float chance)
    {
        if (Inst != null) Inst.TrySpawn(chance);
    }
    public void Bind(SephiraUnit unit, bool finalForm)
    {
        if (owner != unit) Collected = 0;
        owner = unit;
        truth = finalForm;
        if (overlay == null) BuildUI();
        if (aura == null)
        {
            aura = new GameObject("Truth Epic Seal");
            aura.transform.SetParent(transform, false);
            var renderer = aura.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 2, 2), Vector2.one * .5f, 2);
            auraMaterial = new Material(hypnosisMaterial);
            renderer.sharedMaterial = auraMaterial;
            renderer.sortingOrder = -1;
        }
        SyncGauge();
    }
    void OnEnable() { Inst = this; }
    public void SyncGauge()
    {
        if (owner == null || truth) return;
        float gauge = owner.maxAbllityCount * (Ready ? 1 : Mathf.Min(Collected, 99) / 100f);
        bool changed = !Mathf.Approximately(owner.abllityCount, gauge);
        owner.abllityCount = gauge;
        if (changed && Application.isPlaying) owner.UpdateAbilityBar();
    }
    void TrySpawn(float chance)
    {
        if (owner == null || truth || cinematic || GameManager.Inst == null || !GameManager.Inst.isGameStart || Collected + ActiveShards() >= 100) return;
        if ((chance < 1f && Random.value >= chance) || FieldManager.Inst.fieldSlots.Length == 0) return;
        Bounds bounds = new Bounds(FieldManager.Inst.fieldSlots[0].position, Vector3.zero);
        foreach (var slot in FieldManager.Inst.fieldSlots) bounds.Encapsulate(slot.position);
        Vector3 position = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), -1.5f);
        var shard = PoolManager.Inst.Get(shardPrefab, position);
        if (!shards.Contains(shard)) shards.Add(shard);
    }
    public bool PrepareLastShardForDebug()
    {
        if (owner == null || truth || cinematic || owner.isSkillActive || GameManager.Inst == null
            || !GameManager.Inst.isGameStart || GameManager.Inst.isPause || FieldManager.Inst == null
            || PoolManager.Inst == null || FieldManager.Inst.fieldSlots.Length == 0) return false;
        // 연타해도 기존 파편을 반환하고 마지막 파편 하나만 남깁니다.
        foreach (var shard in shards) if (shard != null) shard.SetActive(false);
        shards.Clear();
        Collected = 99;
        if (owner.sephiraUnitEvent != null) owner.sephiraUnitEvent.isAbilityReady = false;
        if (InGameUIManager.Inst != null && InGameUIManager.Inst.abilityReadyEffect != null)
            InGameUIManager.Inst.abilityReadyEffect.SetActive(false);
        SyncGauge();
        PresentFragment(98);
        TrySpawn(1f);
        return ActiveShards() == 1;
    }
    int ActiveShards()
    {
        int count = 0;
        foreach (var shard in shards) if (shard != null && shard.activeSelf) count++;
        return count;
    }
    public bool Collect()
    {
        if (owner == null || truth || cinematic || Collected >= 100 || !GameManager.Inst.isGameStart || GameManager.Inst.isPause) return false;
        int index = Collected++;
        SyncGauge();
        if (Collected == 100)
        {
            cinematic = true;
            StartCoroutine(FinalFragment());
        }
        else
        {
            PresentFragment(index);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
        }
        return true;
    }
    void Update()
    {
        if (owner == null || GameManager.Inst == null) return;
        // 초기화 중 isGameStart=false인 상태는 종료가 아니라 시작 전 대기입니다.
        if (!GameManager.Inst.isGameStart)
        {
            if (sessionStarted) gameObject.SetActive(false);
            return;
        }
        sessionStarted = true;
        if (!owner.gameObject.activeInHierarchy) { gameObject.SetActive(false); return; }
        SyncGauge();
        if (aura != null)
        {
            aura.transform.position = owner.transform.position;
            aura.transform.localScale = Vector3.one * (0.7f + Collected / 25 * 0.2f);
            auraMaterial.SetFloat("_Complexity", 2 + Collected / 25 * 2);
            aura.SetActive(Collected >= 25);
        }
        if (cinematic || GameManager.Inst.isPause) return;
        transient -= Time.deltaTime;
        if (transient <= 0) overlay.alpha = Mathf.MoveTowards(overlay.alpha, 0, Time.deltaTime * 4);
    }
    void PresentFragment(int index)
    {
        phrase.text = YesodEpicText.Fragments[index].Trim();
        progress.text = $"진리의 서사시  {index + 1}/100";
        transient = 0.65f;
        spiral.rectTransform.sizeDelta = Vector2.one * ((index + 1) % 25 == 0 ? 620 : 150);
        screenMaterial.SetFloat("_Power", .3f);
        spiral.color = Color.white;
        screenMaterial.SetFloat("_Fracture", index % 25 == 0 && index > 0 ? 0.5f : 0);
        spiral.rectTransform.anchoredPosition = new Vector2(0, 50);
        overlay.alpha = 1;
    }
    IEnumerator FinalFragment()
    {
        previousScale = Time.timeScale;
        previousVolume = AudioListener.volume;
        ownsFreeze = true;
        Time.timeScale = 0;
        AudioListener.volume = previousVolume * .12f;
        if (worldCapture == null || worldCapture.width != Screen.width || worldCapture.height != Screen.height)
        {
            if (worldCapture != null) { worldCapture.Release(); Destroy(worldCapture); }
            worldCapture = new RenderTexture(Screen.width, Screen.height, 0);
        }
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshotIntoRenderTexture(worldCapture);
        screenMaterial.SetTexture("_WorldTex", worldCapture);
        yield return new WaitForSecondsRealtime(.12f);
        RestoreFreeze();
        yield return Shot("One of a menace", 1.1f, 1, true);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.GlassBreak);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo6);
        yield return Shot("One of a menace", .5f, 1, true, 1);
        yield return Shot("", .28f, 0, true, 1);
        yield return Shot("One of verity", .65f, 0, false);
        yield return Shot("One of freedom", .65f, 0, false);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Epic);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility4);
        yield return Shot("One of a menace", 1.0f, 1, true);
        cinematic = false;
        overlay.alpha = 0;
        SyncGauge();
        owner.sephiraUnitEvent.OnAbilityCharged?.Invoke();
    }
    public void CastTruthUltimate()
    {
        if (cinematic) return;
        cinematic = true;
        StartCoroutine(TruthUltimate());
    }
    IEnumerator TruthUltimate()
    {
        yield return Shot("One of freedom", .4f, .4f, false);
        yield return Shot("One of verity", .4f, .7f, false);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo6);
        yield return Shot("One of a menace", .8f, 1, true, 1);
        // 세부 전투 계수가 없는 최종 궁극기는 기존 피해 경로로 처리합니다.
        foreach (var enemy in new List<Enemy>(EnemySpawnManager.Inst.spawnedEnemies))
            if (enemy != null && enemy.isLive)
                enemy.OnDamage(owner.unitData.attribute.attributeType, owner.damageAmout * ((YesodSkill)owner.skillSO).ultimateDamageMultiplier * owner.abilityMultiplier, 0, 1, 1, 1, owner.attributeDamageMultiplier, UnitUtility.AttackType.Ability, owner.gameObject);
        cinematic = false;
        overlay.alpha = 0;
    }
    IEnumerator Shot(string text, float seconds, float power, bool jitter, float fracture = 0)
    {
        phrase.text = text;
        progress.text = truth ? "그것은 자유이자 진실 그리고 위협" : "진리의 서사시";
        spiral.rectTransform.sizeDelta = Vector2.one * 2200;
        overlay.alpha = 1;
        screenMaterial.SetFloat("_Power", power);
        screenMaterial.SetFloat("_Fracture", fracture);
        float elapsed = 0;
        while (elapsed < seconds)
        {
            float phase = elapsed / seconds;
            screenMaterial.SetFloat("_Collapse", fracture * phase);
            spiral.color = new Color(1, 1, 1, power > 0 ? 1 : 0);
            // 암전 단계에도 문구는 원문 그대로 유지합니다.
            background.color = new Color(0, 0, 0, power == 0 ? 1 : .25f);
            phrase.rectTransform.anchoredPosition = jitter ? new Vector2(Mathf.Sin(elapsed * 87) * 7, Mathf.Sin(elapsed * 117) * 3) : Vector2.zero;
            spiral.rectTransform.localScale = Vector3.one * (1 - fracture * phase * .8f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        spiral.rectTransform.localScale = Vector3.one;
        screenMaterial.SetFloat("_Collapse", 0);
        background.color = Color.clear;
    }
    Image background;
    void BuildUI()
    {
        var canvasObject = new GameObject("Truth Epic UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 300;
        var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080);
        overlay = canvasObject.GetComponent<CanvasGroup>(); overlay.blocksRaycasts = false; overlay.alpha = 0;
        var shade = new GameObject("Blackout", typeof(RectTransform), typeof(Image)); shade.transform.SetParent(canvasObject.transform, false);
        background = shade.GetComponent<Image>(); background.color = Color.clear; background.raycastTarget = false;
        var br = shade.GetComponent<RectTransform>(); br.anchorMin = Vector2.zero; br.anchorMax = Vector2.one; br.offsetMin = br.offsetMax = Vector2.zero;
        var image = new GameObject("Hypnosis", typeof(RectTransform), typeof(RawImage)); image.transform.SetParent(canvasObject.transform, false);
        spiral = image.GetComponent<RawImage>(); screenMaterial = new Material(hypnosisMaterial); spiral.material = screenMaterial; spiral.raycastTarget = false;
        phrase = MakeText(canvasObject.transform, 34, Vector2.zero);
        progress = MakeText(canvasObject.transform, 18, new Vector2(0, -100));
    }
    TMP_Text MakeText(Transform parent, float size, Vector2 position)
    {
        var obj = new GameObject("Epic Text", typeof(RectTransform)); obj.transform.SetParent(parent, false);
        var text = obj.AddComponent<TextMeshProUGUI>(); text.font = font; text.fontSize = size; text.alignment = TextAlignmentOptions.Center; text.raycastTarget = false;
        text.rectTransform.sizeDelta = new Vector2(1200, 180); text.rectTransform.anchoredPosition = position;
        text.outlineColor = Color.black; text.outlineWidth = .2f;
        return text;
    }
    void RestoreFreeze()
    {
        if (!ownsFreeze) return;
        if (GameManager.Inst == null || GameManager.Inst.isGameStart)
            Time.timeScale = GameManager.Inst != null && GameManager.Inst.isPause ? 0 : previousScale;
        AudioListener.volume = previousVolume;
        ownsFreeze = false;
    }
    void OnDisable()
    {
        StopAllCoroutines(); RestoreFreeze();
        foreach (var shard in shards) if (shard != null) shard.SetActive(false);
        shards.Clear(); owner = null; Collected = 0; cinematic = false; sessionStarted = false;
        if (overlay != null) overlay.alpha = 0;
        if (aura != null) aura.SetActive(false);
        if (Inst == this) Inst = null;
    }
    void OnDestroy()
    {
        if (worldCapture != null) { worldCapture.Release(); Destroy(worldCapture); }
        if (screenMaterial != null) Destroy(screenMaterial);
        if (auraMaterial != null) Destroy(auraMaterial);
        if (aura != null && aura.GetComponent<SpriteRenderer>().sprite != null) Destroy(aura.GetComponent<SpriteRenderer>().sprite);
    }
}
