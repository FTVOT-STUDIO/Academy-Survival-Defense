using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GevurahSkill", menuName = "Unit Skill Data/GevurahSkill")]
public class GevurahSkill : SephiraSkillBase<GevurahSkill.GevurahRuntimeSkill, ISephiraSkill>
{
    public class GevurahRuntimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive
    {
        SephiraUnit unit;

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy enemy = unit.nearestTarget.GetComponent<Enemy>();

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.Attack1);

            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.Launch(enemy.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 1, 0.1f);
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {

            monoBehaviour.StartCoroutine(OnSkill(unit, monoBehaviour));
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            // 광역 피해 증가
        }

        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        IEnumerator OnSkill(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            int bulletCount = 15;
            float totalAngle = 90f;
            float startAngle = -totalAngle / 2f;
            float angleStep = totalAngle / (bulletCount - 1);

            unit.anim.SetTrigger("Attack");
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.airPlane);
            AudioManager.Inst.PlaySfx(AudioManager.Sfx.skillGas);

            for (int i = 0; i < bulletCount; i++)
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                float angle = startAngle + angleStep * i;

                Vector2 direction = Quaternion.Euler(0, 0, -angle) * Vector2.up;
                bullet.transform.localScale = new Vector3(1.5f, 2.5f, 1.5f);

                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 3, unit.criticalPer * 2, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
                bullet.SetVisualEffect(isPlaySfx: false);
                bullet.Launch(unit.transform.position, Bullet.BulletInitMode.Linear, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 0.5f, 3f);

                bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                bullet.rb.linearVelocity = direction * 12f;

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.GevurahGun);
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.3f);

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.bombCount);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float x = -3f + j * 3f;
                    float y = -8.4f + i * 2.8f;

                    GameObject targetEffect = EffectManager.Inst.Get(EffectManager.PrefabType.GevurahTargetEffect, unit.transform.position);
                    targetEffect.transform.position = new Vector3(x, y, 0);
                    targetEffect.GetComponent<ObjectLifecycleManager>().SetDelay(1.8f);

                }
                yield return new WaitForSeconds(0.2f);
            }

            unit.anim.SetTrigger("Attack");
            PoolManager.Inst.Get(PoolManager.PrefabType.GevurahAirPlane, new Vector3(0, 0, 0));

            for (int i = 0; i < 6; i++)
            {
                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
                for (int j = 0; j < 3; j++)
                {

                    float x = -3f + j * 3f;
                    float y = -8.4f + i * 2.8f;

                    Bullet bombBullet = PoolManager.Inst.Get(PoolManager.PrefabType.Bomb, new Vector3(x, y, 0)).GetComponent<Bullet>();

                    bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer * 2, unit.criticalDamage, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
                    bullet.SetVisualEffect(isPlaySfx: false);
                    bullet.Launch(new Vector3(x, y, 0), Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 0.5f, 0.1f);

                    bombBullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout * 10, unit.criticalPer * 2, unit.criticalDamage * 2, UnitUtility.AttackRangeType.Area, unit.areaDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.Ability, unit.abilityMultiplier);
                    bombBullet.SetVisualEffect(isPlaySfx: false);
                    bombBullet.Launch(new Vector3(x, y, 0), Bullet.BulletInitMode.HitScan, Bullet.BulletType.Multi, Bullet.BulletOwnerType.Sephira, 3, 0.1f);

                    EffectManager.Inst.Get(EffectManager.PrefabType.Explosion, new Vector3(x, y, 0));
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Boomb);

                }
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(unit.abllityApplyTime);
        }
    }

}