using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        for (int i = 0; i < maxEggs;i++ )
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

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            ISegmentable<Actor> rigSegment = col.GetComponent<ISegmentable<Actor>>();
            if (rigSegment != null)
            {
                Player p = (Player)rigSegment.rigBase;
                if (_owningPlayer == p.playerId)
                {
                    p.inNest = true;
                    if (p.carryingEgg)
                    {
                        p.carryingEgg = false;
                        AddEgg();
                    }
                }
                else
                {
                    if (!p.carryingEgg)
                    {
                        EggStolen();
                        p.carryingEgg = true;
                    }
                }
            }
        }
        if (col.gameObject.tag == "Egg")
        {
            Egg e = col.gameObject.GetComponent<Egg>();
            if (numEggs < maxEggs)
            {
                e.transform.position = eggTrans[numEggs-1].position;
                e.inNest = true;
                e.owningPlayer = owningPlayer;
                anEggs.Add(e);
                e.DisablePhysics(true);
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            ISegmentable<Actor> rigSegment = col.GetComponent<ISegmentable<Actor>>();
            if (rigSegment != null)
            {
                Player p = (Player)rigSegment.rigBase;
                if (_owningPlayer == p.playerId)
                {
                    p.inNest = false;
                }
            }
            if (col.gameObject.tag == "Egg")
            {
                if (numEggs>0)
                {
                    col.gameObject.GetComponent<Egg>().inNest = true;
                    col.gameObject.GetComponent<Egg>().DisablePhysics(true);
                }
            }
        }
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
        if(anEggs.Count == 0)
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
            anEggs[numEggs-1].ReturnPool();
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
