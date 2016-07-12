using UnityEngine;
using System.Collections;

public class platformManager : MonoBehaviour {

    public static platformManager instance;
    [SerializeField]
    private platform[] allPlatforms;

	void Awake ()
    {
        instance = this;
	}
	
    public void NoCollisionsPlease(Collider2D col)
    {
        foreach(platform p in allPlatforms)
        {
            Physics2D.IgnoreCollision(col, p.thing);
        }
    }

	void Update ()
    {
	
	}
}
