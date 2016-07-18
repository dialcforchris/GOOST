using UnityEngine;
using System.Collections;

public class Nest : MonoBehaviour
{
    [SerializeField]
    private Transform[] eggTrans;
    private Egg[] anEggs;
    private int activeEggs = 0;
    private int _owningPlayer = 0;
    public int owningPlayer
    {
        get { return _owningPlayer; }
        set { _owningPlayer = value; }
    }

    void Start()
    {
        anEggs = new Egg[eggTrans.Length];
        for (int i = 0; i < eggTrans.Length;i++ )
        {
            Egg e = EggPool.instance.PoolEgg();
            e.transform.position = eggTrans[i].position;
            anEggs[i] = e;
            activeEggs++;
        }
    }


	void Update()
    {
        PlayerManager.instance.GetPlayer(_owningPlayer).eggLives = activeEggs;
        UpdateEggs();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player p = col.gameObject.GetComponent<Player>();
            if (owningPlayer == p.playerId)
            {
                p.inNest = true;
            }
            //let player lay egg
        }
        if (col.gameObject.tag == "Egg")
        {
            Egg e = col.gameObject.GetComponent<Egg>();
            e.inNest = true;
            e.owningPlayer = owningPlayer;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Egg")
        {
            activeEggs--;
            col.gameObject.GetComponent<Egg>().inNest = false;
        }
    }
    void UpdateEggs()
    {
        for (int i=0;i<anEggs.Length;i++)
        {
            if (i <= activeEggs)
            {
                anEggs[i].gameObject.SetActive(true);
            }
            else
            {
                anEggs[i].gameObject.SetActive(false);
            }
        }
    }

    public Egg GetResawnEgg()
    {
       
        return anEggs[Random.Range(0, activeEggs)];
    }

}
