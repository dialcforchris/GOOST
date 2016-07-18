using UnityEngine;
using System.Collections;

public class CoinPool : MonoBehaviour
{
    private static CoinPool coinPool = null;
    public static CoinPool instance { get { return coinPool; } }

    private ObjectPool<SilverCoin> objectPool = null;
    [SerializeField]
    private SilverCoin coinPrefab = null;

    private void Awake()
    {
        coinPool = this;
        objectPool = new ObjectPool<SilverCoin>(coinPrefab, 10, transform);
    }

    public SilverCoin PoolCoin()
    {
        SilverCoin _coin = objectPool.GetPooledObject();
        _coin.OnPooled();
        return _coin;
    }
}
