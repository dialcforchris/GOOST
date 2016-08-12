using UnityEngine;
using System.Collections;

public class BrokenEgg : MonoBehaviour 
{
    [SerializeField]
    private Rigidbody2D[] bodies;


	// Use this for initialization
	void Awake () 
    {
	    foreach (Rigidbody2D r in bodies)
        {
            r.AddForce(new Vector2(Random.Range(-5, 5), 5), ForceMode2D.Impulse);
        }
        StartCoroutine(BreakEgg());
	}
	
	IEnumerator BreakEgg()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
