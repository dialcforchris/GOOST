using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour
{
    //false for left, true for right
    [SerializeField] private bool direction = false;

    float maxSpeed=1.25f;

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(System.DateTime.Now.Millisecond);
    }
    void OnCollisionStay2D(Collision2D col)
    {
        if (Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().velocity.x) < maxSpeed)
        {
            col.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2((direction ? 5 : -5) * Time.deltaTime * 10, 0);
            //if (col.gameObject.GetComponent<Rigidbody2D>().velocity.x > maxSpeed)
            //    col.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(maxSpeed, col.gameObject.GetComponent<Rigidbody2D>().velocity.y);

            //if (col.gameObject.GetComponent<Rigidbody2D>().velocity.x < -maxSpeed)
            //    col.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-maxSpeed, col.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        }
        col.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(col.gameObject.GetComponent<Rigidbody2D>().velocity.x, 0);
    }
}
