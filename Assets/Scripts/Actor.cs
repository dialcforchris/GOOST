using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISegmentable<T> where T : Actor
{ 
    T rigBase { get; }
    string segmentName { get; }
}

public enum CollisionEffect
{
    LAND = 0,
    CRUSHED,
    BACKSTABBER,
    BACKSTABBED,
    CLASH,
    CLASH_WIN,
    CLASH_LOSE,
    REVERSE_BUMP,
    COUNT
}

public class Actor : MonoBehaviour
{
    public SpriteRenderer headSprite;
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected Collider2D col = null;
    public Collider2D actorCollider { get { return col; } }
    [SerializeField]
    protected Rigidbody2D body = null;
    public Rigidbody2D GooseyBod { get { return body; } }

    public  ParticleSystem skidMark,landingParticle;
 
    [SerializeField]
    protected PlayerType _playerType = PlayerType.GOODGUY;
    public PlayerType playerType
    {
        get { return _playerType; }
    }

    [SerializeField] protected Lance lance = null;
    [SerializeField] protected Legs legs = null;

    public static Vector3 worldMaxY;

    protected bool onPlatform = false;
    [SerializeField]
    footstepPlayer footStepComponent;
    [SerializeField]
    public List<AudioClip> flappingSounds = new List<AudioClip>();
    [HideInInspector]
    public AudioClip lastFlapSound;
    public AudioClip deathSound;

    public platformManager.platformTypes currentSurface;

    protected virtual void OnEnable()
    {
        Physics2D.IgnoreCollision(col, lance.lanceCollider);
        Physics2D.IgnoreCollision(col, legs.legsCollider);
    }

    public virtual void ApplyKnockback(Vector2 _direction, float _power)
    {
        Vector2 _velClear = body.velocity;
        if(body.velocity.x * _direction.x < 0)
        {
            _velClear.x = 0.0f;
        }
        if (body.velocity.y * _direction.y < 0)
        {
            _velClear.y = 0.0f;
        }
        body.velocity = _velClear;// new Vector2(0.0f, body.velocity.y);
        body.AddForce(_direction.normalized * _power, ForceMode2D.Impulse);
    }

    public virtual void Defeat(PlayerType _type)
    {
        col.enabled = false;
        body.velocity = Vector2.zero;
        lance.ActorDefeated();
        legs.ActorDefeated();
        StartCoroutine(DeathAnimation());
    }

