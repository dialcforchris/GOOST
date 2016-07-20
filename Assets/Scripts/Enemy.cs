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
    public string segmentName { get { return "Enemy"; } }
    #endregion

    [SerializeField] private ScreenWrap screenWrap = null;

    public static int numActive = 0;

    private EnemyBehaviour behaviour = EnemyBehaviour.RANDOM;
    private EnemyBehaviour currentBehaviour;

    public Vector3 worldTarget;
    public Vector3 viewTarget;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetThreshold = 0.5f;

    private float aggression = 0.0f;
    [SerializeField] private float aggressionSpeed = 0.5f;

    private float eggTime = 0.0f;
    [SerializeField] private float eggCooldown = 7.5f;
    [SerializeField][Range(0.0f,1.0f)] private float eggChance = 0.5f;

    [SerializeField] private float platformBounceX = 0.4f;
    [SerializeField] private float platformBounceY = 0.75f;

    protected override void Start()
    {
        screenWrap.AddScreenWrapCall(UpdateWorldFromView);
        base.Start();
    }

    public void Spawn(EnemyBehaviour _behaviour)
    {
        Respawn();
        aggression = 0.0f;
        ++numActive;
        behaviour = _behaviour;
        currentBehaviour = behaviour;
        FindTarget();
        anim.Play("newGoose_flap");
        gameObject.SetActive(true);
    }

    protected override void FixedUpdate()
    {
        aggression = Mathf.Min(1.0f, aggression + (aggressionSpeed * (Time.deltaTime * aggressionSpeed)));
        MovementToTarget();
        base.FixedUpdate();

        DetermineAnimationState();
    }

    public void FindTarget()
    {
        switch(currentBehaviour)
        {
            case EnemyBehaviour.RANDOM:
                float _y = Mathf.Max(0.2f, Mathf.Min(0.8f, Camera.main.WorldToViewportPoint(transform.position).y + Random.Range(-0.1f, 0.25f)));
                if (transform.localScale.x > 0)
                {
                    viewTarget = new Vector3(1.05f, _y, 10);
                }
                else
                {
                    viewTarget = new Vector3(-0.05f, _y, 10);
                }
                
                break;
            case EnemyBehaviour.AGGRESSIVE:
                if (aggression > Random.Range(0.0f, 1.0f))
                {
                    viewTarget = Camera.main.WorldToViewportPoint(PlayerManager.instance.GetClosestPlayer(transform.position).transform.position);
                }
                else
                {
                    viewTarget = new Vector3(Random.Range(0.01f, 0.99f), Random.Range(0.2f, 0.8f), 10);
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
                viewTarget = new Vector3(Random.Range(0.01f, 0.99f), Random.Range(0.85f, 0.95f), 10);
                break;
        }
        UpdateWorldFromView(false);
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
                break;
            case EnemyBehaviour.HUNTER:
                break;
            case EnemyBehaviour.HIGH_FLYER:
                break;
                //Vector3 _wrapTarget = Camera.main.ViewportToWorldPoint(new Vector3(viewTarget.x + (Camera.main.WorldToViewportPoint(transform.position).x > 0.5f ? 1 : -1), viewTarget.y, viewTarget.z));
                //if(Vector3.SqrMagnitude(worldTarget - transform.position) > Vector3.SqrMagnitude(_wrapTarget - transform.position))
                //{
                //    worldTarget = _wrapTarget;
                //}
        }
    }

    private void MovementToTarget()
    {
        body.AddForce(((worldTarget - transform.position).normalized) * speed);
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

    public override void Defeat()
    {
        base.Defeat();
        
        //SilverCoin _coin = CoinPool.instance.PoolCoin();
        //_coin.transform.position = transform.position;
        //////////////////////////////////////////////////////////////////////////////////POOL EGG

        --numActive;
        poolData.ReturnPool(this);
    }

    protected override void OnCollisionEnter2D(Collision2D _col)
    {
        base.OnCollisionEnter2D(_col);
        if (_col.contacts[0].normal == Vector2.down)
        {
            body.AddForce(Vector2.down * platformBounceY, ForceMode2D.Impulse);
        }
        else if(_col.contacts[0].normal != Vector2.up)
        {
            PlatformSideCollision(_col);
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
    }

    public override void TakeOffFromPlatform()
    {
        base.TakeOffFromPlatform();
    }
}
