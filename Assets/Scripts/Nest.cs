using UnityEngine;
using System.Collections;

public class Nest : MonoBehaviour
{

    [SerializeField]
    private GameObject[] anEggs;
    private int _owningPlayer = 0;
    public int owningPlayer
    {
        get { return _owningPlayer; }
        set { _owningPlayer = value; }
    }

    int eggs = 3;

	void Update()
    {
        PlayerManager.instance.GetPlayer(_owningPlayer).eggLives = eggs;
        UpdateEggs();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
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
            eggs--;
            col.gameObject.GetComponent<Egg>().inNest = false;
        }
    }
    void UpdateEggs()
    {
        for (int i=0;i<anEggs.Length;i++)
        {
            if (i <= eggs)
            {
                anEggs[i].SetActive(true);
            }
            else
            {
                anEggs[i].SetActive(false);
            }
        }
    }

    public GameObject GetResawnEgg()
    {
        return anEggs[Random.Range(0, anEggs.Length)];
    }

}
