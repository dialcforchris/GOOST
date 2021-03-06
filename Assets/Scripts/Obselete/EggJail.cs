﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GOOST
{
    public class EggJail : MonoBehaviour
    {
        private static EggJail eggJail = null;
        public static EggJail instance { get { return eggJail; } }

        public int cost = 100;
        public int inflation = 50;
        int numberOfEggs = 3;
        Egg egg;
        [SerializeField]
        private Text price;
        [SerializeField]
        private Text remaining;

        [SerializeField]
        private Transform eggDropoff = null;
        public Transform dropoff { get { return eggDropoff; } }

        private void Awake()
        {
            eggJail = this;
            //egg = EggPool.instance.PoolEgg();
            egg.DisablePhysics(true);
            egg.transform.position = transform.position;
        }

        void Update()
        {
            //ShowEggs();
        }
        //void ShowEggs()
        // {
        //     bool eggsThen = false;
        //     eggsThen = numberOfEggs>0?  true : false;
        //     if (eggsThen)
        //     {

        //     }
        //     price.supportRichText = true;

        //     price.text = "EGG RETURN SERVICE" + "\n"+ "<color=#c0c0c0ff>" + cost + " SILVER</color>";
        //     if (numberOfEggs > 0)
        //     {
        //         remaining.text = "ONLY " + numberOfEggs + " REMAINING!";
        //     }
        //     else
        //     {

        //         remaining.text = "SOLD OUT!";
        //     }
        // }

        //void OnTriggerEnter2D(Collider2D _col)
        //{
        //    if (_col.gameObject.tag == "Player")
        //    {
        //        ISegmentable<Actor> rigSegment = _col.gameObject.GetComponent<ISegmentable<Actor>>();
        //        if (rigSegment != null)
        //        {
        //            Player p = (Player)rigSegment.rigBase;
        //            {

        //                if (numberOfEggs > 0)
        //                {
        //                    if (p.GetScore() > cost)
        //                    {
        //                        NumberOfEggs(-1);
        //                        p.ChangeScore(-cost);
        //                        p.carryingEgg = true;
        //                        cost += inflation;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        public void NumberOfEggs(int _difference)
        {
            numberOfEggs += _difference;
        }

        public void EggCaptured()
        {
            NumberOfEggs(1);
            inflation += numberOfEggs * 15;
        }
    }
}