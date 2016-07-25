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
        ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        if (rigSegment != null)
        {
            foreach (string _s in affectTags)
            {
                if (_s == _col.gameObject.tag)
                {
                    rigSegment.rigBase.Defeat();
                    break;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D _col)
    {
        if (!legsActive)
        {
            return;
        }

        ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
        if (rigSegment != null)
        {
            if (rigSegment.segmentName == "Lance")
            {
                rigSegment.rigBase.ApplyKnockback(new Vector2(_col.collider.transform.position.x - transform.position.x, -1.0f), knockPower);
            }
            else if (rigSegment.segmentName == "Player" || rigSegment.segmentName == "Enemy")
            {
                rigSegment.rigBase.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -1.0f), knockPower); 
            }
        }
    }


    private void OnCollisionExit2D(Collision2D _col)
    {
        if(_col.contacts[0].otherCollider != col)
        {
            Debug.Log("issue");
        }

        if (_col.collider.tag == "Platform")
        {
            actor.TakeOffFromPlatform();
        }
    }
}
