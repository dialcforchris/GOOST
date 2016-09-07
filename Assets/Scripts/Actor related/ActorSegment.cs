using UnityEngine;
using System.Collections;

namespace GOOST
{
    public class ActorSegment : MonoBehaviour, ISegmentable<Actor>
    {
        [SerializeField]
        private Actor actor = null;
        [SerializeField]
        private Collider2D col = null;
        public Collider2D segmentCollider { get { return col; } }

        #region ISegmentable
        public Actor rigBase { get { return actor; } }
        public string segmentName { get { return "Segment"; } }
        #endregion


        private void OnEnable()
        {
            platformManager.instance.NoCollisionsPlease(col);
        }

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
            if (_col.collider.tag == "Enemy")
            {
                ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
                if (rigSegment != null)
                {
                    ((Enemy)rigSegment.rigBase).FindTarget();
                }
            }
        }
    }
}