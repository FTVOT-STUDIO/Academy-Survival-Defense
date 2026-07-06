using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "CheatCodeData", menuName = "CheatCode Data/CheatCodeData")]
public class CheatCodeData : ScriptableObject
{
    [Header("Basic Info")]
    public int cheatId;
    public string cheatName = "치트 코드 : ";
    public enum CheatCodeType { kether, Chokhmah, Binah, Chesed, Gevurah, Tiferet, Netzach, Hod, Yesod, Malkuth }
    public CheatCodeType cheatCodeType;
    [TextArea]
    public string cheatDesc;

    [Header("UI Icons")]
    public Sprite cheatButtonIcon;
    public Sprite cheatInfoIcon;

    [Header("Cheat Settings")]
    public float coolTime;
    public int applyCost;

    [Header("Resource Type")]
    public ResourcesData resourceData;

    public void ApplyCheatCode()
    {
        switch (cheatCodeType)
        {
            case CheatCodeType.kether:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0.5f, 0, 1), 0.4f);
                Vector3 pos = FieldManager.Inst.fieldSephira.transform.position;

                Bullet bulletSlow = PoolManager.Inst.Get(PoolManager.PrefabType.Bullet0, pos).GetComponent<Bullet>();
                bulletSlow.SetAttackInfo(AttributeData.AttributeType.Poison, GameStatManager.Inst.GameDamage, 100, 1000, UnitUtility.AttackRangeType.Area, GameStatManager.Inst.GameAreaDamage, 0, 0, 1, UnitUtility.AttackType.None, 1);
                bulletSlow.SetDebuffInfo(Bullet.DebuffState.Slow, 0.9f, 5);
                bulletSlow.SetVisualEffect(isPlaySfx: false);
                bulletSlow.Launch(pos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.None, 999, 0.1f);

                Bullet bulletDot = PoolManager.Inst.Get(PoolManager.PrefabType.Bullet0, pos).
                GetComponent<Bullet>();

                bulletDot.SetAttackInfo(AttributeData.AttributeType.Poison, GameStatManager.Inst.GameDamage, 100, 1000, UnitUtility.AttackRangeType.Area, GameStatManager.Inst.GameAreaDamage, 0, 0, 1, UnitUtility.AttackType.None, 1);
                bulletDot.SetDebuffInfo(Bullet.DebuffState.Dot, 0.3f, 5, GameStatManager.Inst.GameDamage);
                bulletDot.SetVisualEffect(isPlaySfx: false);
                bulletDot.Launch(pos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.None, 999, 0.1f);

                Bullet bulletWeakness = PoolManager.Inst.Get(PoolManager.PrefabType.Bullet0, pos).GetComponent<Bullet>();

                bulletWeakness.SetAttackInfo(AttributeData.AttributeType.Poison, GameStatManager.Inst.GameDamage, 100, 1000, UnitUtility.AttackRangeType.Area, GameStatManager.Inst.GameAreaDamage, 0, 0, 1, UnitUtility.AttackType.None, 1);
                bulletWeakness.SetDebuffInfo(Bullet.DebuffState.Weakness, 0.9f, 5, GameStatManager.Inst.GameDamage);
                bulletWeakness.SetVisualEffect(isPlaySfx: false);
                bulletWeakness.Launch(pos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.None, 999, 0.1f);

                Bullet bulletStun = PoolManager.Inst.Get(PoolManager.PrefabType.Bullet0, pos).GetComponent<Bullet>();

