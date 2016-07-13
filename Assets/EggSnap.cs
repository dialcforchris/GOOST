using UnityEngine;
using System.Collections;

public class EggSnap : MonoBehaviour 
{

	void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Egg")
        {
           col.transform.SetParent(this.transform);
           Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
           rb.gravityScale = 0;
           rb.mass = 0;
        }
    }
}
