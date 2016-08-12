using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunkPool : MonoBehaviour
{
    private static JunkPool junkPool = null;
    public static JunkPool instance { get { return junkPool; } }

    private ObjectPool<Junk> objectPool = null;
    [SerializeField]
    private Junk junkPrefab = null;
    
    void Awake()
    {
        junkPool = this;

    }
    private void Start()
    {
        objectPool = new ObjectPool<Junk>(junkPrefab, 10, transform);
    }

    public Junk PoolJunk(Vector2 _pos)
    {
        Junk _junk = objectPool.GetPooledObject();
        _junk.transform.position = _pos;
        _junk.OnPooled();
        return _junk;
    }
}
