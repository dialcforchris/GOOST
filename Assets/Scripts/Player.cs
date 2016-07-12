using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;
    float jump;
    [SerializeField]
    private Rigidbody2D body;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        jump = 0;
        Movement();
        if (Input.GetButtonDown("Fire1"))
        {
            body.AddForce(new Vector2(0, 50));
        }
	}

    void Movement()
    {
       // transform.position = new Vector2(transform.position.x + Input.GetAxis("Horizontal") * (Time.deltaTime*speed), transform.position.y);
        body.AddForce(new Vector2(Input.GetAxis("Horizontal") * (speed),0));
    }

    bool JustPressed(string _button)
    {
        bool pressed = false;
       if (Input.GetButtonDown(_button)&&!pressed)
       {
           pressed= true;
       }
        if (Input.GetButtonUp(_button)&&pressed)
        {
            pressed = false;
        }
        return pressed;
    }
    
}
