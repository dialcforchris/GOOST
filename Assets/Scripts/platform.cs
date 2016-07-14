using UnityEngine;
using System.Collections;

public class platform : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    public Collider2D platformCollider { get { return col; } }

    void OnTriggerEnter2D(Collider2D _col)
    {
        Legs _legs = _col.gameObject.GetComponent<Legs>();
        if (_legs)
        {
            if (_legs.tag == "Player")
            {
                Physics2D.IgnoreCollision(_col, col, false);
            }
        }

    }

    void OnTriggerExit2D(Collider2D _col)
    {
        Legs _legs = _col.gameObject.GetComponent<Legs>();
        if (_legs)
        {
            if (_legs.tag == "Player")
            {
                Physics2D.IgnoreCollision(_col, col, true);
            }
        }
    }
}
