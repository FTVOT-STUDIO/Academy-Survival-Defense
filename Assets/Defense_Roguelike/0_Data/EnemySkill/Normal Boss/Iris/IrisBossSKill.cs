using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "IrisBossSKill", menuName = "Enemy Skill Data/IrisBossSKill")]
public class IrisBossSKill : ScriptableObject, IEnemyCoolTimeAttackPassive, IEnemyOnEnablePassive
{
    public void ApplyEnemyCoolTimeAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(OnSkill(enemy));
    }

    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisApear0);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisApear1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisApear2);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.NetzachAbility4);

    }

    IEnumerator OnSkill(Enemy enemy)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Siren1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisSkill);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisSkill1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisSkill2);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Explo7);

        yield return new WaitForSeconds(0.5f);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisSpin);
        enemy.enemyWeapon.isSkillActive = true;
        enemy.anim.SetBool("isTrans", true);

        int activeTime = 5;
        Vector3 spawnPos = new Vector3(-0.5f, 2.1f, 0);
        EnemyBullet irisWeapon = PoolManager.Inst.Get(PoolManager.PrefabType.IrisWeapon, spawnPos).GetComponent<EnemyBullet>();

        irisWeapon.transform.SetParent(enemy.weaponPrefabParent, true);

        irisWeapon.SetAttackInfo(enemy.damage / 10);
        irisWeapon.SetVisualEffect(true, AudioManager.Sfx.damaged);
        irisWeapon.Launch(spawnPos, enemy, activeTime);
        irisWeapon.transform.localPosition = spawnPos;

        DOTweenAnimation dOTweenAnimation = irisWeapon.gameObject.GetComponent<DOTweenAnimation>();

        FlipXPositionSync flipXPositionSync = irisWeapon.GetComponent<FlipXPositionSync>();
        flipXPositionSync.parentSpriteRenderer = enemy.sp;
        flipXPositionSync.baseLocalPosition = spawnPos;

        SortingOrderController sortingOrderController = irisWeapon.GetComponent<SortingOrderController>();
        sortingOrderController.Init(enemy.sp);

        dOTweenAnimation.DORestartById("1");
        yield return new WaitForSeconds(activeTime);

        irisWeapon.gameObject.SetActive(false);
        enemy.enemyWeapon.isSkillActive = false;
        enemy.anim.SetBool("isTrans", false);
        enemy.anim.SetTrigger("Skill");
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.IrisSkill2);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.OnWaveChanged);
    }
}
