using UnityEngine;
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
    private int maxEggMash = 17;
    float eggtimer = 0;
    float maxEggTimer = 1.5f;
    float mashTime = 0;
    float maxMashTime = 0.15f;
    public GameObject eggTrans;
    public bool inNest = false;
    public bool carryingEgg = false;

    
    private bool isDead = false;

    private bool applyForce = false;
    [SerializeField] float upwardForce = 25.0f;

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
        base.Start();
	}
	
	// Update is called once per frame
	protected void Update () 
    {
        if (!isDead)
        {
            MashTimer();
            Movement();
            base.FixedUpdate();

            if(Input.GetButtonDown("Peck"+playerId.ToString())||Input.GetKeyDown(KeyCode.P))
            {
                Peck();
            }
            if (Input.GetButtonDown("BeakHeight"+playerId.ToString())||Input.GetKeyDown(KeyCode.L))
            {
                TogglePeckLocation();
            }

            if (Input.GetButtonDown("Fly" + playerId.ToString()))
            {
                applyForce = true;
            }
            LayAnEgg();
            if (Input.GetAxis("Vertical" + playerId.ToString()) < 0)
                platformManager.instance.NoCollisionsPlease(legs.legsCollider);
        }
	}

    #region movement
    void Movement()
    {
        if (applyForce)
        {
            if (body.velocity.y < 0)
            {
                body.velocity = new Vector2(body.velocity.x, 0.0f);
            }
            body.AddForce(new Vector2(0, upwardForce));
            applyForce = false;
        }

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
        if (Input.GetButtonDown("Interact"+playerId.ToString())&&inNest)
        {
            eggMash++;
            mashTime = 0;
        }
      
        if (eggMash >= maxEggMash)
        {
            eggMash = 0;
            eggtimer = 0;
            Egg e = EggPool.instance.PoolEgg();
            e.DisablePhysics(true);
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
        if (_eggLives > 0)
        {
            isDead = true;
            base.Defeat();
            Egg e = EggPool.instance.PoolEgg();
            e.OnPooled();
            e.transform.position = transform.position;
            PlayerManager.instance.RespawnPlayer(playerId);
            _eggLives--;
            PlayerManager.instance.StealEgg(playerId);
           
        }
    }

    public override void Respawn()
    {
        if (_eggLives > 0)
        {
            isDead = false;
            base.Respawn();
            Egg egg = PlayerManager.instance.GetNest(playerId).GetRespawnEgg();
            transform.position = egg.transform.position;
        }
    }
  

}
