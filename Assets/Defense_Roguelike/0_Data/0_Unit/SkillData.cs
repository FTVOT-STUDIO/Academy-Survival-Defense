using UnityEngine;

public interface IUnitSkillBase
{
    IUnitSkill GenerateRuntimeSkill();
}

public abstract class UnitSkillBase<TSkill, TInterface> : ScriptableObject, IUnitSkillBase
    where TSkill : class, TInterface, new()
{
    public TInterface GenerateRuntimeSkill()
    {
        return new TSkill();
    }

    IUnitSkill IUnitSkillBase.GenerateRuntimeSkill()
    {
        return GenerateRuntimeSkill() as IUnitSkill;
    }
}

public interface ISephiraSkillBase
{
    ISephiraSkill GenerateRuntimeSkill();
}

public abstract class SephiraSkillBase<TSkill, TInterface> : ScriptableObject, ISephiraSkillBase
    where TSkill : class, TInterface, new()
{
    public TInterface GenerateRuntimeSkill()
    {
        return new TSkill();
    }

    ISephiraSkill ISephiraSkillBase.GenerateRuntimeSkill()
    {
        return GenerateRuntimeSkill() as ISephiraSkill;
    }
}

// coolTime Skill
public interface ICoolTimeSkill
{
    void ApplyCoolTimeSkill(MonoBehaviour monoBehaviour);
}

// Attack Skill
public interface IAttackSkill
{
    void ApplyIAttackSkill(MonoBehaviour monoBehaviour);
}

public interface ICoolTimeOverride
{

}

public interface IAttackOverride
{
    void ApplyAttackOverride(MonoBehaviour monoBehaviour);
}
// OnEnable Passive
public interface IOnEnablePassive
{
    void ApplyOnEnablePassive(MonoBehaviour monoBehaviour);
}

public interface IOnEnableSkill
{
    void ApplyOnEnableSkill(MonoBehaviour monoBehaviour);
}

public interface IOnDisablePassive
{
    void ApplyOnDisablePassive(MonoBehaviour monoBehaviour);
}


public interface IOnDisableSkill
{
    void ApplyOnDisableSkill(MonoBehaviour monoBehaviour);
}

public interface IDeActiveSkill
{
    void ApplyDeActiveSkill(MonoBehaviour monoBehaviour);
}

public interface IDeActivePassive
{
    void ApplyDeActivePassive(MonoBehaviour monoBehaviour);
}
public interface ITriggerBeforeTransform
{
    void OnTriggerBeforeTransform();
}
public interface ITriggerAfterTransform
{
    void OnTriggerAfterTransform();
}
public interface IOnDiePassive
{
    void ApplyOnDiePassive(MonoBehaviour monoBehaviour);
}

public interface IOnDieSkill
{
    void ApplyOnDieSkill(MonoBehaviour monoBehaviour);
}

public interface IOnUnitDeathOverride
{
    void ApplyOnUnitDeathOverride(MonoBehaviour monoBehaviour);
}


// CoolTime Passive
public interface ICoolTimePassive
{
    void ApplyPassive(MonoBehaviour monoBehaviour);
}

public interface IUpdatePassive
{
    void ApplyUpdatePassive(MonoBehaviour monoBehaviour);
}

// Attack Passive

public interface IAttackPassive
{
    void ApplyAttackPassive(MonoBehaviour monoBehaviour);
}

public interface ISupportApplyOnce
{
    void ApplySupportApplyOnce(Unit unit);
}

public interface ISingleSupportBuffUpdate
{
    void ApplySupportBuffUpdate(Unit unit, Unit targetUnit);
}


public interface IMultSupportBuffUpdate
{
    void ApplySupportBuffUpdate(Unit unit);
}

public interface IButtonPassive
{
    void ApplyButtonPassive(MonoBehaviour monoBehaviour);
}

public interface IButtonAttckPassive
{
    void ApplyButtonAttckPassive(MonoBehaviour monoBehaviour);
}

public interface IButtonAttckSkill
{
    void ApplyButtonAttckSkill(MonoBehaviour monoBehaviour);
}


public interface IButtonInfoinitialize
{
    /// <summary>
    /// 유닛 정보에 기반하여 스킬 버튼 UI를 설정합니다.
    /// <para/>
    /// unit.unitInfo.InitializeSkillButton() 호출해서 사용
    /// </summary>
    void InitializeSkillButtonUI(MonoBehaviour monoBehaviour);
}

public interface IUnitManagerFactory
{
    void CreateUnitManager();
}

public interface ITransformationState
{
    bool IsTransformationInProgress { get; set; }
}

public interface IFieldManagerFactory
{
    void CreateFieldManager();
}

public interface IUnitManagerInitialization
{
    void Init();
}

public interface IUnitFieldBatchPassive
{
    void ApplyUnitFieldBatchPassive(MonoBehaviour monoBehaviour);
}

public interface IUnitFieldBatchSkill
{
    void ApplyUnitFieldBatchSkill(MonoBehaviour monoBehaviour);
}

public interface IUnitSkill
{
    /// <summary>
    /// this.unit = unit; 으로 사용.
    /// <para/>
    /// SO 변수 참조 필요시,
    /// <para/>
    /// 클래스명 변수 이름
    /// <para/>
    /// 변수 이름 = unit.skillSO as 클래스명; 으로 사용.
    /// </summary>
    void Initialize(Unit unit, MonoBehaviour monoBehaviour);
}

public interface ISephiraSkill
{
    void Initialize(SephiraUnit unit, MonoBehaviour monoBehaviour);
}

public interface IAbilityOverride
{
    void ApplyAbilityOverride();
}

public interface IDamageModifier
{
    float GetDamageMultiplier();
}

public interface ISkillModifier
{
    float GetSkillMultiplier();
}

public interface IDefenseModifier
{
    float GetDefenseMultiplier();
}

public interface IMaxHealthModifier
{
    float GetMaxHealthMultiplier();
}

public interface IMaxShieldModifier
{
    float GetMaxShieldMultiplier();
}

public interface IAttackSpeedModifier
{
    float GetAttackSpeedMultiplier();
}

public interface ICoolTimeModifier
{
    float GetCoolTimeMultiplier();
}

public interface IRangeModifier
{
    float GetRangeMultiplier();
}

public interface IHitChanceModifier
{
    float GetHitChanceMultiplier();
}

public interface ICriticalPerModifier
{
    float GetCriticalPerMultiplier();
}

public interface ICriticalDamageModifier
{
    float GetCriticalDamageMultiplier();
}

public interface IResetSingleSupportSkill
{
    void ResetSupport(Unit unit, Unit targetUnit);
}
public interface IResetMultSupportSkill
{
    void ResetSupport(Unit unit);
}

public interface IEnemyOnEnablePassive
{
    void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour);
}

public interface IEnemyAttackPassive
{
    void ApplyEnemyAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour);
}

public interface IEnemyCoolTimeAttackPassive
{
    void ApplyEnemyCoolTimeAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour);
}

public interface IWaitForAnimationEvent
{
    void ApplyWaitForAnimationEvent(Enemy enemy, MonoBehaviour monoBehaviour);
}

public interface IEnemyDeathPassive
{
    void ApplyEnemyDeathPassive(Enemy enemy, MonoBehaviour monoBehaviour);
}

public interface IFieldOnEnablePassive
{
    void ApplyFieldOnEnablePassive(FieldData fieldData, MonoBehaviour monoBehaviour);
}

public interface IKillEvent
{
    void ApplyKillEvent(MonoBehaviour monoBehaviour);
}


public interface ISupportDetectable
{
}





