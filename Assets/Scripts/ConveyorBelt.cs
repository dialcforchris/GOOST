using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    //false for left, true for right
    [SerializeField] private bool direction = false;
	
    void OnCollisionStay2D(Collision2D col)
    {
        col.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2( (direction? 5 : -5)*Time.deltaTime,0);
    }
}
