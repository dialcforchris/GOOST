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
            body.mass = 0.0f;
            body.drag = 0.0f;
            body.gravityScale = 0.0f;
        }
    }

    private void OnTriggerExit2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            physicsCol.enabled = true;
            platformManager.instance.CollisionsPlease(physicsCol);
            body.mass = 1.0f;
            body.drag = 1.0f;
            body.gravityScale = 1.0f;
        }
    }
}
