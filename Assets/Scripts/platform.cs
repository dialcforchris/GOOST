using UnityEngine;
using System.Collections;

public class platform : MonoBehaviour {

    public Collider2D thing;

    void OnTriggerEnter2D(Collider2D col)
    {
        Physics2D.IgnoreCollision(col, thing,false);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Physics2D.IgnoreCollision(col, thing,true);
    }


    void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}
}
