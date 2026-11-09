using UnityEngine;

[CreateAssetMenu(menuName = "Unit Skill Data/Yesod Epic")]
public class YesodSkill : SephiraSkillBase<YesodSkill.Runtime, ISephiraSkill>, IUnitManagerFactory
{
    public GameObject managerPrefab;
    public bool truthForm;
    [Min(0)] public float ultimateDamageMultiplier = 30;
    public YesodEpicController GetController()
    {
        if (YesodEpicController.Inst != null) return YesodEpicController.Inst;
        if (PoolManager.Inst == null || GameManager.Inst == null || managerPrefab == null) return null;
        var existing = GameManager.Inst.unitManagerGroup.GetComponentInChildren<YesodEpicController>(true);
        if (existing != null) return existing;
        var controller = PoolManager.Inst.Get(managerPrefab, Vector3.zero).GetComponent<YesodEpicController>();
        controller.transform.SetParent(GameManager.Inst.unitManagerGroup);
        return controller;
    }
    public void CreateUnitManager() { GetController(); }
    public class Runtime : ISephiraSkill, IOnEnablePassive, ICoolTimeSkill, IAttackPassive
    {
        SephiraUnit unit;
        YesodSkill settings;
        public void Initialize(SephiraUnit owner, MonoBehaviour behaviour) { unit = owner; settings = (YesodSkill)owner.skillSO; }
        public void ApplyOnEnablePassive(MonoBehaviour behaviour)
        {
            var manager = settings.GetController();
            if (manager != null) manager.Bind(unit, settings.truthForm);
        }
        public void ApplyAttackPassive(MonoBehaviour behaviour)
        {
            if (unit.nearestTarget == null || PoolManager.Inst == null) return;
            var target = unit.nearestTarget.GetComponent<Enemy>();
            if (target == null || !target.isLive) return;
            // 예소드도 기존 세피라와 같은 풀링 탄환 경로로 일반 공격을 처리합니다.
            Bullet bullet = PoolManager.Inst.bulletGenerator.GetBullet(PoolManager.PrefabType.Bullet0, unit.gameObject);
            bullet.SetAttackInfo(unit.unitData.attribute.attributeType, unit.damageAmout, unit.criticalPer, unit.criticalDamage,
            UnitUtility.AttackRangeType.Single, unit.singleTargetDamage, unit.defensePenetration,
            unit.attributeResistancePenetration, unit.attributeDamageMultiplier, UnitUtility.AttackType.NormalAttack, unit.normalAttackMultiplier);
            bullet.SetVisualEffect(EffectManager.PrefabType.EnemyHitEffect, true, AudioManager.Sfx.EnemyHit);
            bullet.Launch(target.transform.position, Bullet.BulletInitMode.HitScan, Bullet.BulletType.Single, Bullet.BulletOwnerType.Sephira, .15f, .15f);
        }
        public void ApplyCoolTimeSkill(MonoBehaviour behaviour)
        {
            var manager = settings.GetController();
            if (manager == null) return;
            manager.Bind(unit, settings.truthForm);
            if (settings.truthForm) manager.CastTruthUltimate();
            else if (manager.Ready) unit.TransInit(unit.unitData.transData);
        }
    }
}