                bulletStun.SetAttackInfo(AttributeData.AttributeType.Poison, GameStatManager.Inst.GameDamage, 100, 1000, UnitUtility.AttackRangeType.Area, GameStatManager.Inst.GameAreaDamage, 0, 0, 1, UnitUtility.AttackType.None, 1);
                bulletStun.SetDebuffInfo(Bullet.DebuffState.Stun, 0.9f, 2);
                bulletStun.SetVisualEffect(isPlaySfx: false);
                bulletStun.Launch(pos, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.None, 999, 0.1f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.KehterCheatCode);
                break;
            case CheatCodeType.Chokhmah:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0, 1, 0.7f), 0.4f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ChokmahCheatCode);
                FieldManager.Inst.fieldSephira.sephiraUnitEvent.AddAbllityStats(FieldManager.Inst.fieldSephira.maxAbllityCount);
                List<Unit> fieldUnits = FieldManager.Inst.GetFieldBatchUnits();
                if (fieldUnits == null || fieldUnits.Count == 0)
                    return;

                foreach (var fieldUnit in fieldUnits)
                {
                    if (fieldUnit == null)
                        continue;
                    fieldUnit.unitWeapon.skillTime = fieldUnit.unitData.coolTime;
                    Vector3 effectPos = fieldUnit.transform.position + new Vector3(0, 0.5f, 0);
                    EffectManager.Inst.Get(EffectManager.PrefabType.ManaRegenEffect, effectPos);
                }
                break;
            case CheatCodeType.Binah:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 1, 0), 0.4f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.BinahCheatCode);
                FieldManager.Inst.fieldSephira.sephiraUnitEvent.ApplyHeal(FieldManager.Inst.fieldSephira.maxHealth, 1, 1);
                FieldManager.Inst.fieldSephira.health = FieldManager.Inst.fieldSephira.maxHealth;
                List<Unit> healUnits = FieldManager.Inst.GetFieldBatchUnits();
                if (healUnits == null || healUnits.Count == 0)
                    return;

                foreach (var fieldUnit in healUnits)
                {
                    if (fieldUnit == null)
                        continue;
                    fieldUnit.unitEvent.ActivateDebuffClear();
                    fieldUnit.unitEvent.ApplyDurationHealToUnit(fieldUnit.maxHealth * 0.2f, GameStatManager.Inst.GameHealingMultiplier, GameStatManager.Inst.GameHealingReceivedMultiplier, 5f, 0.5f);
                }
                break;
            case CheatCodeType.Chesed:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.
                inGameCanvas.transform, new Color(1, 0.5f, 1), 0.4f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ChesedCheatCode);
                List<Unit> shieldUnits = FieldManager.Inst.GetFieldBatchUnits();
                if (shieldUnits == null || shieldUnits.Count == 0)
                    return;

                foreach (var fieldUnit in shieldUnits)
                {
                    if (fieldUnit == null)
                        continue;

                    fieldUnit.unitEvent.IncrementSpecialShield();
                }
                //  GameObject barrier = PoolManager.Inst.Get(PoolManager.PrefabType.BarrierObject, PoolManager.Inst.transform.position);
                // barrier.transform.position = new Vector3(barrier.transform.position.x, -2.5f, barrier.transform.position.z);
                // barrier.GetComponent<ObjectLifecycleManager>().SetDelay(30);
                break;
            case CheatCodeType.Gevurah:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0.5f, 1, 1), 0.4f);
                Bullet rocket = PoolManager.Inst.Get(PoolManager.PrefabType.ScorchedEarthRocket, PoolManager.Inst.transform.position).GetComponent<Bullet>();
                rocket.transform.position = new Vector3(rocket.transform.position.x, -30, rocket.transform.position.z);
                rocket.SetAttackInfo(AttributeData.AttributeType.Electro, 0, 0, 0, UnitUtility.AttackRangeType.Area, 0, 0, 0, 0, UnitUtility.AttackType.None, 0);
                rocket.SetDebuffInfo(Bullet.DebuffState.InsDeathOnlyNormal);
                rocket.SetVisualEffect(EffectManager.PrefabType.ExplosionBigEffect, true, AudioManager.Sfx.Explo3);
                rocket.Launch(new Vector3(0, 10, 0), Bullet.BulletInitMode.Linear, Bullet.BulletType.Multi, Bullet.BulletOwnerType.None, 5, 5, Bullet.BulletSpeedType.Fast);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.GevurahCheatCode);
                break;
            case CheatCodeType.Tiferet:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0, 0.4f), 0.4f);
                GameStatManager.Inst.AddGameDamage(0.5f);
                GameStatManager.Inst.AddGameSkillAmount(0.2f);
                GameStatManager.Inst.AddGameCriticalPer(0.5f);
                GameStatManager.Inst.AddGameCriticalDamage(0.5f);
                GameStatManager.Inst.AddGameExpValue(0.2f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.TipherethCheatCode);
                break;
            case CheatCodeType.Netzach:
                CameraShake.Inst.Shake(2f, 2, 0.1f);
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0.1f, 0), 0.4f);
                Vector3 spawnPos = GameManager.Inst.transform.position;
                spawnPos.y -= 5f;

                GameObject effect = EffectManager.Inst.Get(EffectManager.PrefabType.NetzachCheatCodeEffect, spawnPos);
                foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                {
                    enemy.OnDamage(AttributeData.AttributeType.Electro, enemy.maxHealth * 0.05f, 0, 0, owner: null, onDeathCallBack: null);
                    enemy.enemyEvent.ResetToStartPosition(0.5f);
                    enemy.enemyEvent.ApplyDebuff(Bullet.DebuffState.Stun, 1, 2);
                }
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachCheatCode);
                break;
            case CheatCodeType.Hod:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0, 0.5f), 0.4f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.HodCheatCode);
                List<Unit> lvUnits = FieldManager.Inst.GetFieldBatchUnits();
                if (lvUnits == null || lvUnits.Count == 0)
                    return;

                foreach (Unit unit in lvUnits)
                {
                    if (unit == null)
                        continue;

                    unit.unitEvent.ApplyLevelUpUnit(1);
                }
                break;
            case CheatCodeType.Yesod:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0, 0.1f, 1), 0.4f);
                GameManager.Inst.ModifyCoin(3000);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.YesodCheatCode);
                break;
            case CheatCodeType.Malkuth:
                GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0.7f, 0.4f, 1), 0.4f);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.MalkuthCheatCode);
                break;
        }
        GameStatManager.Inst.MarkStatsDirty();
    }
}
