using UnityEngine;
using System.Collections;

public class Lance : MonoBehaviour, ISegmentable<Actor>
{
    [SerializeField] private Actor actor = null;
    public Actor lanceActor { get { return actor; } }
    [SerializeField] private Collider2D col = null;
    public Collider2D lanceCollider { get { return col; } }

    public bool lanceActive = true;

    [SerializeField] private string[] affectTags = null;

    #region ISegmentable
    public Actor rigBase { get { return actor; } }
    public string segmentName { get { return "Lance"; } }
    #endregion

    [SerializeField] private float knockPower = 70.0f;

    private void OnEnable()
    {
        platformManager.instance.NoCollisionsPlease(col);
    }

    public void ActorSpawned()
    {
        lanceActive = true;
        col.enabled = true;
    }

    public void ActorDefeated()
    {
        lanceActive = false;
        col.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if(!lanceActive)
        {
            return;
        }

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
            if (tag == "Player")
            {
                if (_rigSegment.segmentName == "Lance")
                {
                    Clash.instance.HaveClash(_col.transform.position);
                }
                else if (_rigSegment.segmentName == "Legs")
                {
                    Clash.instance.HaveClash(_col.transform.position);
                }
            }
            ApplyOppositeForce(_rigSegment, -_col.contacts[0].normal);
        }
    }

    private void OnCollisionStay2D(Collision2D _col)
    {
        if (!lanceActive)
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
        if(_direction.x == 0.0f)
        {
            _direction += Vector2.right * (transform.lossyScale.x > 0.0f ? 0.5f : -0.5f);
        }

        _segment.rigBase.ApplyKnockback(_direction, knockPower); 
    }
}

