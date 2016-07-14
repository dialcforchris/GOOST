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
    #endregion

    protected virtual void OnEnable()
    {
        platformManager.instance.NoCollisionsPlease(col);
    }

    public void ActorSpawned()
    {
        lanceActive = true;
    }

    public void ActorDefeated()
    {
        lanceActive = false;
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if (lanceActive)
        {
            foreach(string _s in affectTags)
            {
                if(_s == _col.gameObject.tag)
                {
                    while (true)
                    {
                        Lance _lance = _col.gameObject.GetComponent<Lance>();
                        if (_lance)
                        {
                            _lance.actor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, 0.0f), 10.0f);
                            Debug.Log(actor.tag + " lance clashs " + _col.gameObject.tag + " lance");
                            break;
                        }
                        //Legs _legs = _col.gameObject.GetComponent<Legs>();
                        //if (_legs)
                        //{
                        //    _legs.legsActor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, 0.0f), 10.0f);
                        //    Debug.Log(actor.tag + " lance clashs " + _col.gameObject.tag + " lance");
                        //    break;
                        //}
                        //Actor _actor = _col.gameObject.GetComponent<Actor>();
                        //if (_actor)
                        //{
                        //    _actor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.2f), 10.0f);
                        //    _actor.Defeat();
                        //    Debug.Log(actor.tag + " lance defeats " + _col.gameObject.tag + " body");
                        //    break;
                        //}
                        //ActorSegment _actorSegment = _col.gameObject.GetComponent<ActorSegment>();
                        //if (_actorSegment)
                        //{
                        //    _actorSegment.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.2f), 10.0f);
                        //    _actorSegment.Defeat();
                        //    Debug.Log(actor.tag + " lance defeats " + _col.gameObject.tag + " segment");
                        //    break;
                        //}
                        break;
                    }
                    break;
                }
            }
        }
    }
}
