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


    [SerializeField]
    private string[] affectTags = null;

    #region ISegmentable
    public Actor rigBase { get { return actor; } }
    public string segmentName { get { return "Legs"; } }
    #endregion

    [SerializeField]
    private float knockPower = 50.0f;

    public void ActorSpawned()
    {
        col.enabled = true;
    }

    public void ActorDefeated()
    {
        col.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if (_col.collider.tag == "Platform")
        {
            if (_col.contacts[0].normal == Vector2.up)
            {
                actor.LandedOnPlatform();
            }
        }
        else
        {
            foreach (string _s in affectTags)
            {
                if (_s == _col.gameObject.tag)
                {
                    ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
                    if (rigSegment != null)
                    {
                        if (rigSegment.segmentName == "Lance")
                        {
                            rigSegment.rigBase.ApplyKnockback(new Vector2(_col.collider.transform.position.x - transform.position.x, -1.0f), knockPower);
                            Debug.Log(actor.tag + " legs clashs " + _col.gameObject.tag + " lance");
                            break;
                        }
                        else if (rigSegment.segmentName == _s)
                        {
                            rigSegment.rigBase.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -1.0f), knockPower);
                            rigSegment.rigBase.Defeat();
                            Debug.Log(actor.tag + " legs defeats " + _col.gameObject.tag + " body");
                            break;
                        }
                    }
                    break;
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
        }
    }

    private void OnCollisionExit2D(Collision2D _col)
    {
        if (_col.collider.tag == "Platform")
        {
            actor.TakeOffFromPlatform();
        }
    }
}
