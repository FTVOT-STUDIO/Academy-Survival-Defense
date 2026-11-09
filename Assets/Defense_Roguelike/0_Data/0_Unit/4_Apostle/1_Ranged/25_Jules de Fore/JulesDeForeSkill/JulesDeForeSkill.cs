using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JulesDeForeSkill", menuName = "Unit Skill Data/JulesDeForeSkill")]
public class JulesDeForeSkill : UnitSkillBase<JulesDeForeSkill.JulesDeForeRunTimeSkill, IUnitSkill>
{
    public GameObject effectPrefab;
    [Header("사랑이라는 축복")]
    [Range(0, 0.2f)] public float penetrationPerTiferet = 0.03f;
    [Header("애착")]
    [Range(0, 1)] public float attachmentPenetration = 0.15f;
    [Range(0, 2)] public float attachmentAttackSpeed = 0.25f;
    [Range(0, 100)] public float attachmentCriticalChance = 20;
    [Range(0, 2)] public float attachmentDamageTaken = 0.25f;
    [Range(0, 1)] public float healingPerSecond = 0.04f;
    [Range(0, 1)] public float separationHealthLoss = 0.65f;
    [Header("사랑받지 못한다는 저주")]
    [Min(0)] public float curseDamageMultiplier = 20;
    [Min(0.1f)] public float curseDelay = 1;
    [Range(0, 1)] public float curseTransferMultiplier = 0.5f;

    // 원본 스탯을 수정하지 않아 재계산·풀 재사용 때 강화가 누적되지 않습니다.
    static readonly HashSet<JulesDeForeRunTimeSkill> activeSkills = new HashSet<JulesDeForeRunTimeSkill>();

    public static void ApplyBonuses(Unit target)
    {
        float penetration = 0, speed = 0, critical = 0, damageTaken = 0;
        foreach (var skill in activeSkills)
        {
            if (!skill.IsActive) continue;
            penetration += skill.DeckBlessing();
            if (skill.target != target || skill.targetSkill != target.unitSkill) continue;
            penetration += skill.data.attachmentPenetration;
            speed += skill.data.attachmentAttackSpeed;
            critical += skill.data.attachmentCriticalChance;
            damageTaken += skill.data.attachmentDamageTaken;
        }
        target.attributeResistancePenetration = Mathf.Clamp01(target.attributeResistancePenetration + penetration);
        target.attackSpeed = Mathf.Max(0.03f, target.attackSpeed / (1 + speed));
        target.criticalPer += critical;
        target.damageTakenIncrease *= 1 + damageTaken;
    }

    public class JulesDeForeRunTimeSkill : SingleSupportSkillBase, IUnitSkill, IOnEnablePassive, IOnDisablePassive, IUpdatePassive
    {
        Unit unit;
        internal Unit target;
        internal IUnitSkill targetSkill;
        internal JulesDeForeSkill data;
        float healTime;
        Coroutine blessingRoutine;
        GameObject bond;
        uint bondLease;
        internal bool IsActive => unit != null && unit.isLive && unit.gameObject.activeInHierarchy && unit.unitSkill == this
            && !unit.unitEvent.IsIgnored && !unit.unitEvent.IsSilenced;

        public void Initialize(Unit owner, MonoBehaviour monoBehaviour)
        {
            unit = owner;
            data = owner.skillSO as JulesDeForeSkill;
        }

        internal float DeckBlessing()
        {
            var deck = DeckManager.Inst != null ? DeckManager.Inst.currentInGameDeck?.unitdatas : null;
            int count = 0;
            if (deck != null)
                foreach (var member in deck)
                    if (member != null && member.sephiraPathData != null
                        && member.sephiraPathData.sephirahPath == SephiraPathData.SephirahPath.Tiferet) count++;
            return count * data.penetrationPerTiferet;
        }

        public void ApplyOnEnablePassive(MonoBehaviour monoBehaviour)
        {
            activeSkills.Add(this);
            if (blessingRoutine != null) unit.StopCoroutine(blessingRoutine);
            blessingRoutine = unit.StartCoroutine(RefreshBlessing());
            RefreshAllies();
        }

