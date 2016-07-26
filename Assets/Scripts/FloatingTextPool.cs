using UnityEngine;
using System.Collections;

public class FloatingTextPool : MonoBehaviour 
{
    private static FloatingTextPool textPool = null;
    public static FloatingTextPool instance { get { return textPool; } }

    private ObjectPool<FloatingText> objectPool = null;
    [SerializeField]
    private FloatingText floatingTextPrefab = null;

    void Awake()
    {
        textPool = this;

    }
    private void Start()
    {
        objectPool = new ObjectPool<FloatingText>(floatingTextPrefab, 10, transform);
    }

    public FloatingText PoolText(int _score, Vector2 _position, Color _colour)
    {
        Debug.Log("pooled Text");
        FloatingText _floatingText = objectPool.GetPooledObject();
        _floatingText.OnPooled(_score,_position,_colour);
        return _floatingText;
    }
}