    protected virtual IEnumerator DeathAnimation()
    {
        SoundManager.instance.playSound(deathSound);
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
            else if (transform.position.y < -5.5f)
            {
                if (tag == "Player")
                {
                    GetComponent<Player>().collectable = 0;
                    Defeat(PlayerType.OTHER);
                }
                else
                {
                    transform.position = new Vector2(transform.position.x, 5.5f);
                }
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D _col)
    {
        if (tag == "Player")
        {
            CollisionEnterDetermineImpact(_col);
            CollisionStayDetermineImpact(_col);
        }
        else
        {
            if (_col.transform.tag == "Player")
            {
                CollisionEnterDetermineImpact(_col);
                CollisionStayDetermineImpact(_col);
            }
        }
    }

    private void CollisionEnterDetermineImpact(Collision2D _col)
    {
        ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        if (_rigSegment != null)
        {
            switch (CollisionDetermineImpact(_rigSegment, _col))
            {
                case CollisionEffect.LAND:
                    _rigSegment.rigBase.Defeat(_playerType);
                    break;
                case CollisionEffect.BACKSTABBER:
                    _rigSegment.rigBase.Defeat(_playerType);
                    break;
                case CollisionEffect.CLASH:
                    Clash.instance.HaveClash(_col.contacts[0].point);
                    break;
                case CollisionEffect.CLASH_WIN:
                    _rigSegment.rigBase.Defeat(_playerType);
                    break;
                default:
                    break;
            }
        }
    }

    private void CollisionStayDetermineImpact(Collision2D _col)
    {
        ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        if (_rigSegment != null)
        {
            switch (CollisionDetermineImpact(_rigSegment, _col))
            {
                case CollisionEffect.LAND:
                    _rigSegment.rigBase.ApplyKnockback(Vector3.down, 0.5f);
                    break;
                case CollisionEffect.CRUSHED:
                    _rigSegment.rigBase.ApplyKnockback(Vector3.up, 1.0f);
                    break;
                case CollisionEffect.BACKSTABBER:
                    _rigSegment.rigBase.ApplyKnockback(Vector3.right * col.transform.lossyScale.x, 0.75f);
                    break;
                case CollisionEffect.CLASH:
                    _rigSegment.rigBase.ApplyKnockback(Vector3.right * col.transform.lossyScale.x, 0.75f);
                    break;
                case CollisionEffect.CLASH_WIN:
                    _rigSegment.rigBase.ApplyKnockback(Vector3.right * col.transform.lossyScale.x, 0.75f);
                    break;
                default:
                    break;
            }
        }
    }

    protected CollisionEffect CollisionDetermineImpact(ISegmentable<Actor> _rigSegment, Collision2D _col)
    {
        //Attack from above
        if (_col.contacts[0].normal.y > 0.0f && transform.position.y > _col.transform.position.y + 0.1f)
        {
            return CollisionEffect.LAND;
        }
        else if(_col.contacts[0].normal.y < 0.0f && transform.position.y - 0.1f < _col.transform.position.y)
        {
            return CollisionEffect.CRUSHED;
        }
        //Attack from same direction
        else if (col.transform.lossyScale.x == _col.transform.lossyScale.x)
        {
            if ((col.transform.lossyScale.x * -_col.contacts[0].normal.x) > 0.0f)
            {
                return CollisionEffect.BACKSTABBER;
            }
            else
            {
                return CollisionEffect.BACKSTABBED;
            }
        }
        //Attack from different direction
        else
        {
            //Back clash
            if(_col.contacts[0].normal.x * transform.lossyScale.x > 0.0f)
            {
                return CollisionEffect.REVERSE_BUMP;
            }
            //Frontal clash
            if (Mathf.Abs(col.transform.position.y - _col.transform.position.y) < 0.2f)
            {
                return CollisionEffect.CLASH;
            }
            else
            {
                //Frontal win
                if (col.transform.position.y > _col.transform.position.y)
                {
                    return CollisionEffect.CLASH_WIN;
                }
                //Frontal loss
                else
                {
                    return CollisionEffect.CLASH_LOSE;
                }
            }
        }
        //return CollisionEffect.COUNT;
    }

    protected virtual void OnCollisionStay2D(Collision2D _col)
    {
        //Head hit platform
        if (_col.contacts[0].otherCollider == col)
        {
            if (_col.collider.tag == "Platform")
            {
                if (_col.contacts[0].normal.y > 0.0f)
                {
                    body.AddForce(new Vector2(_col.transform.position.x > transform.position.x ? -0.05f : 0.05f, 0.0f), ForceMode2D.Impulse);
                }
            }
        }

        if (tag == "Player")
        {
            CollisionStayDetermineImpact(_col);
        }
        else
        {
            if (_col.transform.tag == "Player")
            {
                CollisionStayDetermineImpact(_col);
            }
        }

        if (_col.transform.tag == "Player" || _col.transform.tag == "Enemy")
        {
            ApplyKnockback(_col.contacts[0].normal, 0.1f);
        }
    }

    public virtual void LandedOnPlatform(Collider2D col)
    {
        if (!onPlatform)
        {
            //Make sure the player hasn't sunk into the floor for some silly reason when we freeze the Y position of the player
            float landPosition = col.bounds.max.y;
            transform.position = new Vector3(transform.position.x, landPosition + 0.37f, transform.position.z);

            currentSurface = platformManager.instance.whatPlatformIsThis(col);

            if (!footStepComponent)
                footStepComponent = GetComponentInChildren<footstepPlayer>();
            footStepComponent.playFootstepSound();

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
                //skidMark.transform.position = transform.position;
            }
        }
    }

    public virtual void TakeOffFromPlatform()
    {
        if (onPlatform)
        {
            onPlatform = false;
            body.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            anim.Play("newGoose_flap");
        }
    }

    public virtual void DetermineAnimationState()
    {
        if (onPlatform)
        {
            if (Mathf.Abs(body.velocity.x) < 0.01f)
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
                    //Play a flapping sound, but make sure we don't play the same one twice in a row
                    int thisOne = Random.Range(0, flappingSounds.Count);
                    SoundManager.instance.playSound(flappingSounds[thisOne],0.25f);
                    AudioClip mostRecentSound = flappingSounds[thisOne];
                    if (lastFlapSound)
                        flappingSounds.Add(lastFlapSound);
                    flappingSounds.Remove(mostRecentSound);
                    lastFlapSound = mostRecentSound;

                    anim.Play("newGoose_flap");
                }
            }
        }
    }

}
public enum PlayerType
{
    BADGUY,
    GOODGUY,
    ENEMY,
    OTHER,
    COUNT
};