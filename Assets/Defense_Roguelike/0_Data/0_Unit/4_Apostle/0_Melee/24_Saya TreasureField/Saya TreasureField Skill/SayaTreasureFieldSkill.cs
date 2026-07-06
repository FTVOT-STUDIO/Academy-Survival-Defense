using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "SayaTreasureFieldSkill", menuName = "Unit Skill Data/SayaTreasureFieldSkill")]
public class SayaTreasureFieldSkill : UnitSkillBase<SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill, IUnitSkill>, IUnitManagerFactory
{
    public ResourcesData resourcesData;
    int batchCount = 0;
    public void CreateUnitManager()
    {
        GameObject manager = PoolManager.Inst.Get(PoolManager.PrefabType.SayaCharacterManager, Vector3.zero);
        manager.transform.SetParent(GameManager.Inst.unitManagerGroup);
    }

    public class SayaTreasureFieldRuntimeSkill : IUnitSkill, IAttackPassive, IButtonAttckSkill, IButtonInfoinitialize, IUnitFieldBatchPassive
    {
        SayaTreasureFieldSkill sayaTreasureFieldSkill;

        Unit unit;
        int combo = 0;

        public void ApplyAttackPassive(MonoBehaviour monoBehaviour)
        {
            Enemy target = unit.unitScanner.GetCurrentTarget();

            if (target == null)
                return;

            double finalDamage = 0;

            if (Random.value <= UnitUtility.CalculateSkillTrigger(unit.skillTriggerChance, 0.5f))
            {
                combo++;
                Debug.Log("강화 공격 발동");
                Debug.Log(combo + "Combo");

                finalDamage = unit.damageAmout * 3;
            }
            else
            {
                if (combo > 0)
                {
                    Debug.Log(combo + "만큼 쿨타임을 회복함");
                    combo = 0;
                }

                finalDamage = unit.damageAmout;
                Debug.Log("평타");
            }

            unit.unitWeapon.TakeDamage(unit.unitData.attribute.attributeType, finalDamage, unit.criticalPer, unit.criticalDamage, unit.singleTargetDamage, unit.defensePenetration, unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier, target);
        }

        public void ApplyButtonAttckSkill(MonoBehaviour monoBehaviour)
        {
            SayaCharacterManager.Inst.TryRolling();
        }

        public void InitializeSkillButtonUI(MonoBehaviour monoBehaviour)
        {
            int cost = SayaCharacterManager.Inst.rollingCost;
            unit.unitInfo.InitializeSkillButton("주사위 롤링", true, cost.ToString(), true, sayaTreasureFieldSkill.resourcesData.icon);
        }

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
            unit.unitEvent.IsUnitLocked = true;
            sayaTreasureFieldSkill = unit.skillSO as SayaTreasureFieldSkill;
        }

        public void ApplyUnitFieldBatchPassive(MonoBehaviour monoBehaviour)
        {
            SayaCharacterManager.Inst.ApplyTransformNextFrame(unit);
        }
    }
}
