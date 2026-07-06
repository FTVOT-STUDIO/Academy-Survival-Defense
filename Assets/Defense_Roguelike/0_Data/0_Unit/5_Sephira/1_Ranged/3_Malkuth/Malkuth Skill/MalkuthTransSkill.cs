using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "MalkuthTransSkill", menuName = "Unit Skill Data/MalkuthTransSkill")]
public class MalkuthTransSkill : SephiraSkillBase<MalkuthTransSkill.MalkuthTransRumtimeSkill, ISephiraSkill>
{
    public class MalkuthTransRumtimeSkill : ISephiraSkill, IOnEnableSkill, IAttackPassive, IOnDisablePassive
    {
        SephiraUnit unit;

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            if (unit.nearestTarget == unit.lastNearestTarget)
            {
                float multiplier = 1f + Mathf.Log(unit.sameTargetHitCount + 1, 2f) * 0.2f;

                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 2 * multiplier, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetVisualEffect(EffectManager.PrefabType.KetherSkillEffect, true, AudioManager.Sfx.malkuthAttackSpeed);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 0.3f, 0.1f);
            }
            else
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 2, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 0.3f, 0.1f);
            }

            if (unit.hitCount > 0 && unit.hitCount % 50 == 0)
            {
                List<Unit> fieldUnits = FieldManager.Inst.GetFieldBatchUnits();

                if (fieldUnits == null || fieldUnits.Count == 0)
                    return;

                Unit targetUnit = fieldUnits
                    .OrderBy(u => u.health / u.unitData.maxHealth)
                    .FirstOrDefault();

                if (targetUnit != null)
                {
                    targetUnit.unitEvent.ApplyDurationHealToUnit((float)unit.damageAmout, unit.healingMultiplier, targetUnit.healingReceivedMultiplier, 2, 0.3f, unit.unitData);
                }

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.Heal);
            }

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.05f))
            {
                Bullet waeknessBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                Bullet stunBullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

                waeknessBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 2, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                waeknessBullet.SetDebuffInfo(Bullet.DebuffState.Weakness, unit.skillAmount, 2, unit.damageAmout);
                waeknessBullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 2f, 0.1f);

                stunBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 2, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                stunBullet.SetDebuffInfo(Bullet.DebuffState.Stun, unit.skillAmount, 2, unit.damageAmout);
                stunBullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 2f, 0.1f);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.MalkuthStun);
            }

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.1f))
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 2 * (1 + unit.skillAmount), 100, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetVisualEffect(EffectManager.PrefabType.MalkuthExploEffect, true, AudioManager.Sfx.Explo9);
                bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 2f, 0.1f);
            }

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.01f))
            {
                LaserBullet laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.MalkuthBirdBullet, unit.gameObject);

                laserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, 0.2f);
                laserBullet.Launch(unit.transform.position, Bullet.BulletOwnerType.Sephira);

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.EagleScream);
            }
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameAttackSpeed(-0.4f);
        }

        public void ApplyOnEnableSkill(MonoBehaviour monoBehaviour)
        {
            GameStatManager.Inst.AddGameAttackSpeed(0.4f);

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.SetDebuffInfo(Bullet.DebuffState.Terrified, unit.skillAmount, 1);
            bullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 5, 0.1f);

            CameraShake.Inst.Shake(1, 1, 0.1f);

            monoBehaviour.StartCoroutine(OnSkill(unit));

            ObjectLifecycleManager malkuthTransUrp = PoolManager.Inst.Get(PoolManager.PrefabType.MalkuthTransUrp, PoolManager.Inst.transform.position).GetComponent<ObjectLifecycleManager>();

            malkuthTransUrp.SetDelay(unit.abllityApplyTime);

            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0.7f, 0.4f, 1), 0.8f);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Chord);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.MultiBelling);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Alert3);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.MalkuthTrans);

            AudioManager.Inst.AudioLowPassFilter(true);
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill(SephiraUnit unit)
        {
            int getCoin = unit.hitCount;
            unit.anim.SetTrigger("Attack");
            GameObject magicCircle = PoolManager.Inst.Get(PoolManager.PrefabType.MalkuthMagicCircle, unit.transform.position);

            magicCircle.transform.SetParent(FieldManager.Inst.fieldSephira.sp.transform);
            magicCircle.GetComponentInChildren<FlipXPositionSync>().parentSpriteRenderer = FieldManager.Inst.fieldSephira.sp;
            magicCircle.GetComponentsInChildren<SortingOrderController>()[0].Init
            (unit.sp);
            magicCircle.GetComponentsInChildren<SortingOrderController>()[1].Init(unit.sp);

            EffectManager.Inst.Get(EffectManager.PrefabType.MalkuthTransEffect, unit.transform.position);

            yield return new WaitForSeconds(unit.abllityApplyTime);
            CameraShake.Inst.Shake(1, 1, 1f);
            EffectManager.Inst.Get(EffectManager.PrefabType.MalkuthTransOffExlpoEffect, unit.transform.position);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo8);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo6);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.EarthQuake);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Epic);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.FocusOn2);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.MalkuthAbilityOff);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.TimeStop);

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, 4, 0, 4, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.SetDebuffInfo(Bullet.DebuffState.InsDeathAll);
            bullet.Launch(unit.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 40, 1f);

            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0.7f, 0.4f, 1), 0.4f);

            PoolManager.Inst.Get(PoolManager.PrefabType.MalkuthBlackholeUrp, PoolManager.Inst.transform.position);
            yield return new WaitForSeconds(0.8f);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo7);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.InputBeep);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.OutputBeep);

            Transform malkuthAbilityOffUI = PoolManager.Inst.Get(PoolManager.PrefabType.MalkuthAbilityOffUI, PoolManager.Inst.transform.position).transform;

            malkuthAbilityOffUI.SetParent(InGameUIManager.Inst.inGameCanvas.transform, false);
            RectTransform rect = malkuthAbilityOffUI.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            GameUIManager.Inst.ShowFleshImage(InGameUIManager.Inst.inGameCanvas.transform, new Color(0.7f, 0.4f, 1), 0.4f);

            yield return new WaitForSeconds(1);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Electronic);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Glitch);
            magicCircle.SetActive(false);
            unit.anim.SetTrigger("Attack");
            unit.TransInit(unit.unitData.transData);
            AudioManager.Inst.AudioLowPassFilter(false);
            GameManager.Inst.ModifyCoin(getCoin);

            List<Unit> batchUnits = FieldManager.Inst.GetFieldBatchUnits();
            for (int i = 0; i < batchUnits.Count; i++)
            {
                batchUnits[i].Dead();
            }
        }

    }
}
