using Unity.VisualScripting;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    Enemy enemy;
    public float coolTime;
    public float attackTime;
    public float skillTime;
    public float baseCoolTime = 1;

    public bool isSkillActive;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (!enemy.isLive || enemy.enemyEvent.isStunned || GameManager.Inst.isPause || enemy.enemyMove.isBarrierTouched || isSkillActive)
            return;

        attackTime += Time.deltaTime;
        skillTime += Time.deltaTime;

        if (skillTime >= coolTime && !enemy.enemyEvent.isCharmed)
        {
            enemy.enemyBehavior.UseSkill();
            skillTime = 0;
        }

        if (enemy.enemyMove.isInContactWithTarget)
        {
            enemy.enemyBehavior.PerformAttack();
        }
    }
    void LateUpdate()
    {
        coolTime = Mathf.Max(enemy.enemyData.coolTime * baseCoolTime * GameStatManager.Inst.GameBossCoolTime, 1);
    }

    public void OnAttack()
    {
        if (isSkillActive)
            return;

        if (enemy.enemyEvent.isCharmed)
        {
            if (enemy.enemyScanner.nearestTarget.GetComponent<Enemy>())
            {
                Enemy target = enemy.enemyScanner.nearestTarget.GetComponent<Enemy>();
                if (attackTime >= enemy.attackSpeed)
                {
                    enemy.anim.SetTrigger("Attack");
                    double damage = target.health * 0.1f;
                    target.OnDamage(AttributeData.AttributeType.Fixed, damage, 0, 0);
                    attackTime = 0;
                }
            }
        }
        else
        {
            if (enemy.enemyScanner.nearestTarget.GetComponent<Unit>())
            {
                Unit unit = enemy.enemyScanner.nearestTarget.GetComponent<Unit>();
                if (attackTime >= enemy.attackSpeed && !unit.isHit)
                {

                    if (enemy.enemySkill is IEnemyAttackPassive enemyAttackPassive)
                    {
                        enemyAttackPassive.ApplyEnemyAttackPassive(enemy, this);
                    }

                    enemy.anim.SetTrigger("Attack");
                    unit.OnDamage(enemy.damage);
                    attackTime = 0;
                }
            }
            else if (enemy.enemyScanner.nearestTarget.GetComponent<SephiraUnit>())
            {
                SephiraUnit sephiraUnit = enemy.enemyScanner.nearestTarget.GetComponent<SephiraUnit>();
                if (attackTime >= enemy.attackSpeed && !sephiraUnit.isHit)
                {
                    if (enemy.enemySkill is IEnemyAttackPassive enemyAttackPassive)
                    {
                        enemyAttackPassive.ApplyEnemyAttackPassive(enemy, this);
                    }

                    enemy.anim.SetTrigger("Attack");
                    sephiraUnit.Ondamage(enemy.damage);
                    attackTime = 0;
                }
            }
        }
    }


    public void OnSkill()
    {
        if (enemy.enemySkill is IEnemyCoolTimeAttackPassive CoolTimeskill)
        {
            CoolTimeskill.ApplyEnemyCoolTimeAttackPassive(enemy, enemy);
            enemy.anim.SetTrigger("Skill");
        }
    }

}
