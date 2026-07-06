using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    public Bullet GetBullet(PoolManager.PrefabType bulletPrefab, GameObject targetOwner)
    {
        Bullet bullet = CreateBullet<Bullet>(bulletPrefab, targetOwner.transform);
        bullet.owner = targetOwner;
        return bullet;
    }

    public LaserBullet GetLaserBullet(PoolManager.PrefabType bulletPrefab, GameObject targetOwner)
    {
        LaserBullet bullet = CreateBullet<LaserBullet>(bulletPrefab, targetOwner.transform);
        bullet.owner = targetOwner;
        return bullet;
    }

    public BombBullet GetBombBullet(PoolManager.PrefabType bulletPrefab, GameObject targetOwner)
    {
        BombBullet bullet = CreateBullet<BombBullet>(bulletPrefab, targetOwner.transform);
        bullet.owner = targetOwner;
        return bullet;
    }

    private T CreateBullet<T>(PoolManager.PrefabType bulletPrefab, Transform spawnTransform)
    {
        return PoolManager.Inst.Get(bulletPrefab, spawnTransform.position).GetComponent<T>();
    }
}