        public void ApplyOnDisablePassive(MonoBehaviour monoBehaviour)
        {
            activeSkills.Remove(this);
            if (blessingRoutine != null && unit != null) unit.StopCoroutine(blessingRoutine);
            blessingRoutine = null;
            ClearSupport(unit, target);
            RefreshAllies();
        }

        public override void ApplySupport(Unit owner, Unit recipient)
        {
            if (recipient == null || recipient == target || !recipient.isLive) return;
            ClearSupport(owner, target);
            target = recipient;
            targetSkill = recipient.unitSkill;
            target.unitEvent.OnDeath += OnAttachmentDeath;
            healTime = 0;
            target.RecalculateStats();
            bond = ForeSkillEffect.CreateBond(data.effectPrefab, owner.transform, recipient.transform);
            if (bond != null) bondLease = bond.GetComponent<ForeSkillEffect>().Lease;
        }

        public override void ClearSupport(Unit owner, Unit recipient)
        {
            if (target == null) return;
            Unit previous = target;
            bool sameLife = previous.unitSkill == targetSkill;
            previous.unitEvent.OnDeath -= OnAttachmentDeath;
            target = null;
            targetSkill = null;
            if (bond != null && bond.GetComponent<ForeSkillEffect>().Lease == bondLease) bond.SetActive(false);
            bond = null;
            // 스탯 갱신이 아닌 실제 연결 종료 때만 현재 체력을 잃습니다.
            if (sameLife)
            {
                if (previous.isLive && GameManager.Inst != null && GameManager.Inst.isGameStart)
                    previous.health *= 1 - data.separationHealthLoss;
                previous.RecalculateStats();
            }
        }

        public override void ApplySupportBuffUpdate(Unit owner, Unit recipient)
        {
            owner.RecalculateStats();
            if (target != null) target.RecalculateStats();
        }

        public void ApplyUpdatePassive(MonoBehaviour monoBehaviour)
        {
            if (GameManager.Inst == null || !GameManager.Inst.isGameStart || GameManager.Inst.isPause) return;
            if (target == null) return;
            if (!target.isLive || !target.gameObject.activeInHierarchy || target.unitSkill != targetSkill)
            {
                ClearSupport(unit, target);
                return;
            }
            if (!IsActive || target.unitEvent.IsIgnored) return;
            healTime += Time.deltaTime;
            if (healTime < 1) return;
            healTime -= 1;
            int distance = GridDistance(unit, target);
            float amount = target.maxHealth * data.healingPerSecond / Mathf.Max(1, distance);
            target.unitEvent.ApplyGetHeal(amount, unit.healingMultiplier, target.healingReceivedMultiplier, unit.unitData);
        }

        public static int GridDistance(Unit a, Unit b)
        {
            var first = a.unitDrag.curBatchFieldTrigger;
            var second = b.unitDrag.curBatchFieldTrigger;
            if (first == null || second == null) return 1;
            return Mathf.Max(Mathf.Abs(first.fieldSlot.x - second.fieldSlot.x), Mathf.Abs(first.fieldSlot.y - second.fieldSlot.y));
        }

        void OnAttachmentDeath()
        {
            if (IsActive)
                ForeSkillEffect.CastCurse(unit, data);
            ClearSupport(unit, target);
        }

        IEnumerator RefreshBlessing()
        {
            var interval = new WaitForSeconds(0.5f);
            while (unit != null && unit.unitSkill == this && unit.gameObject.activeInHierarchy)
            {
                yield return interval;
                // 묵살 중에도 다른 아군에게 남아 있는 축복 수치를 제거합니다.
                RefreshAllies();
            }
        }

        static void RefreshAllies()
        {
            if (FieldManager.Inst == null) return;
            foreach (var obj in FieldManager.Inst.fieldUnits)
                if (obj != null && obj.TryGetComponent<Unit>(out var ally) && ally.isLive) ally.RecalculateStats();
        }
    }
}
