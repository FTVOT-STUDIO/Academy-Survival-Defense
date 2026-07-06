using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "StarLightSkill", menuName = "Unit Skill Data/StarLightSkill")]
public class StarLightSkill : UnitSkillBase<StarLightSkill.StarLightRuntimeSkill, IUnitSkill>
{
    public class StarLightRuntimeSkill : IUnitSkill, IAttackPassive, IButtonAttckSkill, IOnDisableSkill, ICoolTimeOverride, IButtonInfoinitialize
    {
        Unit unit;

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }


        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy target = unit.unitScanner.GetCurrentTarget();
            if (target == null)
                return;

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.Launch(target.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Unit, 0.8f, 0.1f);
        }

        public void ApplyButtonAttckSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.isSkillActive && unit.unitEvent.IsUnitLocked)
            {
                if (unit.unitWeapon.weaponObj == null)
                    return;

                unit.unitWeapon.isSkillActive = false;
                unit.unitEvent.IsUnitLocked = false;

                unit.unitWeapon.weaponObj.transform.SetParent(PoolManager.Inst.transform);
                unit.unitWeapon.weaponObj.SetActive(false);

                monoBehaviour.StopCoroutine(ActiveSkillCo(unit));
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ClearBuff);

                unit.anim.SetBool("isSkill", false);
                return;
            }

            if (!unit.unitWeapon.isSkillActive && !unit.unitEvent.IsUnitLocked && unit.unitWeapon.skillTime > 0)
            {
                unit.unitWeapon.isSkillActive = true;
                unit.unitEvent.IsUnitLocked = true;

                unit.anim.SetBool("isSkill", true);

                monoBehaviour.StartCoroutine(ActiveSkillCo(unit));
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.StarLightOn0);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.StarLightOn1);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.StarLightOn2);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.Laser);
            }
        }

        public void InitializeSkillButtonUI(MonoBehaviour monoBehaviour)
        {
            unit.unitInfo.InitializeSkillButton("빛의 광선");
        }

        public void ApplyOnDisableSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.weaponObj != null)
            {
                unit.unitWeapon.weaponObj.transform.SetParent(PoolManager.Inst.transform);
                unit.unitWeapon.weaponObj.SetActive(false);
            }

        }



        IEnumerator ActiveSkillCo(Unit unit)
        {
            LaserBullet laserBullet = PoolManager.Inst.bulletGenerator.GetLaserBullet(PoolManager.PrefabType.StarLight, unit.gameObject);
            unit.unitWeapon.weaponObj = laserBullet.gameObject;
            laserBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * (1f + unit.skillAmount), unit.criticalPer, unit.criticalDamage, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier, 0.1f);
            laserBullet.Launch(unit.transform.position, Bullet.BulletOwnerType.Unit);

            laserBullet.transform.SetParent(unit.unitWeaponPrefabParent);

            while (unit.unitWeapon.skillTime > 0)
            {
                //    while (GameManager.Inst.isIntroDelay)
                //yield return null;

                if (!unit.unitWeapon.isSkillActive && !unit.unitEvent.IsUnitLocked)
                {
                    break;
                }
                laserBullet.damage = unit.damageAmout * (1f + unit.skillAmount);
                laserBullet.criticalPer = unit.criticalPer;
                laserBullet.criticalDamage = unit.criticalDamage;
                unit.unitWeapon.skillTime = Mathf.Max(unit.unitWeapon.skillTime - Time.deltaTime * 3, 0);
                unit.UpdateManaBar();
                yield return null;
            }

            unit.anim.SetBool("isSkill", false);
            unit.unitEvent.IsUnitLocked = false;
            unit.unitWeapon.isSkillActive = false;
            unit.unitWeapon.weaponObj.transform.SetParent(PoolManager.Inst.transform);
            unit.unitWeapon.weaponObj.SetActive(false);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ClearBuff);
        }
    }
}


