using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public Enemy owner;
    public float lifeTime;
    public bool isPlaySfx;
    public AudioManager.Sfx hitSound;

    void OnEnable()
    {
        CancelInvoke();

        owner = null;

        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        isPlaySfx = false;
    }

    public void SetAttackInfo(float damage)
    {
        this.damage = damage;
    }

    public void SetVisualEffect(bool isPlaySfx = true, AudioManager.Sfx hitSound = AudioManager.Sfx.EnemyHit)
    {
        this.isPlaySfx = isPlaySfx;
        this.hitSound = hitSound;
    }

    public void Launch(Vector3 positionOrDirection, Enemy owner, float lifeTime = 0)
    {
        if (positionOrDirection == null) return;
        CancelInvoke();
        this.lifeTime = lifeTime;
        this.owner = owner;
        transform.position = positionOrDirection;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.activeInHierarchy) return;


        if (collision.gameObject.layer == LayerMask.NameToLayer("Unit HitBox"))
        {
            Unit unit = collision.gameObject.GetComponentInParent<Unit>();
            unit.OnDamage(damage, owner);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Sephira HitBox"))
        {
            SephiraUnit sephira = collision.gameObject.GetComponentInParent<SephiraUnit>();
            sephira.Ondamage(damage, owner);
        }

        if (isPlaySfx)
            AudioManager.Inst.PlaySfx(hitSound);
        //Invoke(nameof(DeActiveBullet), lifeTime);
    }

    private void DeActiveBullet()
    {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
    }
}
