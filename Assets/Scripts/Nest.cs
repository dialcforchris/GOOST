using UnityEngine;
using System.Collections;

public class Nest : MonoBehaviour
{
    private int _owningPlayer = 0;
    public int owningPlayer
    {
        get { return _owningPlayer; }
        set { _owningPlayer = value; }
    }

    int eggs = 0;

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update () 
    {
	
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

}
