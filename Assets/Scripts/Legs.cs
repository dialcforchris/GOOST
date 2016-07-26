using UnityEngine;
using System.Collections;

public class Legs : MonoBehaviour, ISegmentable<Actor>
{
    [SerializeField]
    private Actor actor = null;
    public Actor legsActor { get { return actor; } }
    [SerializeField]
    private Collider2D col = null;
    public Collider2D legsCollider { get { return col; } }

    private bool legsActive = true;
    [SerializeField] private string[] affectTags = null;

    #region ISegmentable
    public Actor rigBase { get { return actor; } }
    public string segmentName { get { return "Legs"; } }
    #endregion

    [SerializeField]
    private float knockPower = 50.0f;

    public void ActorSpawned()
    {
        legsActive = true;
        col.enabled = true;
    }

    public void ActorDefeated()
    {
        legsActive = false;
        col.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if (!legsActive)
        {
            return;
        }

        if (_col.collider.tag == "Platform")
        {
            if (_col.contacts[0].normal.y > 0)
            {
                actor.LandedOnPlatform(_col.collider);
            }
            else if (_col.contacts[0].normal.x != 0.0f)
            {
                if (tag == "Enemy")
                {
                    ((Enemy)actor).PlatformSideCollision(_col);
                }
            }
        }
        else
        {
            ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
            if (_rigSegment != null)
            {
                foreach (string _s in affectTags)
                {
                    if (_s == _col.gameObject.tag)
                    {
                        if (_rigSegment.segmentName == "Body")
                        {
                            //if (_col.contacts[0].normal.x != 0.0f)
                            //{
                            _rigSegment.rigBase.Defeat(actor.playerType);
                            return;
                            //}
                        }
                        break;
                    }
                }
                ApplyOppositeForce(_rigSegment, -_col.contacts[0].normal);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D _col)
    {
        if (!legsActive)
        {
            return;
        }

        ISegmentable<Actor> _rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        if (_rigSegment != null)
        {
            ApplyOppositeForce(_rigSegment, -_col.contacts[0].normal);
        }
    }

    private void ApplyOppositeForce(ISegmentable<Actor> _segment, Vector2 _direction)
    {
        if (_direction.y == 0.0f)
        {
            _direction += Vector2.down * 0.5f;
        }

        _segment.rigBase.ApplyKnockback(_direction, knockPower);
    }


    private void OnCollisionExit2D(Collision2D _col)
    {
        if (_col.collider.tag == "Platform")
        {
            actor.TakeOffFromPlatform();
        }
    }
}
