using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScalarUXSkill", menuName = "Enemy Skill Data/ScalarUXSkill")]
public class ScalarUXSkill : ScriptableObject, IEnemyCoolTimeAttackPassive, IEnemyOnEnablePassive
{
    public void ApplyEnemyCoolTimeAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        DebuffManager.Inst.ApplyParasiticDebuff(3, enemy.damage * 0.5f, 60, 1);
    }

    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXIntro2);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXIntro1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.ScalarUXIntro0);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
    }
}
