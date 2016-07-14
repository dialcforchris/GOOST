using UnityEngine;
using System.Collections;

public class Legs : MonoBehaviour, ISegmentable<Actor>
{
    [SerializeField] private Actor actor = null;
    public Actor legsActor { get { return actor; } }
    [SerializeField] private Collider2D col = null;
    public Collider2D legsCollider { get { return col; } }

    public bool legsActive = true;

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
        legsActive = true;
    }

    public void ActorDefeated()
    {
        legsActive = false;
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if (legsActive)
        {
            foreach (string _s in affectTags)
            {
                if (_s == _col.gameObject.tag)
                {
                    while (true)
                    {
                        Lance _lance = _col.gameObject.GetComponent<Lance>();
                        if (_lance)
                        {
                            _lance.lanceActor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.8f), 10.0f);
                            Debug.Log(actor.tag + " legs clashs " + _col.gameObject.tag + " lance");
                            break;
                        }
                        Actor _actor = _col.gameObject.GetComponent<Actor>();
                        if (_actor)
                        {
                            _actor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.8f), 10.0f);
                            _actor.Defeat();
                            Debug.Log(actor.tag + " legs defeats " + _col.gameObject.tag + " body");
                            break;
                        }
                        ActorSegment _actorSegment = _col.gameObject.GetComponent<ActorSegment>();
                        if (_actorSegment)
                        {
                            _actorSegment.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.8f), 10.0f);
                            _actorSegment.Defeat();
                            Debug.Log(actor.tag + " legs defeats " + _col.gameObject.tag + " segment");
                            break;
                        }
                        break;
                    }
                    break;
                }
            }
        }
        
    }
}
