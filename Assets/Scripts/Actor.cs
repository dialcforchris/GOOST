using UnityEngine;
using System.Collections;

public interface ISegmentable<T> where T : Actor
{ 
    T rigBase { get; }
    string segmentName { get; }
}

public class Actor : MonoBehaviour
{
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected Collider2D col = null;
    public Collider2D actorCollider { get { return col; } }
    [SerializeField] protected Rigidbody2D body = null;

    public  ParticleSystem skidMark,landingParticle;
    private bool peckUp = true;

    [SerializeField] protected Lance lance = null;
    [SerializeField] protected Legs legs = null;

    [SerializeField] protected ActorSegment[] segments = null;

    protected Vector3 worldMaxY;
    [SerializeField] protected float viewportMaxY = 1.01f;

    protected bool onPlatform = false;

    private bool extending = false;

    protected virtual void Start()
    {
        worldMaxY = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, viewportMaxY));
    }

    protected virtual void OnEnable()
    {
        //Ignore collisions with child objects
        for (int i = 0; i < segments.Length; ++i)
        {
            Physics2D.IgnoreCollision(col, segments[i].segmentCollider);
            for(int j = i + 1; j < segments.Length; ++j)
            {
                Physics2D.IgnoreCollision(segments[i].segmentCollider, segments[j].segmentCollider);
            }
            Physics2D.IgnoreCollision(segments[i].segmentCollider, lance.lanceCollider);
            Physics2D.IgnoreCollision(segments[i].segmentCollider, legs.legsCollider);
        }
        Physics2D.IgnoreCollision(col, lance.lanceCollider);
        Physics2D.IgnoreCollision(col, legs.legsCollider);
    }

    protected bool Extend()
    {
        if(extending)
        {
            return false;
        }
        else
        {
            extending = true;
            return true;
        }
    }

    public virtual void ApplyKnockback(Vector2 _direction, float _power)
    {
        body.velocity = new Vector2(0.0f, body.velocity.y);
        body.AddForce(_direction.normalized * _power);
    }

    public virtual void Defeat()
    {
        col.enabled = false;
        lance.ActorDefeated();
        legs.ActorDefeated();
        foreach(ActorSegment _seg in segments)
        {
            _seg.ActorDefeated();
        }
        StartCoroutine(DeathAnimation());
    }

    protected virtual IEnumerator DeathAnimation()
    {
        //frameHolder.instance.holdFrame(0.25f);
        FeatherManager.instance.HaveSomeFeathers(transform.position);
        yield return new WaitForSeconds(0.01f);
        anim.Stop();
        gameObject.SetActive(false);
    }

    public virtual void Respawn()
    {
        col.enabled = true;
        lance.ActorSpawned();
        legs.ActorSpawned();
        foreach (ActorSegment _seg in segments)
        {
            _seg.ActorSpawned();
        }
        gameObject.SetActive(true);
    }

    protected virtual void FixedUpdate()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (transform.position.y > worldMaxY.y)
            {
                body.velocity = new Vector2(body.velocity.x, -0.5f);
            }
        }
    }

    protected void Peck()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("goose_neck_up_idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("goose_neck_down_idle"))
        {
            if(peckUp)
            {
                anim.Play("goose_neck_up_extend");
            }
            else
            {
                anim.Play("goose_neck_down_extend");
            }
        }
    }

    protected void TogglePeckLocation()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("goose_neck_up_idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("goose_neck_down_idle"))
        {
            anim.SetBool("mirror", peckUp ? false : true);
            peckUp = !peckUp;
            anim.Play("goose_neck_up_to_down");
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D _col)
    {
    }

    protected virtual void OnCollisionStay2D(Collision2D _col)
    {
        
        if (_col.collider.tag == "Platform")
        {
            if (_col.contacts[0].otherCollider == col)
            {
                if (_col.contacts[0].normal == Vector2.up)
                {
                    body.AddForce(new Vector2(_col.transform.position.x > transform.position.x ? -0.05f : 0.05f, 0.0f), ForceMode2D.Impulse);
                }
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

    public virtual void LandedOnPlatform(Collider2D col)
    {
        //Make sure the player hasn't sunk into the floor for some silly reason when we freeze the Y position of the player
        float landPosition = col.bounds.max.y;
        transform.position = new Vector3(transform.position.x, landPosition+0.37f, transform.position.z);
        
        landingParticle.Play();
        onPlatform = true;
        body.constraints = RigidbodyConstraints2D.FreezePositionY | ~RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        if (Mathf.Abs(body.velocity.x) > 0)
            anim.Play("newGoose_run");
        else
            anim.Play("newGoose_idle");

        if (body.velocity.x > 5 || body.velocity.x < -5)
        {
            skidMark.Emit(1);
        }
    }

    public virtual void TakeOffFromPlatform()
    {
        onPlatform = false;
        body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        anim.Play("newGoose_flap");
    }

    public virtual void DetermineAnimationState()
    {
        if (onPlatform)
        {
            if (Mathf.Abs(body.velocity.x) < 0.5f)
            {
                anim.Play("newGoose_idle");
            }
            else
            {
                anim.Play("newGoose_run");
            }
        }
        else
        {
            if (body.velocity.y > 0)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("newGoose_glide"))
                {
                    anim.Play("newGoose_flap");
                }
            }
        }
    }

}
