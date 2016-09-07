using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOOST
{
    public class Nest : MonoBehaviour
    {
        [SerializeField]
        private Transform[] eggTrans;
        private List<Egg> anEggs = new List<Egg>();
        private int maxEggs = 3;
        public int numEggs { get { return anEggs.Count; } }
        private int _owningPlayer = 0;
        public int owningPlayer
        {
            get { return _owningPlayer; }
            set { _owningPlayer = value; }
        }

        void Start()
        {
            for (int i = 0; i < maxEggs; i++)
            {
                //Egg e = EggPool.instance.PoolEgg();
                //e.transform.position = eggTrans[i].position;
                //anEggs.Add(e);
                //e.DisablePhysics(true);
            }
        }


        void Update()
        {
            PlayerManager.instance.GetPlayer(_owningPlayer).eggLives = numEggs;
        }
        //void UpdateEggs()
        //{
        //    for (int i=0;i<anEggs.Length;i++)
        //    {
        //        if (i <= activeEggs)
        //        {
        //            anEggs[i].gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            anEggs[i].gameObject.SetActive(false);
        //        }
        //    }
        //}

        public Egg GetRespawnEgg()
        {
            if (anEggs.Count == 0)
            {
                return null;
            }
            Egg _egg = anEggs[numEggs - 1];
            anEggs.RemoveAt(anEggs.Count - 1);
            _egg.ReturnPool();
            return _egg;
        }

        public void EggStolen()
        {
            if (numEggs > 0)
            {
                anEggs[numEggs - 1].ReturnPool();
            }
        }
        public void AddEgg()
        {
            if (numEggs < maxEggs)
            {
                //anEggs.Add(EggPool.instance.PoolEgg());
            }
            else
            {
                //Egg e = EggPool.instance.PoolEgg();
                //e.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
            }
        }
    }
}
