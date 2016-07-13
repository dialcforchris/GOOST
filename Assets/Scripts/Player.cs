using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Egg egg;
    private int _playerId = 0;
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
	}
	
	// Update is called once per frame
	void Update () 
    {
        MashTimer();
        Movement();
        if (Input.GetButtonDown("Fire1"))
        {
            body.AddForce(new Vector2(0, 50));
        }
        LayAnEgg();
	}

    #region movement
    void Movement()
    {
        VelocityCheck();
        body.AddForce(new Vector2(Input.GetAxis("Horizontal") * (speed), 0));

        if (Input.GetAxis("Horizontal") < 0)
            transform.localScale = Vector3.one;
        if (Input.GetAxis("Horizontal") > 0)
            transform.localScale = new Vector3(-1, 1, 1);

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
                transform.localScale = new Vector3(-1, 1, 1);
            if (body.velocity.x < 0)
                transform.localScale = Vector3.one;

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
        if (Input.GetButtonDown("Fire2")&&EggTimer())
        {
            sr.color = Color.red;
            EggTimer();
            eggMash++;
            mashTime = 0;
        }
        else
        {
            sr.color = Color.white;
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
