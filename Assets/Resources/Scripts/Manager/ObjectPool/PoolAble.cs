using UnityEngine;
using UnityEngine.Pool;

public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public void ReleaseObject(EnemyController enemyController)
    {
        Pool.Release(gameObject);
    }
}
