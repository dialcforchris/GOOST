using UnityEngine;
using System.Collections;

public class ActorSegment : MonoBehaviour, ISegmentable<Actor>
{
    [SerializeField] private Actor actor = null;
    [SerializeField] private Collider2D col = null;
    public Collider2D segmentCollider { get { return col; } }

    #region ISegmentable
    public Actor rigBase { get { return actor; } }
    public string segmentName { get { return "Segment"; } }
    #endregion


    private void Start()
    {
        platformManager.instance.NoCollisionsPlease(col);
    }


}
