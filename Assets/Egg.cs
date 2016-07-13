using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour 
{

    public bool getLaid = false;
    private bool inNest = false;
    float hatchTime = 0;
    float maxHatchTime = 5;
	// Use this for initialization
	void Awake () 
    {
	    if (getLaid)
        {
            //do a thing
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        Hatch();
	}
    
    void OnColliderEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            transform.SetParent(col.gameObject.GetComponent<Player>().eggTrans.transform);
           
            transform.position = col.gameObject.GetComponent<Player>().eggTrans.transform.position;
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Nest")
        {
            inNest = true;
        }
    }
    void Hatch()
    {
        if (!inNest)
        {
            if (hatchTime<maxHatchTime)
            {
                hatchTime += Time.deltaTime;
            }
            else
            {
                //spawn a magpie
               //lets have an eggshell particle system
              //Destroy this
            }
        }
    }
}
