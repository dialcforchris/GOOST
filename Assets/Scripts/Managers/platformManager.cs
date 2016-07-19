using UnityEngine;
using System.Collections;

public class platformManager : MonoBehaviour {

    private static platformManager PlatformManager = null;
    public static platformManager instance {get{return PlatformManager;}}

    [SerializeField] private Collider2D[] allPlatforms = null;

	void Awake ()
    {
        PlatformManager = this;
	}
	
    public void NoCollisionsPlease(Collider2D col)
    {
        foreach(Collider2D p in allPlatforms)
        {
            Physics2D.IgnoreCollision(col, p);
        }
    }
    public void CollisionsPlease(Collider2D col)
    {
        foreach (Collider2D p in allPlatforms)
        {
            Physics2D.IgnoreCollision(col, p, false);
        }
    }
}
