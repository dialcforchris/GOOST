using UnityEngine;
using System.Collections;

public class Player : Actor, ISegmentable<Actor>
{
    [SerializeField]
    Animator riderAnimator;
    [SerializeField]
    private float speed;
    [SerializeField]
    SpriteRenderer spRend;
    [SerializeField]
    private PlayerType _playerType;
    public PlayerType playerType
    {
        get { return _playerType; }
    }
    private int _playerId = 3;
    private int score = 0;
    private SpriteRenderer[] allsprites;
   
    #region egg stuff [old shit]
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
    #endregion


    float dashcool = 0;
    float maxDashCool = 3f;
   
    float flashTime = 0;
    bool flashBool = false;
    float invicibleTimer = 0;
    float maxInvin = 2f;
    bool invincible = false;
    private int _collectable = 10;
    public int collectable
    {
        get { return _collectable; } 
        set {_collectable = value;}
    }
    private bool isDead = false;

    private bool applyFly = false;

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
    protected override void Start()
    {
        base.Start();
        allsprites = transform.GetComponentsInChildren<SpriteRenderer>();
        dashcool = maxDashCool;
    }
	
	// Update is called once per frame
	protected override void FixedUpdate () 
    {
        if (!isDead)
        {
            //Debug.Log(collectable);
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
            if(applyFly)
            {
                body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                body.AddForce(new Vector2(0, 50));
                StatTracker.instance.stats.totalFlaps++;
                applyFly = false;
            }
            DetermineAnimationState();
        }
	}

    protected void Update()
    {
        if (Input.GetButtonDown("Fly" + playerId.ToString()))
        {
            applyFly = true;
        }
        Dash();
        DashCoolTime();
        Invinciblity(invincible);
    }

    #region movement
    void Movement()
    {
        VelocityCheck();
        body.AddForce(new Vector2(Input.GetAxis("Horizontal"+playerId) * (speed),0));

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


    //kind of redundant
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

    public override void LandedOnPlatform(Collider2D col)
    {
        base.LandedOnPlatform(col);
        riderAnimator.Play("cap_flap_a");
    }

    public override void TakeOffFromPlatform()
    {
        base.TakeOffFromPlatform();
        riderAnimator.Play("cap_flap_b");
    }

    public override void DetermineAnimationState()
    {
        if (onPlatform)
        {
            if (Mathf.Abs(body.velocity.x) < 0.25f)
            {
                anim.Play("newGoose_idle");
                riderAnimator.Play("cape_idle");
            }
            else
            {
                anim.Play("newGoose_run");
                riderAnimator.Play("cape_flap_a");
            }
        }
        else
        {
            if (body.velocity.y > 0)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("newGoose_glide"))
                {
                    anim.Play("newGoose_flap");
                    riderAnimator.Play("cape_flap_b");
                }
            }
        }
    }

    public override void Defeat()
    {
        if (!invincible)
        {
            applyFly = false;
            if (collectable > 0)
            {
                for (int i = 0; i < collectable; i++)
                {
                    Collectables c = CollectablePool.instance.PoolCollectables(playerType == PlayerType.BADGUY ? PickUpType.MONEY : PickUpType.HARDDRIVE);
                    c.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                }
                _collectable = 0;
                invincible = true;
            }
            else
            {
               Collectables c = CollectablePool.instance.PoolCollectables(playerType == PlayerType.BADGUY ? PickUpType.MONEY : PickUpType.HARDDRIVE);
               c.OnPooled(playerType == PlayerType.BADGUY ? PickUpType.MONEY : PickUpType.HARDDRIVE);
               c.transform.position = transform.position;
               isDead = true;
               base.Defeat();
               PlayerManager.instance.RespawnPlayer(playerId);
            }
        }
    }

    public override void Respawn()
    {
        if (_eggLives>0)
        {
            base.Respawn();
            isDead = false;
            _eggLives--;
            transform.position = Vector2.zero;
            invincible = true;
        }
    }
  
    void Invinciblity(bool _on)
    {
        if (_on)
        {
            if (invicibleTimer<maxInvin)
            {
                invicibleTimer += Time.deltaTime;
            }
            else
            {
                invicibleTimer = 0;
                invincible = false;
            }
            FlashSprite();
        }
        else
        {
            foreach (SpriteRenderer s in allsprites)
            {
                s.enabled = true;
            }
            spRend.enabled = true;
        }
    }

    void FlashSprite()
    {
      
        if (flashTime<0.1f)
        {
            flashTime += Time.deltaTime;
            foreach (SpriteRenderer s in allsprites)
            {
                s.enabled = flashBool;
            }
            spRend.enabled = flashBool;
        }
        else
        {
            flashBool = !flashBool;
            flashTime = 0;
        }
    }

    void Dash()
    {
        if (Input.GetButton("Interact"+playerId)&&DashCoolTime())
        {
            if (transform.localScale.x>0)
            {
                body.AddForce(new Vector2(transform.right.x * (speed * 100.5f),0));
            }
            else
            {
                body.AddForce(new Vector2(-transform.right.x * (speed * 100.5f),0));
            }
            dashcool = 0;
        }
    }
    bool DashCoolTime()
    {
        if (dashcool<maxDashCool)
        {
            dashcool += Time.deltaTime;
            return false;
        }
        else
        {
            return true;
        }
    }
}
public enum PlayerType
{
    BADGUY,
    GOODGUY,
};