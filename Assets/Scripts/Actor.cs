using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected Collider2D col = null;
    public Collider2D actorCollider { get { return col; } }
    [SerializeField] protected Rigidbody2D body = null;
    [SerializeField] protected SpriteRenderer renderer = null;
    

    [SerializeField] protected Lance lance = null;

    [SerializeField] protected ActorSegment[] segments = null;

    private bool extending = false;

    protected virtual void OnEnable()
    {
        //Ignore collisions with child objects
        for(int i = 0; i < segments.Length; ++i)
        {
            Physics2D.IgnoreCollision(col, segments[i].segmentCollider);
            for(int j = i + 1; j < segments.Length; ++j)
            {
                Physics2D.IgnoreCollision(segments[i].segmentCollider, segments[j].segmentCollider);
            }
            Physics2D.IgnoreCollision(segments[i].segmentCollider, lance.lanceCollider);
        }
        Physics2D.IgnoreCollision(col, lance.lanceCollider);
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

    public void ApplyKnockback(Vector2 _direction, float _power)
    {
        body.velocity = new Vector2(0.0f, body.velocity.y);
        body.AddForce(_direction.normalized * _power);
    }

    public virtual void Defeat()
    {
        lance.ActorDefeated();
    }
}
