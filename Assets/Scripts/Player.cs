using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;
    float jump;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private Animator anim;

    void Start () 
    {
        platformManager.instance.NoCollisionsPlease(GetComponent<Collider2D>());
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
        VelocityCheck();
        // transform.position = new Vector2(transform.position.x + Input.GetAxis("Horizontal") * (Time.deltaTime*speed), transform.position.y);
        body.AddForce(new Vector2(Input.GetAxis("Horizontal") * (speed), 0));

        if (Input.GetAxis("Horizontal") < 0)
            sr.flipX = true;
        if (Input.GetAxis("Horizontal") > 0)
            sr.flipX = false;

        if (body.velocity.x == 0f && body.velocity.y == 0f)
            anim.Play("idle");
        else if (body.velocity.x > 0 && body.velocity.y == 0f)
        {
            anim.Play("walk");
        }
        else if (body.velocity.y == 0f)
        {
            sr.flipX = true;
        }
        else
        {
            if (body.velocity.x > 0)
                sr.flipX = false;
            if (body.velocity.x < 0)
                sr.flipX = true;

            anim.Play("fly");
        }
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
    void VelocityCheck()
    {
        if (body.velocity.magnitude>10 )
        {
            body.velocity *= 0.9f;
        }
    }
}
