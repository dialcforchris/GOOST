using UnityEngine;
using System.Collections;

public class platformManager : MonoBehaviour {

    private static platformManager PlatformManager = null;
    public static platformManager instance {get{return PlatformManager;}}

    [SerializeField] private platform[] allPlatforms = null;

	void Awake ()
    {
        PlatformManager = this;
	}
	
    public void NoCollisionsPlease(Collider2D col)
    {
        foreach(platform p in allPlatforms)
        {
            Physics2D.IgnoreCollision(col, p.platformCollider);
        }
    }

	void Update ()
    {
	
	}
}
