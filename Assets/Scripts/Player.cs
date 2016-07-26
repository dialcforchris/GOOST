using UnityEngine;
using System.Collections;

public class Player : Actor, ISegmentable<Actor>
{
    [SerializeField]
    Animator riderAnimator;
    [SerializeField]
    private float speed;
    [SerializeField]
    private SpriteRenderer cape;
    public SpriteRenderer backpack;
    
    [SerializeField]
    private SpriteRenderer weapon;
    [SerializeField]
    private Sprite[] weaponChoice;
   
    [SerializeField]
    SpriteRenderer spRend = null;
    [SerializeField]
    private int _playerId = 0;
    [SerializeField]
    private int deathScore = 0;
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


    public float dashcool = 0;
    float maxDashCool = 5.0f;
   
    float flashTime = 0;
    bool flashBool = false;
    float invicibleTimer = 0;
    float maxInvin = 2f;
    bool invincible = false;
    bool invinciblePermanence = false;
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
        SwitchGuys(_playerType);
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
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY || GameStateManager.instance.GetState() == GameStates.STATE_READYUP)
        {
            if (!isDead)
            {
                //Debug.Log(collectable);
                MashTimer();
                Movement();
                base.FixedUpdate();

                if (Input.GetButtonDown("Peck" + playerId.ToString()) || Input.GetKeyDown(KeyCode.P))
                {
                    Peck();
                }
                if (Input.GetButtonDown("BeakHeight" + playerId.ToString()) || Input.GetKeyDown(KeyCode.L))
                {
                    TogglePeckLocation();
                }
                if (applyFly)
                {
                    body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                    body.AddForce(new Vector2(0, 50));
                    StatTracker.instance.stats.totalFlaps++;
                    applyFly = false;
                }
            }
        }
                DetermineAnimationState();
	}

    protected void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY || GameStateManager.instance.GetState() == GameStates.STATE_READYUP)
        {
            if (Input.GetButtonDown("Fly" + playerId.ToString()))
            {
                applyFly = true;
            }

            if (onPlatform)
            {
                if (Input.GetAxis("Horizontal" + playerId) < 0 && body.velocity.x > 5)
                    base.skidMark.Emit(1);
                else if (Input.GetAxis("Horizontal" + playerId) > 0 && body.velocity.x < -5)
                    base.skidMark.Emit(1);
                else if (Mathf.Abs(Input.GetAxis("Horizontal" + playerId)) < 0.25f && Mathf.Abs(body.velocity.x) > 5)
                    base.skidMark.Emit(1);

                if (Mathf.Abs(Input.GetAxis("Horizontal" + playerId)) < 0.25f)
                    body.drag = 3.5f;
                else
                    body.drag = 1;
            }
            else
                body.drag = 1;

            Dash();
            DashCoolTime();
            Invinciblity(invincible);
        }
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
            //Egg e = EggPool.instance.PoolEgg();
            //e.DisablePhysics(true);
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
        riderAnimator.Play("cape_flap_a");
    }

    public override void TakeOffFromPlatform()
    {
        base.TakeOffFromPlatform();
        riderAnimator.Play("cape_flap_b");
    }

    public override void DetermineAnimationState()
    {
        if (onPlatform)
        {
            if (Mathf.Abs(body.velocity.x) < 0.1f || Mathf.Abs(Input.GetAxis("Horizontal" + playerId)) < 0.1f)
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

    public override void Defeat(PlayerType _type)
    {
        if (!invincible)
        {
            if (collectable > 0)
            {
                for (int i = 0; i < collectable; i++)
                {
                    Collectables c = CollectablePool.instance.PoolCollectables(playerType == PlayerType.BADGUY ? PickUpType.MONEY : PickUpType.HARDDRIVE,playerId);
                    c.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                }
                _collectable = 0;

                Physics2D.IgnoreLayerCollision(8 + playerId, 10, true);
                invinciblePermanence = true;
                invincible = true;
            }
            else
            {
                if (_type != PlayerType.ENEMY)
                {
                    FloatingTextPool.instance.PoolText(score, transform.position, Color.red);
                    PlayerManager.instance.GetPlayer(playerId == 0 ? 1 : 0).ChangeScore(score);
                }
                applyFly = false;
                Collectables c = CollectablePool.instance.PoolCollectables(playerType == PlayerType.BADGUY ? PickUpType.MONEY : PickUpType.HARDDRIVE);
                c.transform.position = transform.position;
                isDead = true;
                base.Defeat(_type);
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
            transform.position = Vector2.up;
            TakeOffFromPlatform();
            invincible = true;
            Physics2D.IgnoreLayerCollision(8 + playerId, 10, true);
            invinciblePermanence = true;
        }
    }
  
    void Invinciblity(bool _on)
    {
        if (_on)
        {
            if (invicibleTimer<maxInvin)
            {
                invicibleTimer += Time.deltaTime;
                if(invinciblePermanence)
                {
                    if(invicibleTimer > maxInvin / 2.0f)
                    {
                        Physics2D.IgnoreLayerCollision(8 + playerId, 10, false);
                        invinciblePermanence = false;
                    }
                }
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
            if (playerType == PlayerType.BADGUY)
            {
                cape.enabled = flashBool;
            }
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
                body.AddForce(new Vector2(transform.right.x * (5), 0), ForceMode2D.Impulse);
            }
            else
            {
                body.AddForce(new Vector2(-transform.right.x * (5), 0), ForceMode2D.Impulse);
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

    void SwitchGuys(PlayerType _type)
    {
        switch (_playerType)
        {
            case PlayerType.GOODGUY:
                {
                    cape.gameObject.SetActive(false);
                    backpack.gameObject.SetActive(true);
                    weapon.sprite = weaponChoice[1];
                    break;
                }
            case PlayerType.BADGUY:
                {
                    cape.gameObject.SetActive(true);
                    backpack.gameObject.SetActive(false);
                    weapon.sprite = weaponChoice[0];
                    break;
                }
        }
    }
}
