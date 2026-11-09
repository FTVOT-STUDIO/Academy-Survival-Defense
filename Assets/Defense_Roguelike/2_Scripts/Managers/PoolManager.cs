using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public enum PrefabType
    {
        Bullet0, enemy0, enemy2, SnowWhiteLaser, Unit, SnowWhiteCircleLaser,
        Bullet2, Bomb, Enemy3, FloatingText, BossSlot, unitTabPrefab, FleshImage, StarLight,
        StateCardUI, RewardItem, GachaRewardItem, fieldTabPrefab, GevurahAirPlane
        , InGameResourceVisualizer, BarrierObject, ScorchedEarthRocket, ZoomojiTntBullet, ZoomojiAbilityBullet, MalkuthBlackholeUrp, MalkuthTransUrp, MalkuthAbilityOffUI, MalkuthMagicCircle, MalkuthBirdBullet, JOP_UnitList, JOP_SephiraList, JOP_FieldList, HodAbilityBullet, SpecialShield, InvulnerableShield, FieldAttackBoostMarker, GamchicothShield, UnitCombatStatList, TiferetURP, IrisWeapon, IgnisSkillHorizontalBullet, IgnisSkillVerticalBullet, damageText, IgnisTransManager, SayaCharacterManager
    }
    public static PoolManager Inst;
    [Header("# Prefabs")]
    public GameObject[] prefabs;
    [Header("# Pool Size")]
    public int size;
    List<GameObject>[] pools;
    readonly Dictionary<GameObject, List<GameObject>> effectPools = new Dictionary<GameObject, List<GameObject>>();
    [Header("# Bullet Genrator")]
    public BulletGenerator bulletGenerator;

    void Awake()
    {
        Inst = this;

        Init();
    }
    void Init()
    {
        pools = new List<GameObject>[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(PrefabType prefabType, Vector3 pos)
    {
        GameObject select = null;

        foreach (GameObject item in pools[(int)prefabType])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.transform.position = pos;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(prefabs[(int)prefabType], pos, Quaternion.identity, transform);
            pools[(int)prefabType].Add(select);
        }
        return select;
    }

    // 신규 캐릭터 이펙트는 프리팹 참조로 대여하여 씬의 enum 배열 순서에 의존하지 않습니다.
    public GameObject Get(GameObject prefab, Vector3 pos)
    {
        if (prefab == null) return null;
        if (!effectPools.TryGetValue(prefab, out var pool))
        {
            pool = new List<GameObject>();
            effectPools.Add(prefab, pool);
        }
        foreach (var item in pool)
        {
            if (item == null || item.activeSelf) continue;
            item.transform.SetParent(transform, false);
            item.transform.SetPositionAndRotation(pos, Quaternion.identity);
            item.transform.localScale = Vector3.one;
            item.SetActive(true);
            return item;
        }
        var created = Instantiate(prefab, pos, Quaternion.identity, transform);
        pool.Add(created);
        return created;
    }

    public void Clear()
    {
        foreach (var pool in effectPools.Values)
            foreach (var item in pool)
                if (item != null) Destroy(item);
        effectPools.Clear();
        foreach (List<GameObject> pool in pools)
        {
            foreach (GameObject obj in pool)
            {
                if (obj != null)
                    Destroy(obj);

            }
            pool.Clear();
        }

        Init();
    }
}