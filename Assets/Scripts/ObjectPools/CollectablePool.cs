using UnityEngine;
using System.Collections;

public class CollectablePool : MonoBehaviour
{
    private static CollectablePool collectablePool = null;
    public static CollectablePool instance { get { return collectablePool; } }

    private ObjectPool<Collectables> objectPool = null;
    [SerializeField]
    private Collectables collectablesPrefab = null;

    private void Awake()
    {
        collectablePool = this;
        objectPool = new ObjectPool<Collectables>(collectablesPrefab, 10, transform);
    }

    public Collectables PoolCollectables(PickUpType _type)
    {
        Collectables _collectables = objectPool.GetPooledObject();
        _collectables.OnPooled(_type);
        return _collectables;
    }
}
