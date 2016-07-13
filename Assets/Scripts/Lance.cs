using UnityEngine;
using System.Collections;

public class Lance : MonoBehaviour
{
    [SerializeField] private Actor actor = null;
    public Actor lanceActor { get { return actor; } }
    [SerializeField] private Collider2D col = null;
    public Collider2D lanceCollider { get { return col; } }

    public bool lanceActive = true;

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
            while (true)
            {
                Actor _actor = _col.gameObject.GetComponent<Actor>();
                if (_actor)
                {
                    _actor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.2f), 5.0f);
                    _actor.Defeat();
                    break;
                }
                ActorSegment _actorSegment = _col.gameObject.GetComponent<ActorSegment>();
                if (_actorSegment)
                {
                    _actorSegment.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, -0.2f), 5.0f);
                    _actorSegment.Defeat();
                    break;
                }
                Lance _lance = _col.gameObject.GetComponent<Lance>();
                if (_lance)
                {
                    _lance.actor.ApplyKnockback(new Vector2(_col.transform.position.x - transform.position.x, 0.0f), 3.0f);
                    actor.ApplyKnockback(new Vector2(transform.position.x - _col.transform.position.x, 0.0f), 3.0f);
                    break;
                }
                break;
            }
        }
    }
}
