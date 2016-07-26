using UnityEngine;
using System.Collections;

public enum EnemyBehaviour
{
    RANDOM = 0, //Random movement, may attack when close
    AGGRESSIVE, //Progressively get more aggressive
    HUNTER, //Search and kill the player
    HIGH_FLYER, //Stay at the top of the screen
    COUNT
}

public class Enemy : Actor, IPoolable<Enemy>, ISegmentable<Actor>
{
    #region IPoolable
    public PoolData<Enemy> poolData { get; set; }
    #endregion

    #region ISegmentable
    public Actor rigBase { get { return this; } }
    public string segmentName { get { return "Body"; } }
    #endregion

    [SerializeField] private ScreenWrap screenWrap = null;

    public static int numActive = 0;
    public static int roughMaxActive = 10;

    private EnemyBehaviour behaviour = EnemyBehaviour.RANDOM;
    private EnemyBehaviour currentBehaviour;

    public Vector3 worldTarget;
    public Vector3 viewTarget;
    [SerializeField]
    private Transform eggTrans;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetThreshold = 0.5f;

    private float aggression = 0.0f;
    [SerializeField] private float aggressionSpeed = 0.5f;

    private float eggTime = 0.0f;
    [SerializeField] private float eggCooldown = 2.5f;
    [SerializeField][Range(0.0f,1.0f)] private float eggChance = 0.5f;

    [SerializeField] private float platformBounceX = 0.4f;
    [SerializeField] private float platformBounceY = 0.75f;

    [SerializeField] private float takeOffCooldown = 3.0f;
    private float takeOffTime = 0.0f;

    private Player closestPlayer = null;

    [SerializeField] private LayerMask RayLayerMask;
    [SerializeField]
    private int score;

    protected void Start()
    {
        screenWrap.AddScreenWrapCall(UpdateWorldFromView);
    }

