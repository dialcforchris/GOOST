﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISegmentable<T> where T : Actor
{ 
    T rigBase { get; }
    string segmentName { get; }
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

    public static bool originalJoustCollisions = true;

    protected virtual void OnEnable()
    {
        Physics2D.IgnoreCollision(col, lance.lanceCollider);
        Physics2D.IgnoreCollision(col, legs.legsCollider);
    }

    public virtual void ApplyKnockback(Vector2 _direction, float _power)
    {
        body.velocity = Vector2.zero;// new Vector2(0.0f, body.velocity.y);
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
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D _col)
    {
        if (originalJoustCollisions)
        {
            if (tag == "Player")
            {
                CollisionDetermineImpact(_col);
            }
            else
            {
                if (_col.transform.tag == "Player")
                {
                    CollisionDetermineImpact(_col);
                }
            }
        }
        else
        {
            if (tag == "Player")
            {
                ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
                if (_rigSegment != null)
                {
                    if (_rigSegment.segmentName == "Body" || _col.collider.tag == "Enemy")
                    {
                        if (_col.contacts[0].normal.y > 0.0f)
                        {
                            if (_col.contacts[0].otherCollider == col)
                            {
                                _rigSegment.rigBase.Defeat(_playerType);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CollisionDetermineImpact(Collision2D _col)
    {
        ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        if (_rigSegment != null)
        {
            Vector2 _normal = (Vector2)col.transform.position - _col.contacts[0].point;
            //if(_normal.x == 0.5739126f)
            //{
            //    Debug.Log("col");
            //}
            //Attack from above
            if (_col.contacts[0].normal.y > 0.0f && _normal.y > 0.1f)
            {
                _rigSegment.rigBase.ApplyKnockback(Vector3.down, 0.5f);
                ApplyKnockback(Vector3.up, 1.0f);
                _rigSegment.rigBase.Defeat(_playerType);
            }
            //Attack from behind
            else if (col.transform.lossyScale.x == _col.transform.lossyScale.x)
            {
                if ((col.transform.lossyScale.x * -_col.contacts[0].normal.x) > 0.0f)
                {
                    _rigSegment.rigBase.ApplyKnockback(Vector3.right * col.transform.lossyScale.x, 0.75f);
                    _rigSegment.rigBase.Defeat(_playerType);
                }
            }
            else
            {
                if (Mathf.Abs(col.transform.position.y - _col.transform.position.y) < 0.2f)
                {
                    _rigSegment.rigBase.ApplyKnockback(Vector3.right * col.transform.lossyScale.x, 0.75f);
                    Clash.instance.HaveClash(_col.contacts[0].point);
                }
                else
                {
                    if (col.transform.position.y > _col.transform.position.y)
                    {
                        _rigSegment.rigBase.ApplyKnockback(Vector3.right * col.transform.lossyScale.x, 0.75f);
                        _rigSegment.rigBase.Defeat(_playerType);
                    }
                }
            }
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D _col)
    {
        if (_col.contacts[0].otherCollider == col)
        {
            if (_col.collider.tag == "Platform")
            {
                if (_col.contacts[0].normal.y > 0.0f)
                {
                    body.AddForce(new Vector2(_col.transform.position.x > transform.position.x ? -0.05f : 0.05f, 0.0f), ForceMode2D.Impulse);
                }
            }

            ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
            if (_rigSegment != null)
            {
                if (_rigSegment.segmentName == "Body")
                {
                    _rigSegment.rigBase.ApplyKnockback(_col.contacts[0].normal, -0.5f);
                }        
            }
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
    COUNT
};