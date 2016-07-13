using UnityEngine;
using System.Collections;

public class platform : MonoBehaviour {

    public Collider2D thing;
    void OnTriggerEnter2D(Collider2D col)
    {
<<<<<<< HEAD
        if (col.tag == "Player")
        {
            Player1 = true;
            playerGuy = col;
            Physics2D.IgnoreCollision(col, thing, false);
        }
        
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Player1 = false;
            Physics2D.IgnoreCollision(col, thing, true);
        }

=======
        Physics2D.IgnoreCollision(col, thing, false);
    }

    void OnTriggerExit2D(Collider2D col)
    {        
        Physics2D.IgnoreCollision(col, thing,true);
>>>>>>> origin/master
    }


    void Start()
    {

    }

    void Update()
    {
    }    
}
