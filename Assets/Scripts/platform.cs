using UnityEngine;
using System.Collections;

public class platform : MonoBehaviour {

    public Collider2D thing;
    bool Player1, Player2;
    Collider2D playerGuy;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Player1 = true;
            playerGuy = col;
        }
        Physics2D.IgnoreCollision(col, thing, false);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
            Player1 = false;
        
        Physics2D.IgnoreCollision(col, thing,true);
    }


    void Start()
    {

    }

    void Update()
    {
        if (Player1)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Physics2D.IgnoreCollision(playerGuy, thing, true);
            }
        }
    }    
}
