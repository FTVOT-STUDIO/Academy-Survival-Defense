using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SayaTreasureFieldSkill", menuName = "Unit Skill Data/SayaTreasureFieldSkill")]
public class SayaTreasureFieldSkill : UnitSkillBase<SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill, IUnitSkill>, IUnitManagerFactory
{
    public ResourcesData resourcesData;
    [Header("보너스 턴")]
    [Range(0, 1)] public float bonusTurnChance = 0.2f;
    [Range(0, 1)] public float bonusRepeatChance = 0.55f;
    [Min(1)] public float bonusDamageMultiplier = 3;
    [Min(0.02f)] public float bonusTurnInterval = 0.15f;
    [Min(0)] public float comboCooldownRecovery = 1;

    [Header("행동 선언: 누적 강화량")]
    [Min(0)] public float healthGain = 0.15f;
    [Min(0)] public float shieldGain = 0.2f;
    [Min(0)] public float defenseGain = 0.1f;
    [Min(0)] public float damageGain = 0.12f;
    [Min(0)] public float criticalGain = 0.1f;
    [Min(0)] public float criticalDamageGain = 0.15f;
    [Min(0)] public float attackSpeedGain = 0.1f;
    [Min(0)] public float selfIgnoreDuration = 5;
    [Min(0)] public float resourceBlockDuration = 10;

    [Header("변신 능력")]
    [Range(0, 1)] public float allyDamageAbsorption = 0.2f;
    [Range(0, 1)] public float lostHealthRecovery = 0.05f;
    [Range(0, 1)] public float stunChance = 0.15f;
    [Min(0)] public float stunDuration = 1;
    [Min(0)] public float hunterRangeBonus = 0.3f;
    [Range(0, 1)] public float resistancePenetration = 0.25f;
    [Min(0)] public float splashRadius = 1.2f;
    [Range(0, 1)] public float executionHealthRatio = 0.15f;
    [Range(0, 1)] public float shamanHealRatio = 0.03f;
    [Range(0, 1)] public float allyCooldownChance = 0.2f;
    [Min(0)] public float allyCooldownRecovery = 2;
    [Min(0)] public float allCooldownRecovery = 1;

    public void CreateUnitManager()
    {
        if (SayaCharacterManager.Inst != null) return;
        GameObject manager = PoolManager.Inst.Get(PoolManager.PrefabType.SayaCharacterManager, Vector3.zero);
        manager.transform.SetParent(GameManager.Inst.unitManagerGroup);
    }

    public class SayaTreasureFieldRuntimeSkill : IUnitSkill, IAttackPassive, IButtonAttckSkill, IButtonInfoinitialize, IUnitFieldBatchPassive, ICoolTimeOverride, IFieldMovementLocked, IUpdatePassive, IOnUnitDeathOverride, IDamageModifier, IMaxHealthModifier, IMaxShieldModifier, IDefenseModifier, ICriticalPerModifier, ICriticalDamageModifier, IAttackSpeedModifier, IRangeModifier
    {
        private SayaTreasureFieldSkill settings;
        private Unit unit;
        private bool bonusActive;
        private float nextButtonRefresh;
        private bool deathPending;
        private int sharedShieldApplied;
        private SayaCharacterManager Manager => SayaCharacterManager.Inst;
        private SayaCharacterManager.UnitType Position => Manager.GetPosition(unit);
        private int Level => Manager.GetLevel(Position);

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
            settings = (SayaTreasureFieldSkill)unit.skillSO;
        }

        public void ApplyUnitFieldBatchPassive(MonoBehaviour monoBehaviour)
        {
            Manager.ApplyTransformNextFrame(unit);
        }

