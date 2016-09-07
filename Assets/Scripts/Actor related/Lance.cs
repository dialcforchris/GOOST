using UnityEngine;
using System.Collections;

namespace GOOST
{
    public class Lance : MonoBehaviour, ISegmentable<Actor>
    {
        [SerializeField]
        private Actor actor = null;
        public Actor lanceActor { get { return actor; } }
        [SerializeField]
        private Collider2D col = null;
        public Collider2D lanceCollider { get { return col; } }

        public bool lanceActive = true;

        [SerializeField]
        private string[] affectTags = null;

        #region ISegmentable
        public Actor rigBase { get { return actor; } }
        public string segmentName { get { return "Lance"; } }
        #endregion

        [SerializeField]
        private float knockPower = 70.0f;

        private void OnEnable()
        {
            platformManager.instance.NoCollisionsPlease(col);
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
        }

        private void OnCollisionStay2D(Collision2D _col)
        {
            if (!lanceActive)
            {
                return;
            }
        }
    }
    }