﻿using UnityEngine;
using System.Collections;

public class Player : Actor, ISegmentable<Actor>
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Egg egg;
    private int _playerId = 3;
    private int score = 0;
    private int _eggLives = 3;
    public int eggMash = 0;
    private int maxEggMash = 20;
    float eggtimer = 0;
    float maxEggTimer = 1.5f;
    float mashTime = 0;
    float maxMashTime = 0.15f;
    public GameObject eggTrans;

    #region ISegmentable
    public Actor rigBase { get { return this; } }
    public string segmentName { get { return "Player"; } }
    #endregion

    public int playerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }
    public int eggLives
    {
        get { return _eggLives; }
        set { _eggLives = value; }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void Start () 
    {
      //  platformManager.instance.NoCollisionsPlease(legs);
        Debug.Log(playerId);
        base.Start();
	}
	
	// Update is called once per frame
	protected override void FixedUpdate () 
    {
        MashTimer();
        Movement();
        base.FixedUpdate();
        if (Input.GetButtonDown("Fly"+playerId.ToString()))
        {
            body.AddForce(new Vector2(0, 50));
        }
        LayAnEgg();
        if (Input.GetAxis("Vertical"+playerId.ToString())<0)
            platformManager.instance.NoCollisionsPlease(legs.legsCollider);
	}

    #region movement
    void Movement()
    {
        VelocityCheck();
        body.AddForce(new Vector2(Input.GetAxis("Horizontal"+playerId) * (speed), 0));

        if (Input.GetAxis("Horizontal"+playerId) < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (Input.GetAxis("Horizontal"+playerId) > 0)
            transform.localScale = Vector3.one;

        if (body.velocity.x > 0)
                transform.localScale = Vector3.one;
        if (body.velocity.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
    }
  

    void VelocityCheck()
    {
        if (body.velocity.magnitude>10 )
        {
            body.velocity *= 0.9f;
        }
    }
    #endregion

    #region egg stuff
    void LayAnEgg()
    {
        if (Input.GetButtonDown("Interact"+playerId.ToString()))//&&EggTimer())
        {
            EggTimer();
            eggMash++;
            mashTime = 0;
        }
      
        if (eggMash >= maxEggMash)
        {
            eggMash = 0;
            eggtimer = 0;
            Egg e = (Egg)Instantiate(egg, this.transform.position, Quaternion.identity);
            e.getLaid = true;
        }
    }

    bool EggTimer()
    {
        if (eggtimer < maxEggTimer)
        {
            eggtimer += Time.deltaTime;
            return true;
        }
        else
        {
           // eggMash--;
            return false;
        }
    }
    bool MashTimer()
    {
        if (eggMash > 0)
        {
            if (mashTime < maxMashTime)
            {
                mashTime += Time.deltaTime;
                return true;
            }
            else
            {
                mashTime = 0;
                eggMash--;
                return false;
            }
        }
        else
        {
            eggtimer = 0;
        }
        return true;
    }
    #endregion

    #region score
    public void ChangeScore(int _change)
    {
        score += _change;
    }
    public int GetScore()
    {
        return score;
    }
    #endregion 

    public override void Defeat()
    {
        base.Defeat();
        transform.position = Vector3.zero;
        lance.ActorSpawned();
        legs.ActorSpawned();
    }
}
