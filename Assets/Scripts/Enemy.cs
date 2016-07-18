﻿using UnityEngine;
using System.Collections;

public enum EnemyBehaviour
{
    RANDOM = 0, //Random movement, may attack when close
    AGGRESSIVE, //Progressively get more aggressive
    HUNTER, //Search and kill the player
    HIGH_FLYER, //Stay at the top of the screen
    CAPTIVE_EGG,
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

    [SerializeField] private SpriteRenderer eggSpriteRenderer = null;

    [SerializeField] private ScreenWrap screenWrap = null;

    public static int numActive = 0;

    private EnemyBehaviour behaviour = EnemyBehaviour.RANDOM;
    private EnemyBehaviour currentBehaviour;

    public Vector3 worldTarget;
    public Vector3 viewTarget;

    private float speed = 1.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float targetThreshold = 0.5f;

    private float aggression = 0.0f;
    [SerializeField] private float aggressionSpeed = 0.5f;

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
        //anim.Play("fly");
        behaviour = _behaviour;
        currentBehaviour = behaviour;
        FindTarget();
        gameObject.SetActive(true);
    }

    protected override void FixedUpdate()
    {
        aggression = Mathf.Min(1.0f, aggression + (aggressionSpeed * (Time.deltaTime * aggressionSpeed)));
        MovementToTarget();
        base.FixedUpdate();
    }

    public void FindTarget()
    {
        switch(currentBehaviour)
        {
            case EnemyBehaviour.RANDOM:
                viewTarget = new Vector3(Random.Range(0.01f, 0.99f), Random.Range(0.2f, 0.8f), 10);
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
                Nest _nest = PlayerManager.instance.GetLargestNest();
                if (_nest)
                {
                    viewTarget = Camera.main.WorldToViewportPoint(_nest.transform.position);
                }
                else
                {
                    viewTarget = new Vector3(Random.Range(0.01f, 0.99f), Random.Range(0.2f, 0.8f), 10);
                }
                break;
            case EnemyBehaviour.HIGH_FLYER:
                viewTarget = new Vector3(Random.Range(0.01f, 0.99f), Random.Range(0.85f, 0.95f), 10);
                break;
            case EnemyBehaviour.CAPTIVE_EGG:
                viewTarget = Camera.main.WorldToViewportPoint(EggJail.instance.dropoff.position);
                break;
        }
        UpdateWorldFromView();
    }

    private void UpdateWorldFromView()
    {
        worldTarget = Camera.main.ViewportToWorldPoint(viewTarget);
        Vector3 _wrapTarget = Camera.main.ViewportToWorldPoint(new Vector3(viewTarget.x + (Camera.main.WorldToViewportPoint(transform.position).x > 0.5f ? 1 : -1), viewTarget.y, viewTarget.z));
        if(Vector3.SqrMagnitude(worldTarget - transform.position) > Vector3.SqrMagnitude(_wrapTarget - transform.position))
        {
            worldTarget = _wrapTarget;
        }
    }

    private void MovementToTarget()
    {
        body.AddForce(((worldTarget - transform.position).normalized) * speed);
        body.velocity = new Vector2(Mathf.Min(maxSpeed, body.velocity.x), Mathf.Min(maxSpeed, body.velocity.y));

        if (body.velocity.x > 0) transform.localScale = Vector3.one;
        if (body.velocity.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (Vector3.SqrMagnitude(worldTarget - transform.position) < targetThreshold)
        {
            if (currentBehaviour == EnemyBehaviour.CAPTIVE_EGG)
            {
                currentBehaviour = behaviour;
                eggSpriteRenderer.gameObject.SetActive(false);
                EggJail.instance.EggCaptured();
            }
            FindTarget();
        }
    }

    public override void ApplyKnockback(Vector2 _direction, float _power)
    {
        base.ApplyKnockback(_direction, _power);
        FindTarget();
    }

    public override void Defeat()
    {
        base.Defeat();
        //anim.Stop();
        poolData.ReturnPool(this);
        --numActive;
    }

    private void OnTriggerEnter2D(Collider2D _col)
    {
        if (currentBehaviour != EnemyBehaviour.CAPTIVE_EGG)
        {
            if (_col.tag == "Nest")
            {
                _col.GetComponent<Nest>().EggStolen();
                eggSpriteRenderer.gameObject.SetActive(true);
                currentBehaviour = EnemyBehaviour.CAPTIVE_EGG;
                FindTarget();
            }
        }
    }
}