        public void RefreshSharedShield()
        {
            if (unit.unitEvent.IsIgnored) return;
            while (sharedShieldApplied < Manager.specialShieldBonus)
            {
                unit.unitEvent.IncrementSpecialShield();
                sharedShieldApplied++;
            }
        }

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            if (Manager == null) return;
            Enemy target = unit.unitScanner.GetCurrentTarget();
            if (target == null || !target.isLive) return;
            Attack(target, 1);
            ApplyTransformationAttack(target);
            if (!bonusActive && Random.value < UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, settings.bonusTurnChance))
                unit.StartCoroutine(BonusTurns(unit.unitSkill));
        }

        private IEnumerator BonusTurns(IUnitSkill originalSkill)
        {
            bonusActive = true;
            int combo = 0;
            bool repeat;
            do
            {
                yield return new WaitForSeconds(settings.bonusTurnInterval);
                if (!unit.isLive || unit.unitSkill != originalSkill || unit.unitEvent.IsIgnored || unit.unitEvent.IsSilenced || unit.unitEvent.IsUnitLocked) break;
                Enemy target = unit.unitScanner.GetCurrentTarget();
                if (target == null || !target.isLive) break;
                Attack(target, settings.bonusDamageMultiplier);
                combo++;
                ForeSkillEffect.Play(SayaCharacterManager.Inst.skillEffectPrefab, target.transform.position, 2, Color.cyan, 0.9f, 0.3f);
                unit.anim.SetTrigger("Attack");
                var text = PoolManager.Inst.Get(PoolManager.PrefabType.FloatingText, unit.transform.position).GetComponent<FloatingText>();
                text.InitializeText("보너스 턴 " + combo, 4, Color.cyan);
                repeat = Random.value < settings.bonusRepeatChance;
            } while (repeat);

            if (unit.isLive && unit.unitSkill == originalSkill && combo > 0)
                unit.unitEvent.ApplyGetCoolTime(combo * settings.comboCooldownRecovery);
            bonusActive = false;
        }

        private void Attack(Enemy target, float multiplier)
        {
            float penetration = unit.attributeResistancePenetration;
            if (Position == SayaCharacterManager.UnitType.hunter && Level >= 2)
                penetration = Mathf.Clamp01(penetration + settings.resistancePenetration);
            Vector3 center = target.transform.position;
            bool splash = Position == SayaCharacterManager.UnitType.hunter && Level >= 3;
            var targets = new List<Enemy> { target };
            if (splash)
            {
                // 적 탐지 콜라이더가 여러 개여도 한 적은 한 번만 타격합니다.
                foreach (Enemy enemy in EnemySpawnManager.Inst.spawnedEnemies)
                {
                    if (enemy != target && enemy != null && enemy.isLive && Vector2.Distance(center, enemy.transform.position) <= settings.splashRadius)
                        targets.Add(enemy);
                }
            }
            foreach (Enemy enemy in targets)
            {
                if (!enemy.isLive) continue;
                unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, unit.damageAmout * multiplier, unit.criticalPer, unit.criticalDamage, splash ? unit.areaDamage : unit.singleTargetDamage, unit.defensePenetration, penetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, enemy);
                if (Position == SayaCharacterManager.UnitType.hunter && Level >= 4 && enemy.isLive && enemy.health > 0 && enemy.health / enemy.maxHealth <= settings.executionHealthRatio)
                    enemy.enemyEvent.InsDeathAll();
            }
        }

        private void ApplyTransformationAttack(Enemy target)
        {
            if (Position == SayaCharacterManager.UnitType.field)
            {
                if (Level >= 2)
                    unit.unitEvent.ApplyGetHeal((unit.maxHealth - unit.health) * settings.lostHealthRecovery, unit.healingMultiplier, unit.healingReceivedMultiplier, unit.unitData);
                if (Level >= 3 && target.isLive && Random.value < settings.stunChance)
                    target.enemyEvent.TryStun(1, settings.stunDuration);
            }
            else if (Position == SayaCharacterManager.UnitType.shaman && Level >= 1)
            {
                var allies = FieldManager.Inst.GetFieldBatchUnits();
                allies.RemoveAll(u => !u.isLive);
                Unit lowest = null;
                foreach (Unit ally in allies)
                    if (lowest == null || ally.health / ally.maxHealth < lowest.health / lowest.maxHealth) lowest = ally;
                if (lowest != null)
                {
                    lowest.unitEvent.ApplyGetHeal(lowest.maxHealth * settings.shamanHealRatio, unit.healingMultiplier, lowest.healingReceivedMultiplier, unit.unitData);
                    ForeSkillEffect.Play(SayaCharacterManager.Inst.skillEffectPrefab, lowest.transform.position, 2, new Color(0.25f, 1, 0.65f), 0.8f, 0.45f);
                }
                if (Level >= 2 && Random.value < settings.allyCooldownChance)
                {
                    allies.RemoveAll(u => u.unitWeapon.skillTime >= u.coolTime);
                    if (allies.Count > 0) RecoverCooldown(allies[Random.Range(0, allies.Count)], settings.allyCooldownRecovery);
                }
            }
        }

        private void RecoverCooldown(Unit target, float amount)
        {
            if (Level >= 4)
            {
                target.unitWeapon.skillTime = target.coolTime;
                target.UpdateManaBar();
                ForeSkillEffect.Play(SayaCharacterManager.Inst.skillEffectPrefab, target.transform.position, 2, new Color(0.3f, 0.65f, 1), 0.8f, 0.45f);
            }
            else target.unitEvent.ApplyGetCoolTime(amount);
        }

        public void ApplyShamanSkillRecovery()
        {
            if (Position != SayaCharacterManager.UnitType.shaman || Level < 3) return;
            foreach (Unit ally in FieldManager.Inst.GetFieldBatchUnits())
                if (ally.isLive) RecoverCooldown(ally, settings.allCooldownRecovery);
        }

        public void ApplyButtonAttckSkill(MonoBehaviour monoBehaviour)
        {
            if (Manager != null) Manager.TryRolling(unit);
        }

        public void ApplyUpdatePassive(MonoBehaviour monoBehaviour)
        {
            if (Time.time < nextButtonRefresh || !unit.unitInfo.skillBtn.activeSelf) return;
            nextButtonRefresh = Time.time + 0.2f;
            InitializeSkillButtonUI(monoBehaviour);
        }

        public void InitializeSkillButtonUI(MonoBehaviour monoBehaviour)
        {
            if (Manager == null) return;
            Sprite icon = settings.resourcesData != null ? settings.resourcesData.icon : null;
            unit.unitInfo.InitializeSkillButton("행동 선언", true, Manager.rollingCost + " / " + Mathf.RoundToInt(unit.unitWeapon.skillTime) + "/" + unit.coolTime.ToString("F0"), icon != null, icon);
        }

        public void ApplyOnUnitDeathOverride(MonoBehaviour monoBehaviour)
        {
            if (deathPending) return;
            if (Manager == null || Position != SayaCharacterManager.UnitType.field || Level < 4)
            {
                unit.Dead();
                return;
            }
            deathPending = true;
            unit.health = 1;
            unit.unitEvent.ApplyInvulnerable(3);
            unit.StartCoroutine(DelayedDisassembly(unit.unitSkill));
        }

        private IEnumerator DelayedDisassembly(IUnitSkill originalSkill)
        {
            yield return new WaitForSeconds(3);
            if (unit.isLive && unit.unitSkill == originalSkill) SayaCharacterManager.Disassemble(unit);
        }

        public float GetDamageMultiplier() => 1 + (Manager != null ? Manager.damageBonus : 0);
        public float GetMaxHealthMultiplier() => 1 + (Manager != null ? Manager.healthBonus : 0);
        public float GetMaxShieldMultiplier() => 1 + (Manager != null ? Manager.shieldBonus : 0);
        public float GetDefenseMultiplier() => 1 + (Manager != null ? Manager.defenseBonus : 0);
        public float GetCriticalPerMultiplier() => 1 + (Manager != null ? Manager.criticalBonus : 0);
        public float GetCriticalDamageMultiplier() => 1 + (Manager != null ? Manager.criticalDamageBonus : 0);
        public float GetAttackSpeedMultiplier() => 1 + (Manager != null ? Manager.attackSpeedBonus : 0);
        public float GetRangeMultiplier() => Manager != null && Position == SayaCharacterManager.UnitType.hunter && Level >= 1 ? 1 + settings.hunterRangeBonus : 1;
    }
}
