using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour
{
    //false for left, true for right
    [SerializeField] private bool direction = false;

    float maxSpeed=1.25f;

    void OnCollisionStay2D(Collision2D col)
    {
        if (Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().velocity.x) < maxSpeed)
        {
            col.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2((direction ? 5 : -5) * Time.deltaTime * 10, 0);
      
        }
        col.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(col.gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);
    }
}
