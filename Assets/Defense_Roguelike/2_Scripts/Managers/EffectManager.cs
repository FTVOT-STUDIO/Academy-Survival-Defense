using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Inst;
    public Light2D light2D;

    public enum PrefabType
    {
        StunEffect, KetherSkillEffect, SpawnEffect, EnemyHitEffect, AttackSpeedEffect, LunaEffect,
        ketherEffect, HealEffect, DeltaEffect, SheldEffect, BombEffect, EnemySpawnEffect, EnemyAttackEffect, InstDeathEffect, TsubasaShiraiEffect, ChokmahEffect,
        ManaRegenEffect, RandomizeEffect, Slow, Dot, Weakness,
        BryceEffect, ParasiticEffect, SnowWhiteAttackEffect0, SnowWhiteTransEffect, SnowWhiteAttackEffec1, ZetaEffect, GevurahTargetEffect,
        Explosion, ShopItemGetEffect, SyntaxErrorEffect, HomeRunEffect, KohakuSwordingEffect, KohakuAblityEffect,
        JinIllustratorWaitEffect, JinIllustratorIdeaEffect, JinIllustratorSuccessEffect, JinIllustratorFailEffect,
        JinIllustratorGreatSuccessEffect, EpicaExplosionEffect, EpicaRocketEffect0, EpicaRocketEffect1, EpicaIdeaEffect,
        LimeEchoEffect, DurationHealEffect, ExplosionBigEffect, ZoomojiExploEffect, ZoomojiTransSuccessEffect, ZoomojiTransFailEffect, ShockBeatSkillEffect, MalkuthTransEffect, Terrified, MalkuthTransOffExlpoEffect, MalkuthExploEffect, MalkuthUpgradeEffect, SlienceEffect, HodKillEffect, HoldSummonSpawnEffect, ChokmahManaRegenEffect, VoidClusterStunEffect, CharmedEffect, TiferetSkillEffect, TiferetAbilityBuffEffect, TiferetAbilityEffect, NetzachCheatCodeEffect, NemesisTerrifiedEffect, NemesisSkillEffect, SephiraHealEffect, EnemyHealBenEffect, EnemyShieldBenEffect, NetzachByCurrentHPEffect, NetzachAbilityEffect, NetzachLostHpEffect, BloodEffect, NetzachAttackEffect, IgnisBombEffect, IgnisNormalAttackEffect, IgnisSuperAttackEffect, IgnisTransOffEffect, IgnisTransOnEffect, SpecialShieldHideEffect, CriticalPerEffect, StarLevelDownEffect, StarLevelUpEffect

    }

    [Header("# Prefabs")]
    public GameObject[] prefabs;
    [Header("# Pool Size")]
    public int size;
    List<GameObject>[] pools;

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

    public GameObject Get(PrefabType prefabType, Vector3 pos, Transform parent = null)
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

        if (parent)
        {
            select.transform.SetParent(parent);
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
