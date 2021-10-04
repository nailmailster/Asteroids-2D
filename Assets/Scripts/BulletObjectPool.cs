using UnityEngine;
using UnityEngine.Pool;
using UnityEditor;

public class BulletObjectPool : MonoBehaviour
{
    public int maxPoolSize = 10;
    public int stackDefaultCapacity = 10;

    public Transform playerTransform;

    public IObjectPool<Bullet> Pool
    {
        get
        {
            if (b_pool == null)
                b_pool = new ObjectPool<Bullet>(
                                CreatedPooledItem,
                                OnTakeFromPool,
                                OnReturnedToPool,
                                OnDestroyPullObject,
                                true,
                                stackDefaultCapacity,
                                maxPoolSize);
                
            return b_pool;
        }
    }

    private IObjectPool<Bullet> b_pool;

    private Bullet CreatedPooledItem()
    {
        var b = AssetDatabase.LoadAssetAtPath<Bullet>("Assets/Prefabs/Bullet.prefab");
        var go = b.gameObject;
        Bullet bullet = Instantiate(b);

        go.name = "Pool Bullet";
        bullet.Pool = Pool;
        // bullet.transform.up = playerTransform.up;

        return bullet;
    }

    private void OnReturnedToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnTakeFromPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnDestroyPullObject(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    public void Spawn(Vector3 position, Quaternion rotation, Transform transform)
    {
        var bullet = Pool.Get();

        bullet.transform.position = position;
        bullet.transform.rotation = rotation;

        playerTransform = transform;
    }
}
