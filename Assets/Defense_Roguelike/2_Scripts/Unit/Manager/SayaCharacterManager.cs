using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SayaCharacterManager : MonoBehaviour
{
    public static SayaCharacterManager Inst;
    public int fieldLevel;
    public int hunterLevel;
    public int shamanLevel;
    public AnimatorOverrideController[] frontAnim;
    public AnimatorOverrideController[] middleAnim;
    public AnimatorOverrideController[] backAnim;
    public UnitData fieldData;
    public UnitData hunterData;
    public UnitData shamanData;

    [Header("행동 선언 연출")]
    public GameObject diceModel;
    public GameObject rollVisualPrefab;
    public GameObject skillEffectPrefab;
    public Sprite rollingPanel;
    public TMP_FontAsset rollingFont;
    public Material diceMaterial;
    [Min(0)] public int rollingCost = 300;
    [Min(1)] public float rollingDuration = 3.4f;
    [Min(0)] public float resultDuration = 1.5f;

    // 강화값은 매니저에 보관하여 새로 배치된 사야도 동일하게 적용받습니다.
    public float healthBonus { get; private set; }
    public float shieldBonus { get; private set; }
    public float defenseBonus { get; private set; }
    public float damageBonus { get; private set; }
    public float criticalBonus { get; private set; }
    public float criticalDamageBonus { get; private set; }
    public float attackSpeedBonus { get; private set; }
    public int specialShieldBonus { get; private set; }
    public bool IsRolling => rollingVisuals.Count > 0;
    public bool IsResourceBlocked => resourceBlockRemaining > 0;
    private float resourceBlockRemaining;
    private readonly HashSet<Unit> pendingTransforms = new HashSet<Unit>();
    private readonly Dictionary<Unit, GameObject> rollingVisuals = new Dictionary<Unit, GameObject>();
    public enum UnitType { field, hunter, shaman }

    void OnEnable()
    {
        Inst = this;
        fieldLevel = hunterLevel = shamanLevel = 0;
        healthBonus = shieldBonus = defenseBonus = damageBonus = 0;
        criticalBonus = criticalDamageBonus = attackSpeedBonus = 0;
        specialShieldBonus = 0;
    }

    void Update()
    {
        resourceBlockRemaining = Mathf.Max(0, resourceBlockRemaining - Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.P)) RollAllForDebug();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        foreach (GameObject visual in rollingVisuals.Values)
            if (visual != null) visual.SetActive(false);
        rollingVisuals.Clear();
        resourceBlockRemaining = 0;
        pendingTransforms.Clear();
        if (Inst == this) Inst = null;
    }

    public UnitType GetPosition(Unit unit)
    {
        var slot = unit.unitDrag.curBatchFieldTrigger;
        if (slot != null)
        {
            if (slot.formationRow == FieldSlot.FormationRow.Middle) return UnitType.hunter;
            if (slot.formationRow == FieldSlot.FormationRow.Back) return UnitType.shaman;
        }
        return UnitType.field;
    }

    public int GetLevel(UnitType type)
    {
        return type == UnitType.field ? fieldLevel : type == UnitType.hunter ? hunterLevel : shamanLevel;
    }

    public UnitData GetUnitData(FieldSlot.FormationRow row)
    {
        return row == FieldSlot.FormationRow.Front ? fieldData : row == FieldSlot.FormationRow.Middle ? hunterData : shamanData;
    }

    public AnimatorOverrideController GetAnimatorController(FieldSlot.FormationRow row)
    {
        UnitType type = row == FieldSlot.FormationRow.Front ? UnitType.field : row == FieldSlot.FormationRow.Middle ? UnitType.hunter : UnitType.shaman;
        var controllers = type == UnitType.field ? frontAnim : type == UnitType.hunter ? middleAnim : backAnim;
        int level = Mathf.Clamp(GetLevel(type), 0, 4);
        return controllers != null && controllers.Length > level ? controllers[level] : null;
    }

    public List<Unit> GetSayaUnits()
    {
        var units = new List<Unit>();
        foreach (var obj in FieldManager.Inst.fieldUnits)
        {
            if (obj == null) continue;
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null && unit.isLive && unit.unitSkill is SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill)
                units.Add(unit);
        }
        return units;
    }

    public void ApplyTransformNextFrame(Unit unit)
    {
        if (unit != null && pendingTransforms.Add(unit))
            StartCoroutine(ApplyTransformNextFrameCoroutine(unit, unit.unitSkill));
    }

    private IEnumerator ApplyTransformNextFrameCoroutine(Unit unit, IUnitSkill originalSkill)
    {
        yield return null;
        pendingTransforms.Remove(unit);
        if (unit == null || !unit.isLive || !unit.gameObject.activeInHierarchy || unit.unitSkill != originalSkill || unit.unitDrag.curBatchFieldTrigger == null)
            yield break;

        var row = unit.unitDrag.curBatchFieldTrigger.formationRow;
        UnitData data = GetUnitData(row);
        if (data == null) yield break;
        // TransInit은 체력과 강화, 상태이상을 초기화하므로 같은 사야의 형태만 갱신합니다.
        float healthRatio = unit.health / Mathf.Max(unit.maxHealth, 0.0001f);
        unit.unitData = data;
        unit.gameObject.name = data.name;
        unit.anim.runtimeAnimatorController = GetAnimatorController(row) ?? data.animController;
        unit.RecalculateStats();
        unit.health = unit.maxHealth * healthRatio;
        unit.UpdateHealthBar();
        ((SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill)unit.unitSkill).RefreshSharedShield();
    }

    public void UpgradeUnit(UnitType type)
    {
        if (GetLevel(type) >= 4) return;
        if (type == UnitType.field) fieldLevel++;
        else if (type == UnitType.hunter) hunterLevel++;
        else shamanLevel++;
        foreach (Unit unit in GetSayaUnits())
        {
            if (GetPosition(unit) != type) continue;
            ApplyTransformNextFrame(unit);
            ForeSkillEffect.Play(skillEffectPrefab, unit.transform.position, 2, new Color(1, 0.8f, 0.2f), 1.15f, 0.8f);
        }
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Trans);
    }

    public void FillFieldForDebug()
    {
        if (!CanDebugField()) return;
        // 테스트 필드만 교체하며 영구 덱·보유 카드 데이터는 수정하지 않습니다.
        var existing = new List<Unit>();
        foreach (GameObject obj in FieldManager.Inst.fieldUnits)
            if (obj != null && obj.TryGetComponent<Unit>(out var occupant)) existing.Add(occupant);
        foreach (Unit unit in existing)
        {
            if (unit == null || unit.unitSkill is SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill) continue;
            unit.unitEvent.IsIgnored = false;
            unit.UnitDeActive();
        }
        int empty = FieldManager.Inst.GetEmptyUnitSlots().Count;
        for (int i = 0; i < empty; i++) GameManager.Inst.unitSpawn.GenerateUnit(fieldData);
        foreach (Unit unit in GetSayaUnits()) ApplyTransformNextFrame(unit);
    }

    public void MaxFieldForDebug()
    {
        if (!CanDebugField()) return;
        fieldLevel = hunterLevel = shamanLevel = 4;
        foreach (GameObject obj in FieldManager.Inst.fieldUnits)
        {
            if (obj == null || !obj.TryGetComponent<Unit>(out var unit) || !unit.isLive) continue;
            unit.unitLevel.SetLevel(unit.unitLevel.maxLevel, true);
            if (unit.unitSkill is SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill) ApplyTransformNextFrame(unit);
        }
    }

    static bool CanDebugField()
    {
        return GameManager.Inst != null && GameManager.Inst.isGameStart && !GameManager.Inst.isPause
            && FieldManager.Inst != null && PoolManager.Inst != null;
    }

    public static SayaCharacterManager GetForDebug()
    {
        if (!CanDebugField()) return null;
        if (Inst != null) return Inst;
        var obj = PoolManager.Inst.Get(PoolManager.PrefabType.SayaCharacterManager, Vector3.zero);
        obj.transform.SetParent(GameManager.Inst.unitManagerGroup);
        return obj.GetComponent<SayaCharacterManager>();
    }

    public void RollAllForDebug()
    {
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause) return;
        // 개발 확인용 단축키이므로 코인과 게이지를 소비하지 않습니다.
        foreach (Unit caster in GetSayaUnits()) TryRolling(caster, true);
    }

    public void TryRolling(Unit caster, bool debugRoll = false)
    {
        if (caster == null || !caster.isLive || !caster.gameObject.activeInHierarchy || rollingVisuals.ContainsKey(caster)) return;
        if (!GameManager.Inst.isGameStart || GameManager.Inst.isPause) return;
        if (!debugRoll)
        {
            if (caster.unitEvent.IsIgnored || caster.unitEvent.IsSilenced || caster.unitEvent.IsUnitLocked) return;
            if (GameManager.Inst.coin < rollingCost || caster.unitWeapon.skillTime < caster.coolTime) return;
            GameManager.Inst.ModifyCoin(-rollingCost);
            caster.unitWeapon.skillTime = 0;
            caster.UpdateManaBar();
        }
        // 유닛별로 잠가 P 연타와 버튼 중복 입력만 차단합니다.
        rollingVisuals.Add(caster, null);
        StartCoroutine(Roll(caster, caster.unitSkill, GetPosition(caster)));
    }

    private IEnumerator Roll(Unit caster, IUnitSkill originalSkill, UnitType position)
    {
        int value = Random.Range(0, 101);
        GameObject rollingVisual = PoolManager.Inst.Get(rollVisualPrefab, caster.transform.position);
        rollingVisuals[caster] = rollingVisual;
        var ui = rollingVisual.GetComponent<SayaRollVisual>().Prepare(this, value, caster);
        Color positionColor = position == UnitType.field ? new Color(0.2f, 0.8f, 1)
            : position == UnitType.hunter ? new Color(1, 0.2f, 0.55f) : new Color(0.65f, 0.25f, 1);
        ForeSkillEffect.Play(skillEffectPrefab, caster.transform.position, 3, positionColor, 0.65f, rollingDuration);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.spin0);
        yield return new WaitForSeconds(rollingDuration);

        // 소환 해제되거나 다른 캐릭터가 된 시전자의 예약 효과는 취소합니다.
        if (caster != null && caster.isLive && caster.gameObject.activeInHierarchy && caster.unitSkill == originalSkill && !caster.unitEvent.IsIgnored && !caster.unitEvent.IsSilenced)
        {
            ui.ShowResult(value, GetResultName(position, value));
            bool fumble = value <= 10;
            ForeSkillEffect.Play(skillEffectPrefab, caster.transform.position, 2, fumble ? new Color(1, 0.1f, 0.2f) : positionColor, value >= 91 ? 1.15f : 0.8f, 0.8f);
            AudioManager.Inst.PlaySfx(fumble ? AudioManager.Sfx.Fail : AudioManager.Sfx.Success0);
            ApplyResult(caster, position, value);
            if (caster.isLive && caster.gameObject.activeInHierarchy && caster.unitSkill == originalSkill)
            {
                ((SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill)originalSkill).ApplyShamanSkillRecovery();
                InGameEventManager.Inst.UnitSkillEvent?.Invoke(caster);
            }
        }
        else ui.ShowResult(value, "행동 취소");

        yield return new WaitForSeconds(resultDuration);
        rollingVisual.SetActive(false);
        rollingVisuals.Remove(caster);
    }

    public static int GetResultIndex(int value)
    {
        if (value == 1) return 0;
        if (value <= 10) return 1;
        if (value <= 30) return 2;
        if (value <= 50) return 3;
        if (value <= 70) return 4;
        if (value <= 90) return 5;
        return 6;
    }

    public static string GetResultName(UnitType position, int value)
    {
        string[] names = position == UnitType.field
        ? new[] { "펌블 : 종말", "펌블 : 즉사", "생명 주입", "인내력 강화", "저항력 상승", "이상현상 적응력 상승", "올 세이브 챌린지 루트" }
        : position == UnitType.hunter
        ? new[] { "펌블 : 뇌사", "펌블 : 통제", "폭력성 상승", "인지 불가력 상승", "부도덕성 상승", "통제 불가력 상승", "엔딩 : COC" }
        : new[] { "펌블 : 실업", "펌블 : 공제", "보물 수령", "혼돈 결정", "잠재력 강화", "기술 습득", "엔딩 : 룰치킨" };
        return names[GetResultIndex(value)];
    }

    private void ApplyResult(Unit caster, UnitType position, int value)
    {
        var settings = (SayaTreasureFieldSkill)caster.skillSO;
        int result = GetResultIndex(value);
        if (result == 6) { UpgradeUnit(position); return; }
        if (position == UnitType.field)
        {
            if (result == 0)
            {
                // 묵살 상태도 포함하되 영구적으로 묵살되는 블록 오브젝트는 제외합니다.
                var victims = new List<Unit>();
                foreach (var obj in FieldManager.Inst.fieldUnits)
                {
                    if (obj == null) continue;
                    var victim = obj.GetComponent<Unit>();
                    if (victim != null && victim.isLive && !(victim.unitSkill is BlockUnitSkill.BlockUnitRuntimeSkill)) victims.Add(victim);
                }
                foreach (Unit victim in victims) Disassemble(victim);
            }
            else if (result == 1) Disassemble(caster);
            else if (result == 2) { healthBonus += settings.healthGain; RefreshSharedStats(true); }
            else if (result == 3) { shieldBonus += settings.shieldGain; RefreshSharedStats(false, true); }
            else if (result == 4) { defenseBonus += settings.defenseGain; RefreshSharedStats(); }
            else { specialShieldBonus++; RefreshSharedStats(); }
        }
        else if (position == UnitType.hunter)
        {
            if (result == 0)
            {
                foreach (var obj in FieldManager.Inst.fieldUnits)
                {
                    if (obj == null) continue;
                    var victim = obj.GetComponent<Unit>();
                    if (victim != null && victim.isLive) victim.unitEvent.ApplyIgnoredUnit(10);
                }
            }
            else if (result == 1) caster.unitEvent.ApplyIgnoredUnit(settings.selfIgnoreDuration);
            else if (result == 2) { damageBonus += settings.damageGain; RefreshSharedStats(); }
            else if (result == 3) { criticalBonus += settings.criticalGain; RefreshSharedStats(); }
            else if (result == 4) { criticalDamageBonus += settings.criticalDamageGain; RefreshSharedStats(); }
            else { attackSpeedBonus += settings.attackSpeedGain; RefreshSharedStats(); }
        }
        else
        {
            if (result == 0) resourceBlockRemaining = Mathf.Max(resourceBlockRemaining, settings.resourceBlockDuration);
            else if (result == 1)
            {
                GameManager.Inst.ModifyCoin(-5000);
                GameManager.Inst.ModifyDarkMatteCoin(-10);
            }
            else if (result == 2) GameManager.Inst.ModifyCoin(Random.Range(0, 5001));
            else if (result == 3) GameManager.Inst.ModifyDarkMatteCoin(Random.Range(0, 31));
            else if (result == 4) GameManager.Inst.skillCardManager.GetSkillCard(1);
            else GameManager.Inst.cheatCodeManager.ResetAllCooldowns();
        }
    }

    public static void Disassemble(Unit victim)
    {
        // 분해는 사망 유예와 특수 보호막을 무시합니다.
        victim.unitEvent.IsIgnored = false;
        ForeSkillEffect.Play(Inst != null ? Inst.skillEffectPrefab : null, victim.transform.position, 2, new Color(1, 0.15f, 0.2f), 1.6f, 0.7f);
        victim.Dead();
    }

    private void RefreshSharedStats(bool healAddedHealth = false, bool fillShield = false)
    {
        foreach (Unit unit in GetSayaUnits())
        {
            float oldMaxHealth = unit.maxHealth;
            unit.RecalculateStats();
            if (healAddedHealth) unit.health = Mathf.Min(unit.maxHealth, unit.health + unit.maxHealth - oldMaxHealth);
            if (fillShield && !unit.unitEvent.IsIgnored)
                unit.unitEvent.ApplyGetShield(unit.maxShield, unit.shieldMultiplier, unit.shieldReceivedMultiplier);
            ((SayaTreasureFieldSkill.SayaTreasureFieldRuntimeSkill)unit.unitSkill).RefreshSharedShield();
            unit.UpdateHealthBar();
        }
    }

    public float AbsorbAllyDamage(Unit victim, float damage)
    {
        if (fieldLevel < 1 || damage <= 0) return damage;
        var tanks = GetSayaUnits();
        tanks.RemoveAll(u => u == victim || GetPosition(u) != UnitType.field || u.unitEvent.IsIgnored || u.unitEvent.IsInvulnerable);
        if (tanks.Count == 0) return damage;
        var settings = (SayaTreasureFieldSkill)tanks[0].skillSO;
        float absorbed = damage * settings.allyDamageAbsorption;
        // DamageCoroutine을 사용하여 흡수 피해가 다시 분산되는 재귀를 방지합니다.
        foreach (Unit tank in tanks) tank.StartCoroutine(tank.DamageCoroutine(absorbed / tanks.Count, null, false));
        return damage - absorbed;
    }
}
