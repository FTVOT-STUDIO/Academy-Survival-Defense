using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CyclopsBossSkill", menuName = "Enemy Skill Data/CyclopsBossSkill")]
public class CyclopsSkill : ScriptableObject, IEnemyCoolTimeAttackPassive, IEnemyOnEnablePassive
{
    public void ApplyEnemyCoolTimeAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        DebuffManager.Inst.ApplySyntaxErrorDebuff(3, 0.5f, 0.5f, 120);
    }

    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsIntro0);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.CyclopsIntro1);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
    }
}
