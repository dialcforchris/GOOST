using UnityEngine;
using System.Collections;

public class Player : Actor
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Egg egg;
    private int _playerId = 3;
    private int score = 0;
    private int _eggLives = 3;
    private int eggMash = 0;
    private int maxEggMash = 20;
    float eggtimer = 0;
    float maxEggTimer = 1.5f;
    float mashTime = 0;
    float maxMashTime = 0.2f;
    public GameObject eggTrans;
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

    void Start () 
    {
        platformManager.instance.NoCollisionsPlease(GetComponent<Collider2D>());
        Debug.Log(playerId);
	}
	
	// Update is called once per frame
	void Update () 
    {
        MashTimer();
        Movement();
        if (Input.GetButtonDown("Fly"+playerId.ToString()))
        {
            body.AddForce(new Vector2(0, 50));
        }
        LayAnEgg();
        if (Input.GetKeyDown(KeyCode.DownArrow))
            platformManager.instance.NoCollisionsPlease(GetComponent<Collider2D>());
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

        if (body.velocity.x == 0f && body.velocity.y == 0f)
            anim.Play("idle");
        else if (body.velocity.x > 0 && body.velocity.y == 0f)
        {
            anim.Play("walk");
        }
        else if (body.velocity.y < 0.025f)
        {
            //sr.flipX = true;
        }
        else
        {
            if (body.velocity.x > 0)
                transform.localScale = Vector3.one;
            if (body.velocity.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            anim.Play("fly");
        }
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
        if (Input.GetButtonDown("Interact"+playerId.ToString())&&EggTimer())
        {
            sp.color = Color.red;
            EggTimer();
            eggMash++;
            mashTime = 0;
        }
        else
        {
            sp.color = Color.white;
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
            eggMash--;
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
}
