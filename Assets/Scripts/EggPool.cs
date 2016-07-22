using UnityEngine;
using System.Collections;

public class EggPool : MonoBehaviour
{
    private static EggPool eggPool = null;
    public static EggPool instance { get { return eggPool; } }

    private ObjectPool<Egg> objectPool = null;
    [SerializeField] private Egg eggPrefab = null;

    private void Awake()
    {
        eggPool = this;
        objectPool = new ObjectPool<Egg>(eggPrefab, 10, transform);
    }

    public Egg PoolEgg(EnemyBehaviour _behaviour, float _speed)
    {
        Egg _egg = objectPool.GetPooledObject();
        _egg.OnPooled(_behaviour, _speed);
        return _egg;
    }
}
