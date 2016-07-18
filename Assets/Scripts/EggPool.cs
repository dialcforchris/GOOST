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

    public Egg PoolEgg()
    {
        Egg _egg = objectPool.GetPooledObject();
        _egg.OnPooled();
        return _egg;
    }
}
