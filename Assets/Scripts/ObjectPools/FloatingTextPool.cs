using UnityEngine;
using System.Collections;

namespace GOOST
{
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

        public FloatingText PoolText(string _score, Vector2 _position, Color _colour, float size = 1)
        {
            FloatingText _floatingText = objectPool.GetPooledObject();
            _floatingText.score.fontSize = (int)size * 24;
            _floatingText.OnPooled(_score, _position, _colour);
            return _floatingText;
        }
    }
}