using UnityEngine;

[CreateAssetMenu(fileName = "ZoomojiSkill", menuName = "Unit Skill Data/ZoomojiSkill")]
public class ZoomojiSkill : UnitSkillBase<ZoomojiSkill.ZoomojiRumtimeSkill, IUnitSkill>
{
    public class ZoomojiRumtimeSkill : IUnitSkill, IAttackPassive, IButtonAttckSkill, ICoolTimeOverride, IButtonInfoinitialize
    {
        Unit unit;
        public UnitData unitStateData;

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }
        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            int bulletCount = 5;
            float totalAngle = 45;
            float startAngle = -totalAngle / 2f;
            float angleStep = totalAngle / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = startAngle + angleStep * i;

                Vector2 direction = Quaternion.Euler(0, 0, -angle) * unit.transform.up;

                Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

                bullet.transform.localScale = new Vector3(1f, 1f, 1f);
                bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer * 2, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
                bullet.SetVisualEffect(isPlaySfx: false);
                bullet.Launch(unit.unitScanner.nearestTarget.position, Bullet.BulletInitMode.Linear, Bullet.BulletType.Single, Bullet.BulletOwnerType.Unit, 0.5f, 0.5f);

                bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                bullet.rb.linearVelocity = direction * 8;
            }

            AudioManager.Inst.PlaySfx(AudioManager.Sfx.ShotGun3);
        }

        public void ApplyButtonAttckSkill(MonoBehaviour monoBehaviour)
        {
            if (unit.unitWeapon.skillTime >= unit.coolTime)
            {
                if (Random.value <= 0.5f)
                {
                    EffectManager.Inst.Get(EffectManager.PrefabType.ZoomojiTransSuccessEffect, unit.transform.position);
                    unit.TransInit(unit.unitData.transData, unit.starLevel);
                    unit.anim.SetTrigger("Attack");
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Stress2);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Alert2);


                }
                else
                {
                    EffectManager.Inst.Get(EffectManager.PrefabType.ZoomojiTransFailEffect, unit.transform.position);
                    unit.UnitDeActive();
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.ZoomojiTransFail);
                    AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo5);
                }

                AudioManager.Inst.PlaySfx(AudioManager.Sfx.Stress);

            }
        }

        public void InitializeSkillButtonUI(MonoBehaviour monoBehaviour)
        {
            unit.unitInfo.InitializeSkillButton("50%", true, "재창조");
        }

        public void ApplyICoolTimeOverride(MonoBehaviour monoBehaviour)
        {
        }
    }
}
