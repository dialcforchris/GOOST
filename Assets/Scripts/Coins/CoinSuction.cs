using UnityEngine;
using System.Collections;

public class CoinSuction : MonoBehaviour
{
    [SerializeField] private Collider2D col = null;
    [SerializeField] private Rigidbody2D body = null;
    [SerializeField] private Collider2D physicsCol = null;
    [SerializeField] private float suctionSpeed = 3.0f;

    public void Enable()
    {
        col.enabled = true;
    }

    public void Disable()
    {
        col.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            float _speedMultiplier = Vector3.SqrMagnitude(_col.transform.position - transform.position);
            body.AddForce((_col.transform.position - transform.position).normalized * _speedMultiplier * suctionSpeed);
            platformManager.instance.NoCollisionsPlease(physicsCol);
        }
    }

    private void OnTriggerExit2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            physicsCol.enabled = true;
            platformManager.instance.CollisionsPlease(physicsCol);
        }
    }
}
