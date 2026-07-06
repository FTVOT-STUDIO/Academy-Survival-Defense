using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChesedSkill", menuName = "Unit Skill Data/ChesedSkill")]
public class ChesedSkill : SephiraSkillBase<ChesedSkill.ChesedRuntimeSkill, ISephiraSkill>
{
    public class ChesedRuntimeSkill : ISephiraSkill, IAttackPassive, ICoolTimeSkill, IOnEnablePassive
    {
        SephiraUnit unit;

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            //게임 내 방어력 증가
            GameStatManager.Inst.AddGameDefense(0.3f);
        }

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            //일반 공격시 일정 확률로 필드내 아군 1명에게 보호막 부여
            Enemy enemy = unit.GetNearestTarget();
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.2f))
            {
                List<Unit> fieldUnit = FieldManager.Inst.GetFieldBatchUnits();

                Unit targetUnit = fieldUnit[Random.Range(0, fieldUnit.Count)];
                float shieldAmount = targetUnit.maxShield * 0.2f;
                targetUnit.unitEvent.ApplyGetShield(shieldAmount, unit.shieldMultiplier, targetUnit.shieldReceivedMultiplier);
                AudioManager.Inst.PlaySfx(AudioManager.Sfx.ChokmahManaRegen);
            }

            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage, UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.Launch(unit.nearestTarget.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, 1, 0.1f);
        }

        public void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour)
        {
            // 모든 아군에게 보호막 + 특수 보호막 
            //방어력 증가
            //아군 혹은 자신이 n회 이상 피해를 입을시 추가공격
            //피해분담 활성화
            //자신의 주변에 전자기장 방어벽 생성
        }


        public void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }


    }
}
