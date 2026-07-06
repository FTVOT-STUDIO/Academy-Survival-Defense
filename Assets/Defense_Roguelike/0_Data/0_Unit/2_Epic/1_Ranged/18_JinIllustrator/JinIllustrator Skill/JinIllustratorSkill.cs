using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "JinIllustratorSkill", menuName = "Unit Skill Data/JinIllustratorSkill")]
public class JinIllustratorSkill : UnitSkillBase<JinIllustratorSkill.JinIllustratorRuntimeSkill, IUnitSkill>
{
    public class JinIllustratorRuntimeSkill : IUnitSkill, ICoolTimeOverride, IAttackPassive, IButtonPassive, IButtonInfoinitialize
    {
        Unit unit;
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Transform target = unit.unitScanner.nearestTarget;
            double damage = unit.damageAmout;
            float critRate = unit.criticalPer;
            float critDamage = unit.criticalDamage;
            Vector3 position = target.position;

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            if (Random.value < UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.08f))
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, damage, critRate, critDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetDebuffInfo(Bullet.DebuffState.Slow, 0.5f, 2);
                bullet.Launch(position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Unit, 0.5f, 0.1f);
            }
            else
            {
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, damage, critRate, critDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.Launch(position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Unit, 0.5f, 0.1f);
            }

        }

        public void ApplyButtonPassive(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.skillTime <= 0 || unit.unitWeapon.isSkillActive)
                return;

            monoBehaviour.StartCoroutine(ApplySkillCoroutine(unit));

        }

        IEnumerator ApplySkillCoroutine(Unit unit)
        {
            unit.unitWeapon.isSkillActive = true;

            int reward;
            float spentCooltime = unit.unitWeapon.skillTime;
            int waitTime = 4;
            float percent = spentCooltime / unit.coolTime;

            unit.unitWeapon.skillTime = 0;
            unit.unitEvent.IsUnitLocked = true;

            unit.anim.SetTrigger("Attack");


            GameObject ideaEffect = EffectManager.Inst.Get(EffectManager.PrefabType.JinIllustratorIdeaEffect, unit.transform.position);
            GameObject waitEffect = EffectManager.Inst.Get(EffectManager.PrefabType.JinIllustratorWaitEffect, unit.transform.position);

            waitEffect.GetComponent<ObjectLifecycleManager>().SetParent(unit.transform);
            waitEffect.GetComponent<ObjectLifecycleManager>().SetDelay(waitTime);
            ideaEffect.GetComponent<ObjectLifecycleManager>().SetParent(unit.transform);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Idea);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Drills);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.RummagingBag);

            yield return new WaitForSeconds(waitTime);

            if (Random.value < percent)
            {
                if (Random.value > (1f - percent))
                {
                    reward = 2000;
                    GameManager.Inst.ModifyCoin(reward);

                    InGameUIManager.Inst.ShowResourceGainEffect(unit.transform, reward, ResourcesData.ResourcesType.coin);

                    EffectManager.Inst.Get(EffectManager.PrefabType.JinIllustratorSuccessEffect, unit.transform.position);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Success0);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Piyoong);
                }
                else
                {
                    reward = 3;
                    unit.unitWeapon.skillTime += spentCooltime * 0.2f;
                    GameManager.Inst.ModifyDarkMatteCoin(reward);

                    InGameUIManager.Inst.ShowResourceGainEffect(unit.transform, reward, ResourcesData.ResourcesType.darkMatteCoin);

                    EffectManager.Inst.Get(EffectManager.PrefabType.JinIllustratorGreatSuccessEffect, unit.transform.position);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Success0);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Success1);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Success2);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.cheering);
                }
            }
            else
            {
                EffectManager.Inst.Get(EffectManager.PrefabType.JinIllustratorFailEffect, unit.transform.position);
                unit.unitEvent.ApplyAttackSpeedStats(-0.1f, true);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.Fail);
            }

            unit.anim.SetTrigger("Attack");

            unit.unitWeapon.isSkillActive = false;
            unit.unitEvent.IsUnitLocked = false;
        }

        public void InitializeSkillButtonUI(MonoBehaviour monoBehaviour)
        {
            if (!unit.gameObject.activeSelf)
                return;

            monoBehaviour.StartCoroutine(UpdateSkillCooldown(unit));
        }

        public IEnumerator UpdateSkillCooldown(Unit unit)
        {
            float duration = unit.coolTime;

            while (unit.unitWeapon.skillTime < duration)
            {
                if (unit.isLive == false || !unit.unitInfo.skillBtn.activeSelf)
                {
                    yield break;
                }

                float spentTime = unit.unitWeapon.skillTime;
                float percent = Mathf.Clamp01(spentTime / duration) * 100f;

                unit.unitInfo.InitializeSkillButton("그림 그리기!", true, percent.ToString("F0") + "%");

                yield return null;
            }

            unit.unitInfo.InitializeSkillButton("그림 그리기!", true, "100%");
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
    }
}
