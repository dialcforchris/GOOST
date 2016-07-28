using UnityEngine;
using System.Collections;

public class Player : Actor, ISegmentable<Actor>
{
    [SerializeField]
    Animator riderAnimator;
    [SerializeField]
    private float speed = 3.0f;

    [SerializeField] private float heightForce = 35.0f;

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

    private int _eggLives = 3;
    [SerializeField]
    Canvas PlayerCanvas;
    [SerializeField]
    AudioClip[] dashSounds,hurtSounds;
  
    public float dashcool = 0;
    public float maxDashCool = 5.0f;
   
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
    public string segmentName { get { return "Body"; } }
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
    protected void Start()
    {
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
                Movement();
                base.FixedUpdate();

                if (applyFly)
                {
                    body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                    body.AddForce(new Vector2(0, heightForce));
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

        if (Input.GetAxis("Horizontal" + playerId) < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            PlayerCanvas.transform.localScale = new Vector3(-0.01f, 0.01f, 0.01f);
        }
        if (Input.GetAxis("Horizontal" + playerId) > 0)
        {
            transform.localScale = Vector3.one;
            PlayerCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
  
    void VelocityCheck()
    {
        if (body.velocity.magnitude>4 )
        {
            body.velocity *= 0.9f;
        }
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
                    //Play a flapping sound, but make sure we don't play the same one twice in a row
                    int thisOne = Random.Range(0, flappingSounds.Count);
                    SoundManager.instance.playSound(flappingSounds[thisOne]);
                    AudioClip mostRecentSound = flappingSounds[thisOne];
                    if (lastFlapSound)
                        flappingSounds.Add(lastFlapSound);
                    flappingSounds.Remove(mostRecentSound);
                    lastFlapSound = mostRecentSound;

                    anim.Play("newGoose_flap");
                    riderAnimator.Play("cape_flap_b");
                }
            }
        }
    }

    public override void Defeat(PlayerType _type)
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (!invincible)
            {
                if (collectable > 0)
                {
                    for (int i = 0; i < collectable; i++)
                    {
                        Collectables c = CollectablePool.instance.PoolCollectables(playerType == PlayerType.BADGUY ? PickUpType.MONEY : PickUpType.HARDDRIVE, playerId);
                        c.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                    }
                    if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
                        _collectable = 0;

                    SoundManager.instance.playSound(hurtSounds[Random.Range(0, hurtSounds.Length)]);

                    Physics2D.IgnoreLayerCollision(8 + playerId, 10, true);
                    invinciblePermanence = true;
                    invincible = true;
                }
                else
                {
                    if (_type != PlayerType.ENEMY)
                    {
                        FloatingTextPool.instance.PoolText(deathScore, transform.position, Color.red);
                        PlayerManager.instance.GetPlayer(playerId == 0 ? 1 : 0).ChangeScore(deathScore);
                    }
                    applyFly = false;
                    Collectables c = CollectablePool.instance.PoolCollectables(playerType == PlayerType.BADGUY ? PickUpType.HARDDRIVE : PickUpType.MONEY, playerId);
                    c.transform.position = transform.position;
                    isDead = true;
                    base.Defeat(_type);
                    PlayerManager.instance.RespawnPlayer(playerId);
                }
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

    public void ResetGameStart()
    {
        if(invincible)
        {
            invicibleTimer = 0;
            invincible = false;
            if (Physics2D.GetIgnoreLayerCollision(8 + playerId, 10))
            {
                Physics2D.IgnoreLayerCollision(8 + playerId, 10, false);
            }
            invinciblePermanence = false;
            foreach (SpriteRenderer s in allsprites)
            {
                s.enabled = true;
            }
            spRend.enabled = true;
            flashBool = true;
            flashTime = 0.0f;
        }
        dashcool = maxDashCool;
        score = 0;
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

    bool dashSoundToggle;

    void Dash()
    {
        if (Input.GetButton("Interact"+playerId)&&DashCoolTime())
        {
            SoundManager.instance.playSound(dashSounds[dashSoundToggle ? 0 : 1]);
            dashSoundToggle = !dashSoundToggle;
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
    protected override void OnCollisionStay2D(Collision2D _col)
    {
        base.OnCollisionStay2D(_col);

        //if (_col.collider.tag == "Platform")
        //{
        //    if (_col.contacts[0].normal.x != 0.0f)
        //    {
        //        ApplyKnockback(_col.contacts[0].normal, 0.45f);
        //    }
        //}
    }

}
