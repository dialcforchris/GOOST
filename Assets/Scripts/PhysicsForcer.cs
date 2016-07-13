using UnityEngine;
using System.Collections;

public class PhysicsForcer : MonoBehaviour
{

   public bool once;
    void Update()
    {
        if (gameObject.activeInHierarchy && !once)
        {
            once = true;
            GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(1,1), transform.parent.position, ForceMode2D.Impulse);
        }
        StartCoroutine(WaitForDestroy());
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
