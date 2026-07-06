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

    public void Clear()
    {
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