using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// pixelfx로 제작한 프레임을 재사용하며 피해 판정은 기존 전투 경로로 처리합니다.
public class ForeSkillEffect : MonoBehaviour
{
    public Material material;
    public PixelFxClip chargeClip, resultClip, riftClip;
    public uint Lease { get; private set; }
    PixelFxPlayer pixels;
    LineRenderer bond;
    Transform source, target;
    IUnitSkill ownerSkill;
    Unit owner;
    float elapsed, duration, scale;
    Color tint;
    int mode;

    void Awake()
    {
        var child = new GameObject("Pixel FX");
        child.transform.SetParent(transform, false);
        pixels = child.AddComponent<PixelFxPlayer>();
        pixels.material = material;
        var thread = new GameObject("Affection Thread");
        thread.transform.SetParent(transform, false);
        bond = thread.AddComponent<LineRenderer>();
        bond.sharedMaterial = material;
        bond.useWorldSpace = true;
        bond.positionCount = 20;
        bond.startWidth = bond.endWidth = 0.018f;
        bond.sortingOrder = 45;
        bond.startColor = new Color(0.7f, 0.4f, 0.85f, 0.65f);
        bond.endColor = new Color(0.95f, 0.7f, 1, 0.7f);
        bond.enabled = false;
    }

    public static ForeSkillEffect Play(GameObject prefab, Vector3 position, int style, Color color, float size, float seconds)
    {
        if (PoolManager.Inst == null || prefab == null) return null;
        var fx = PoolManager.Inst.Get(prefab, position).GetComponent<ForeSkillEffect>();
        fx.Begin(style, color, size, seconds);
        return fx;
    }

    public void Begin(int style, Color color, float size, float seconds)
    {
        if (pixels == null) Awake();
        Lease++;
        mode = style;
        tint = color;
        scale = size;
        duration = seconds;
        elapsed = 0;
        float width = Mathf.Clamp(size, 0.4f, style == 4 ? 1.7f : 1.15f);
        PixelFxClip clip = style == 4 ? riftClip : style == 2 ? resultClip : chargeClip;
        // 텍스처의 팔레트를 살리고 색상은 약하게만 보정합니다.
        Color palette = style == 4 ? Color.white : Color.Lerp(Color.white, color, 0.35f);
        pixels.Play(clip, style == 0 ? 0.55f : width, seconds, style != 2 && style != 4, palette);
        bond.enabled = style == 0;
    }

    public static GameObject CreateBond(GameObject prefab, Transform from, Transform to)
    {
        var fx = Play(prefab, from.position, 0, new Color(0.85f, 0.35f, 1), 0.55f, float.PositiveInfinity);
        if (fx == null) return null;
        fx.source = from;
        fx.target = to;
        return fx.gameObject;
    }

    public static void CastCurse(Unit caster, JulesDeForeSkill data)
    {
        var fx = Play(data.effectPrefab, caster.transform.position, 1, new Color(0.8f, 0.4f, 1), 1.2f, float.PositiveInfinity);
        if (fx == null) return;
        fx.owner = caster;
        fx.ownerSkill = caster.unitSkill;
        fx.StartCoroutine(fx.Curse(data));
    }

    IEnumerator Curse(JulesDeForeSkill data)
    {
        double damage = owner.damageAmout * data.curseDamageMultiplier * owner.skillMultiplier;
        var attribute = owner.unitData.attribute.attributeType;
        float penetration = owner.attributeResistancePenetration;
        float attributeMultiplier = owner.attributeDamageMultiplier;
        Vector3 origin = owner.transform.position;
        var hit = new HashSet<Enemy>();
        while (damage > 0.01 && GameManager.Inst != null && GameManager.Inst.isGameStart)
        {
            Enemy enemy = NearestEnemy(origin, hit);
            if (enemy == null) break;
            hit.Add(enemy);
            // 자식 표식은 적이 비활성화되는 순간 꺼져 풀에서 재등장한 적을 추적하지 않습니다.
            var lifetime = enemy.GetComponent<ForeCurseLifetime>() ?? enemy.gameObject.AddComponent<ForeCurseLifetime>();
            int generation = lifetime.generation;
            target = enemy.transform;
            origin = enemy.transform.position;
            elapsed = 0;
            duration = data.curseDelay;
            AudioManager.Inst?.PlaySfx(AudioManager.Sfx.HeartBeat);
            float wait = 0;
            while (wait < data.curseDelay && lifetime.generation == generation)
            {
                if (GameManager.Inst == null || !GameManager.Inst.isGameStart) break;
                if (!GameManager.Inst.isPause) wait += Time.deltaTime;
                yield return null;
            }
            bool valid = GameManager.Inst != null && GameManager.Inst.isGameStart && lifetime.generation == generation && enemy.isLive && enemy.gameObject.activeInHierarchy;
            if (!valid) break;
            Play(data.effectPrefab, origin, 4, Color.white, 1.55f, 0.65f);
            AudioManager.Inst?.PlaySfx(AudioManager.Sfx.TiferetAbility4);
            bool killed = false;
            // 정규 피해 경로의 사망 콜백으로 실제 처치에 성공했을 때만 전이합니다.
            var attribution = owner != null && owner.unitSkill == ownerSkill ? owner.gameObject : null;
            yield return enemy.DamageCoroutine(attribute, damage, 0, 1, 0, penetration, attributeMultiplier,
            UnitUtility.AttackType.Skill, attribution, () => killed = true);
            if (!killed) break;
            damage *= data.curseTransferMultiplier;
            scale = Mathf.Max(0.8f, scale * 0.9f);
        }
        gameObject.SetActive(false);
    }

    static Enemy NearestEnemy(Vector3 origin, HashSet<Enemy> excluded)
    {
        Enemy result = null;
        float distance = float.PositiveInfinity;
        foreach (var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            if (!enemy.isLive || excluded.Contains(enemy)) continue;
            float current = (enemy.transform.position - origin).sqrMagnitude;
            if (current >= distance) continue;
            distance = current;
            result = enemy;
        }
        return result;
    }

    void Update()
    {
        if (GameManager.Inst != null && GameManager.Inst.isPause) return;
        elapsed += Time.deltaTime;
        if (mode == 0)
        {
            if (source == null || target == null || !source.gameObject.activeInHierarchy || !target.gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
                return;
            }
            transform.position = target.position + Vector3.up * 0.2f;
            for (int i = 0; i < bond.positionCount; i++)
            {
                float t = i / (float)(bond.positionCount - 1);
                Vector3 point = Vector3.Lerp(source.position, target.position, t) + Vector3.up * (0.15f + Mathf.Sin(t * Mathf.PI) * 0.12f);
                bond.SetPosition(i, point);
            }
        }
        else if (target != null) transform.position = target.position + Vector3.up * 0.2f;
        if (mode != 0 && mode != 1 && elapsed >= duration) gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        source = target = null;
        owner = null;
        ownerSkill = null;
        elapsed = 0;
        if (bond != null) bond.enabled = false;
    }
}

public class ForeCurseLifetime : MonoBehaviour
{
    public int generation;
    void OnDisable() { generation++; }
}
