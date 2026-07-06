using UnityEngine;

[CreateAssetMenu(fileName = "JulesDeForeSkill", menuName = "Unit Skill Data/JulesDeForeSkill")]
public class JulesDeForeSkill : UnitSkillBase<JulesDeForeSkill.JulesDeForeRunTimeSkill, IUnitSkill>
{
    public class JulesDeForeRunTimeSkill : SingleSupportSkillBase, IUnitSkill
    {
        Unit unit;

        public void Initialize(Unit unit, MonoBehaviour monoBehaviour)
        {
            this.unit = unit;
        }

        public override void ApplySupport(Unit unit, Unit targetUnit)
        {
            int myX = unit.unitDrag.curBatchFieldTrigger.fieldSlot.x;
            int myY = unit.unitDrag.curBatchFieldTrigger.fieldSlot.y;

            int targetX = targetUnit.unitDrag.curBatchFieldTrigger.fieldSlot.x;
            int targetY = targetUnit.unitDrag.curBatchFieldTrigger.fieldSlot.y;

            int dx = Mathf.Abs(myX - targetX);
            int dy = Mathf.Abs(myY - targetY);

            int distance = Mathf.Max(dx, dy);

            Debug.Log($"대상과의 거리 :{distance}");
            //속성 저항 관통 증가
            //치명타 확률 증가
            //공격 속도 증가
            //유닛의 받는 피해 증가

            //지속 치유 코루틴 실행

            //대상 사망 액션에 함수 추가
            targetUnit.unitEvent.OnDeath += CastUnlovedCurse;
        }

        public override void ClearSupport(Unit unit, Unit targetUnit)
        {
            // 대상 체력 감소

            //속성 저항 관통 감소
            //치명타 확률 감소
            //공격 속도 감소
            //유닛의 받는 피해 감소

            //대상 사망 액션에 함수 제거
            targetUnit.unitEvent.OnDeath -= CastUnlovedCurse;
        }

        private void CastUnlovedCurse()
        {
            //포어 주변 가장 가까운 적에게 총알 발사,
            //해당 총알에 맞은 적이 사망 시 주변 가장 가까운 적에게로 전이
        }
    }
}
