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
   //     platformManager.instance.NoCollisionsPlease(col);
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
        if (!lanceActive)
        {
            return;
        }

        foreach(string _s in affectTags)
        {
            if(_s == _col.gameObject.tag)
            {
                ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
                if (rigSegment != null)
                {
                    if (rigSegment.segmentName == "Lance")
                    {
                        if(_col.contacts[0].normal.x != 0.0f)
                        { 
                            rigSegment.rigBase.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, 0.0f), knockPower);
                            Debug.Log(actor.tag + " lance clashs " + _col.gameObject.tag + " lance");
                        }
                        break;
                    }
                    else if (rigSegment.segmentName == "Legs")
                    {
                        rigSegment.rigBase.ApplyKnockback(_col.collider.transform.position - transform.position, knockPower);
                        Debug.Log(actor.tag + " lance clashs " + _col.gameObject.tag + " lance");
                        break;
                    }
                    else if (rigSegment.segmentName == _s)
                    {
                        rigSegment.rigBase.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.2f), knockPower);
                        rigSegment.rigBase.Defeat();
                        Debug.Log(actor.tag + " lance defeats " + _col.gameObject.tag + " body");
                        break;
                    }
                }
                break;
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
    
}
