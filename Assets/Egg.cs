﻿using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour 
{

    public bool getLaid = false;
    public bool inNest = false;
    [SerializeField]
    private GameObject brokenEgg;
    [SerializeField]
    private Collider2D col;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private GameObject magpie;
    [SerializeField]
    private Animator ani;
    float hatchTime = 0;
    float maxHatchTime = 6;
    private int _owningPlayer = 0;
    public int owningPlayer
    {
        get { return _owningPlayer; }
        set { _owningPlayer = value; }
    }
   
	// Use this for initialization
	void Start () 
    {
	    if (getLaid)
        {
            ani.Play("Laid");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (getLaid)
        {
            getLaid = false;
        }
        InNest();
        Hatch();
        if (transform.parent)
        {
            body.gravityScale = 0;
            body.mass = 0;
            transform.position = transform.parent.position;
        }
	}
    
    //void OnColliderEnter2D(Collision2D col)
    //{
    //    if (col.gameObject.tag == "Player")
    //    {
    //        transform.SetParent(col.gameObject.GetComponent<Player>().eggTrans.transform);
           
    //        transform.position = col.gameObject.GetComponent<Player>().eggTrans.transform.position;
    //    }
    //}
   
    void Hatch()
    {
        if (!inNest&&transform.parent ==null)
        {
            if (hatchTime<maxHatchTime)
            {
                hatchTime += Time.deltaTime;
            }
            else
            {
                Instantiate(brokenEgg,new Vector2(transform.position.x,transform.position.y +1),transform.rotation);
                Destroy(gameObject);
                //spawn a magpie
            }
        }
        else
        {
            hatchTime = 0;
        }
    }

    void InNest()
    {
        if (inNest)
        {
            col.isTrigger = true;
            body.gravityScale = 0;
            body.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            col.isTrigger = false;
            body.gravityScale = 1;
            body.constraints = RigidbodyConstraints2D.None;
            _owningPlayer = 3;
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<Player>().playerId != _owningPlayer)
            {
                transform.SetParent(col.transform);
            }
            else
            {
                if (transform.parent)
                {
                    transform.SetParent(null);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<Player>().playerId != _owningPlayer)
            {
                transform.SetParent(col.gameObject.GetComponent<Player>().eggTrans.transform);
            }
        }
    }
}
