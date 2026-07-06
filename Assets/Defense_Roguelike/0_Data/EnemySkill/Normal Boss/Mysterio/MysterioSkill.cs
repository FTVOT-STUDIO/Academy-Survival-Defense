using UnityEngine;

[CreateAssetMenu(fileName = "MysterioSkill", menuName = "Enemy Skill Data/MysterioSkill")]
public class MysterioSkill : ScriptableObject, IEnemyCoolTimeAttackPassive, IEnemyOnEnablePassive
{
    public void ApplyEnemyOnEnablePassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.EliteEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.SpawnEnemy);
        AudioManager.Inst.PlaySfx(AudioManager.Sfx.Noise);
    }

    public void ApplyEnemyCoolTimeAttackPassive(Enemy enemy, MonoBehaviour monoBehaviour)
    {
        DebuffManager.Inst.ApplyRandomizeDebuff(FieldManager.Inst.fieldSlots.Length);
    }
}
