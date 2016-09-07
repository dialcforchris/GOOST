using UnityEngine;
using System.Collections;

namespace GOOST
{
    public class platformManager : MonoBehaviour
    {

        public static platformManager instance;

        [SerializeField]
        public platform[] allPlatforms = null;

        [System.Serializable]
        public struct platform
        {
            public Collider2D collider;
            public platformTypes type;
        }

        public enum platformTypes
        {
            grass,
            wood,
            concrete,
            metal,
            ERROR
        }

        public platformTypes whatPlatformIsThis(Collider2D col)
        {
            foreach (platform p in allPlatforms)
            {
                if (p.collider == col)
                    return p.type;
            }
            return platformTypes.ERROR;
        }

        void Awake()
        {
            instance = this;
        }

        public void NoCollisionsPlease(Collider2D col)
        {
            foreach (platform p in allPlatforms)
            {
                Physics2D.IgnoreCollision(col, p.collider);
            }
        }
        public void CollisionsPlease(Collider2D col)
        {
            foreach (platform p in allPlatforms)
            {
                Physics2D.IgnoreCollision(col, p.collider, false);
            }
        }
    }
}