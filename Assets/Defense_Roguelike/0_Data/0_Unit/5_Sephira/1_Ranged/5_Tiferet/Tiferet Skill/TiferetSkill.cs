using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TiferetSkill", menuName = "Unit Skill Data/TiferetSkill")]
public class TiferetSkill : SephiraSkillBase<TiferetSkill.TiferetRuntimeSkill, ISephiraSkill>
{
    public class TiferetRuntimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive, IOnDisablePassive
    {
        public SephiraUnit unit;
        public bool isUpdatingCritDamage = false;
        public float lastAppliedOverflow = 0f;

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.nearestTarget.GetComponent<Enemy>();
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {

                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);

                bullet.SetDebuffInfo(Bullet.DebuffState.Charm, 0.2f, 3);
                bullet.SetVisualEffect(EffectManager.PrefabType.TiferetSkillEffect);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 1, 0.1f);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetSkill1);
            }
            else
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 1, 0.1f);
            }
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(OnSkill());
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            unit.sephiraUnitEvent.OnStatChanged -= UpdateCritDamageFromRate;
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            for (int i = 0; i < DeckManager.Inst.currentInGameDeck.unitdatas.Count; i++)
            {
                if (DeckManager.Inst.currentInGameDeck.unitdatas[i].type == Type.Support)
                {
                    GameStatManager.Inst.AddGameCriticalPer(0.1f);
                }
            }
            unit.sephiraUnitEvent.OnStatChanged += UpdateCritDamageFromRate;
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        private void UpdateCritDamageFromRate()
        {
            if (isUpdatingCritDamage) return;
            isUpdatingCritDamage = true;

            float overflowRate = Mathf.Max(unit.criticalPer - 100f, 0f);
            float additionalCritDamage = overflowRate / 100f;

            float delta = additionalCritDamage - lastAppliedOverflow;
            if (delta != 0f)
            {
                GameStatManager.Inst.AddGameCriticalDamage(delta);
                lastAppliedOverflow = additionalCritDamage;
            }

            isUpdatingCritDamage = false;
        }

        IEnumerator OnSkill()
        {
            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0, 0.4f), 0.4f);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetSkill0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility0);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility3);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility4);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility5);
            ObjectLifecycleManager tiferetURP = PoolManager.Inst.Get(PoolManager.PrefabType.TiferetURP, PoolManager.Inst.transform.position).GetComponent<ObjectLifecycleManager>();
            tiferetURP.Init(ObjectLifecycleManager.ObjectType.Effect);

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, 0, 0, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
            bullet.SetDebuffInfo(Bullet.DebuffState.Charm, unit.skillAmount, 2);
            bullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 7, 0.1f);

            EffectManager.Inst.Get(EffectManager.PrefabType.TiferetAbilityEffect, unit.transform.position);
            List<Unit> fieldUnits = FieldManager.Inst.GetFieldBatchUnits();

            if (fieldUnits.Count > 0)
            {
                int maxLevel = fieldUnits.Max(u => u.starLevel);
                List<Unit> topLevelUnits = fieldUnits.Where(u => u.starLevel == maxLevel).ToList();
                List<Unit> selectedUnits = topLevelUnits
                    .OrderBy(x => UnityEngine.Random.value)
                    .Take(3)
                    .ToList();

                foreach (var unit in selectedUnits)
                {
                    unit.unitEvent.ApplyInvulnerable(this.unit.abllityApplyTime);
                }
            }

            for (int i = 0; i < FieldManager.Inst.fieldSlots.Length; i++)
            {
                Vector3 spawnPos = FieldManager.Inst.fieldSlots[i].position + Vector3.up * 0.5f;
                EffectManager.Inst.Get(EffectManager.PrefabType.TiferetAbilityBuffEffect, spawnPos);
            }

            GameStatManager.Inst.AddGameSkillTriggerChance(0.1f);
            GameStatManager.Inst.AddGameCriticalPer(0.5f);
            GameStatManager.Inst.AddGameCriticalDamage(0.5f);
            GameStatManager.Inst.AddGameRange(0.2f);
            yield return new WaitForSeconds(unit.abllityApplyTime);
            GameStatManager.Inst.AddGameSkillTriggerChance(-0.1f);
            GameStatManager.Inst.AddGameCriticalPer(-0.5f);
            GameStatManager.Inst.AddGameCriticalDamage(-0.5f);
            GameStatManager.Inst.AddGameRange(-0.2f);
            tiferetURP.DeActive();
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TiferetAbility3);
            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(1, 0, 0.4f), 0.4f);
        }
    }
}

