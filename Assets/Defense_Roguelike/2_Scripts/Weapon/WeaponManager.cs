using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Inst;

    public bool isBoomb;
    public float boombDamage;
    public float baseBoombDamage;

    public bool isInsDeath;
    public float insDeathProb;

    public bool isKillShieldEnabled;
    public float shieldAmount;

    public bool isHealthDrainEnabled;
    public float drainvalue;

    void Awake()
    {
        Inst = this;
    }

    void LateUpdate()
    {
        boombDamage = baseBoombDamage * GameStatManager.Inst.GameDamage;
    }
}
