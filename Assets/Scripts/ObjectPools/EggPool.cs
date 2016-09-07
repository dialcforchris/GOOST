using UnityEngine;
using System.Collections.Generic;

namespace GOOST
{
    public class EggPool : MonoBehaviour
    {
        private static EggPool eggPool = null;
        public static EggPool instance { get { return eggPool; } }

        //I'm putting all the eggs in OneBasket ( ͡° ͜ʖ ͡°)
        public List<Egg> OneBasket = new List<Egg>();

        private ObjectPool<Egg> objectPool = null;
        [SerializeField]
        private Egg eggPrefab = null;

        private void Awake()
        {
            eggPool = this;
            objectPool = new ObjectPool<Egg>(eggPrefab, 10, transform);
        }

        public void Reset()
        {
            foreach (Egg e in OneBasket)
            {
                e.gameObject.SetActive(false);
                e.ReturnPool();
            }
        }

        public Egg PoolEgg(EnemyBehaviour _behaviour, float _speed)
        {
            Egg _egg = objectPool.GetPooledObject();
            _egg.OnPooled(_behaviour, _speed);
            OneBasket.Add(_egg);
            return _egg;
        }
    }
}