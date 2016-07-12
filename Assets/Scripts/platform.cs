using UnityEngine;
using System.Collections;

public class platform : MonoBehaviour {

    public Collider2D thing;
    bool Player1, Player2;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (Player1)
            Physics2D.IgnoreCollision(col, thing, false);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Player1 = true;
        Physics2D.IgnoreCollision(col, thing,true);
    }


    void Start ()
    {
	
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            thing.enabled = false;
        }
    }
}
