using UnityEngine;
using System.Collections;

public class ActorSegment : MonoBehaviour, ISegmentable<Actor>
{
    [SerializeField] private Actor actor = null;
    [SerializeField] private Collider2D col = null;
    public Collider2D segmentCollider { get { return col; } }

    #region ISegmentable
    public Actor rigBase { get { return actor; } }
    #endregion


    private void Start()
    {
        platformManager.instance.NoCollisionsPlease(col);
    }

    public void ApplyKnockback(Vector2 _direction, float _power)
    {
        actor.ApplyKnockback(_direction, _power);
    }

    public virtual void Defeat()
    {
        actor.Defeat();
    }
}
