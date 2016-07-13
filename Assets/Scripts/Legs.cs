using UnityEngine;
using System.Collections;

public class Legs : MonoBehaviour
{
    [SerializeField] private Actor actor = null;
    [SerializeField] private Collider2D col = null;
    public Collider2D legsCollider { get { return col; } }

    protected virtual void OnEnable()
    {
        platformManager.instance.NoCollisionsPlease(col);
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        while (true)
        {
            Actor _actor = _col.gameObject.GetComponent<Actor>();
            if (_actor)
            {
                _actor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.8f), 5.0f);
                _actor.Defeat();
                break;
            }
            ActorSegment _actorSegment = _col.gameObject.GetComponent<ActorSegment>();
            if (_actorSegment)
            {
                _actorSegment.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.8f), 5.0f);
                _actorSegment.Defeat();
                break;
            }
            Lance _lance = _col.gameObject.GetComponent<Lance>();
            if (_lance)
            {
                _lance.lanceActor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.8f), 5.0f);
                _lance.lanceActor.Defeat();
                break;
            }
            break;
        }
        
    }
}