    public void Spawn(EnemyBehaviour _behaviour, float _speed)
    {
        Respawn();
        aggression = 0.0f;
        ++numActive;
        behaviour = _behaviour;
        currentBehaviour = behaviour;
        if(currentBehaviour == EnemyBehaviour.HUNTER)
        {
            currentBehaviour = EnemyBehaviour.AGGRESSIVE;
            aggression = 1.0f;
        }
        speed = _speed;
        FindTarget();
        gameObject.SetActive(true);
        takeOffTime = 0.0f;
        eggTime = 0.0f;
        onPlatform = false;
        body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        anim.Play("newGoose_flap");
    }
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            LayAnEgg();
        }
    }
    protected override void FixedUpdate()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (currentBehaviour == EnemyBehaviour.AGGRESSIVE)
            {
                aggression = Mathf.Min(1.0f, aggression + (aggressionSpeed * (Time.deltaTime * aggressionSpeed)));
            }

            if (onPlatform)
            {
                takeOffTime += Time.deltaTime;
                if (takeOffTime >= takeOffCooldown)
                {
                    body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                    body.AddForce(new Vector2(0, 50));
                    takeOffTime = 0.0f;
                }
            }

            MovementToTarget();

            base.FixedUpdate();

            DetermineAnimationState();
        }
    }

    public void FindTarget()
    {
        switch(currentBehaviour)
        {
            case EnemyBehaviour.RANDOM:
                RandomTarget();
                break;
            case EnemyBehaviour.AGGRESSIVE:
                if (aggression > Random.Range(0.0f, 1.0f))
                {
                    closestPlayer = PlayerManager.instance.GetClosestPlayer(transform.position);
                    RaycastHit2D _hit = Physics2D.Raycast(transform.position, closestPlayer.transform.position - transform.position, Mathf.Infinity, RayLayerMask.value);
                    if (_hit)
                    {
                        if (_hit.transform.tag == "Player")
                        {
                            viewTarget = Camera.main.WorldToViewportPoint(_hit.point);
                        }
                        else
                        {
                            if (closestPlayer.transform.position.y > transform.position.y)
                            {
                                viewTarget = new Vector3(transform.localScale.x > 0 ? 1.05f : -0.05f, Camera.main.WorldToViewportPoint(closestPlayer.transform.position).y + 0.2f, 10);
                            }
                            else
                            {
                                viewTarget = new Vector3(transform.localScale.x > 0 ? 1.05f : -0.05f, Camera.main.WorldToViewportPoint(closestPlayer.transform.position).y, 10);
                            }
                        }
                    }
                    else
                    {
                        RandomTarget();
                    }
                }
                else
                {
                    RandomTarget();
                }
                break;
            case EnemyBehaviour.HUNTER:
              ////  Nest _nest = PlayerManager.instance.GetLargestNest();
              ////  if (_nest)
              //  {
              ////      viewTarget = Camera.main.WorldToViewportPoint(_nest.transform.position);
              //  }
              //  else
              //  {
              //      viewTarget = new Vector3(Random.Range(0.01f, 0.99f), Random.Range(0.2f, 0.8f), 10);
              //  }
                break;
            case EnemyBehaviour.HIGH_FLYER:
                if (transform.localScale.x > 0)
                {
                    viewTarget = new Vector3(1.05f, Random.Range(0.80f, 0.95f), 10);
                }
                else
                {
                    viewTarget = new Vector3(-0.05f, Random.Range(0.80f, 0.95f), 10);
                }
                break;
        }
        UpdateWorldFromView(false);
    }

    private void RandomTarget()
    {
        float _y = Random.value;
        if (_y < 0.1f)
        {
            _y = Random.Range(0.2f, 0.4f);
        }
        else if (_y < 0.6f)
        {
            _y = Random.Range(0.7f, 0.9f);
        }
        else
        {
            _y = Mathf.Max(0.2f, Mathf.Min(0.9f, Camera.main.WorldToViewportPoint(transform.position).y + Random.Range(-0.1f, 0.25f)));
        }

        if (transform.localScale.x > 0)
        {
            viewTarget = new Vector3(1.05f, _y, 10);
        }
        else
        {
            viewTarget = new Vector3(-0.05f, _y, 10);
        }
    }

    private void UpdateWorldFromView(bool _wrap)
    {
        worldTarget = Camera.main.ViewportToWorldPoint(viewTarget);
        switch (currentBehaviour)
        {
            case EnemyBehaviour.RANDOM:
                if(_wrap)
                {
                    FindTarget();
                }
                break;
            case EnemyBehaviour.AGGRESSIVE:
                if (_wrap)
                {
                    FindTarget();
                }
                break;
            case EnemyBehaviour.HUNTER:
                break;
            case EnemyBehaviour.HIGH_FLYER:
                if (_wrap)
                {
                    FindTarget();
                }
                break;
                
        }
    }

    private void MovementToTarget()
    {
        body.AddForce(((worldTarget - transform.position).normalized) * (speed + aggression));
        VelocityCap();

        if ((worldTarget - transform.position).x > 0) transform.localScale = Vector3.one;
        else if ((worldTarget - transform.position).x < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (Vector3.SqrMagnitude(worldTarget - transform.position) < targetThreshold)
        {
            FindTarget();
        }
    }

    private void VelocityCap()
    {
        Vector2 _vel = body.velocity;
        _vel.x = Mathf.Max(-maxSpeed, Mathf.Min(maxSpeed, body.velocity.x));
        _vel.y = onPlatform ? 0.0f : Mathf.Max(-maxSpeed, Mathf.Min(maxSpeed, body.velocity.y));
        body.velocity = _vel;
    }

    public override void ApplyKnockback(Vector2 _direction, float _power)
    {
        base.ApplyKnockback(_direction, _power);
        //FindTarget();
    }

    public override void Defeat(PlayerType _type)
    {
        base.Defeat(_type);

        Collectables c = CollectablePool.instance.PoolCollectables(_type == PlayerType.BADGUY? PickUpType.MONEY:PickUpType.HARDDRIVE);
        c.transform.position = transform.position;
        FloatingTextPool.instance.PoolText(score,transform.position,Color.green);
        PlayerManager.instance.GetPlayer(_type == PlayerType.BADGUY ? 0 : 1).ChangeScore(score);
        --numActive;
        anim.Stop();
        poolData.ReturnPool(this);
    }

    protected override void OnCollisionEnter2D(Collision2D _col)
    {
        base.OnCollisionEnter2D(_col);
        if (_col.collider.tag == "Player" || _col.collider.tag == "Enemy")
        {
            if (_col.contacts[0].normal.x != 0.0f)
            {
                if(transform.localScale.x > 0)
                {
                    if(_col.contacts[0].point.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, 1.0f, 1.0f);
                        FindTarget();
                    }
                }
                else
                {
                    if (_col.contacts[0].point.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, 1.0f, 1.0f);
                        FindTarget();
                    }
                }
            }
        }
        if (currentBehaviour == EnemyBehaviour.AGGRESSIVE)
        {
            FindTarget();
        }
    }

    protected override void OnCollisionStay2D(Collision2D _col)
    {
        base.OnCollisionStay2D(_col);
        
        if (_col.collider.tag == "Platform")
        {
            if (_col.contacts[0].normal.y < 0)
            {
                body.velocity = new Vector2(body.velocity.x, 0.0f);
                body.AddForce(Vector2.down * platformBounceY, ForceMode2D.Impulse);
            }
            else if (_col.contacts[0].normal.x != 0.0f)
            {
                PlatformSideCollision(_col);
            }
        }

        


        //if (_col.collider.tag == "Enemy")
        //{
        //    ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        //    if (rigSegment != null)
        //    {
        //        ((Enemy)rigSegment.rigBase).FindTarget();
        //    }
        //}
    }

    public void PlatformSideCollision(Collision2D _col)
    {
        body.velocity = Vector2.zero;
        Vector2 _force = _col.contacts[0].normal * platformBounceX;
        Vector2 _colWorldPos = transform.TransformPoint(col.bounds.center) / 2.0f;
        _force.y = (1 - Mathf.Abs(_col.contacts[0].point.y - _colWorldPos.y)) * (_col.contacts[0].point.y > _colWorldPos.y ? -1.0f : 1.0f);
        body.AddForce(_force, ForceMode2D.Impulse);
    }

    public override void LandedOnPlatform(Collider2D col)
    {
        base.LandedOnPlatform(col);
        VelocityCap();
        takeOffTime = 0.0f;
    }

    public override void TakeOffFromPlatform()
    {
        base.TakeOffFromPlatform();
    }

    void LayAnEgg()
    {
        if (EggTimer())
        {
            if (numActive < roughMaxActive)
            {
                if (Random.value < eggChance)
                {
                    Egg e = EggPool.instance.PoolEgg(behaviour, speed);
                    e.transform.position = eggTrans.position;// new Vector2(transform.position.x + 0.5f, transform.position.y);
                    //e.OnPooled();
                }
            }
        }
    }

    bool EggTimer()
    {
        if (eggTime < eggCooldown)
        {
            eggTime += Time.deltaTime;
            return false;
        }
        else
        {
            eggTime = 0;
            return true;
        }
    }
}
