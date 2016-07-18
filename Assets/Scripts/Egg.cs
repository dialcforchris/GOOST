using UnityEngine;
using System.Collections;

public class Egg : MonoBehaviour, IPoolable<Egg>
{
    #region IPoolable
    public PoolData<Egg> poolData { get; set; }
    #endregion

    public bool getLaid = false;
    public bool inNest = false;
    [SerializeField]
    private GameObject brokenEgg;
    [SerializeField]
    private Collider2D col;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private GameObject magpie;
    [SerializeField]
    private Animator ani;
    float hatchTime = 0;
    float maxHatchTime = 6;
    private int _owningPlayer = 0;
    public int owningPlayer
    {
        get { return _owningPlayer; }
        set { _owningPlayer = value; }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (getLaid)
        {
            getLaid = false;
        }
        Hatch();
      //  DisablePhysics(inNest);
	}
    
    void Hatch()
    {
        if (!inNest&&transform.parent ==null)
        {
            if (hatchTime<maxHatchTime)
            {
                hatchTime += Time.deltaTime;
            }
            else
            {
                Instantiate(brokenEgg,new Vector2(transform.position.x,transform.position.y +1),transform.rotation);
                Destroy(gameObject);
                //spawn a magpie
            }
        }
        else
        {
            hatchTime = 0;
        }
    }

   
    void OnTriggerEnter2D(Collider2D col)
    {
        //if (col.gameObject.tag == "Player")
        //{
        //    if (col.gameObject.GetComponent<Player>().playerId != _owningPlayer)
        //    {
        //        transform.SetParent(col.transform);
        //    }
        //    else
        //    {
        //        if (transform.parent)
        //        {
        //            transform.SetParent(null);
        //        }
        //    }
        //}
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //if (col.gameObject.tag == "Player")
        //{
        //    if (col.gameObject.GetComponent<Player>().playerId != _owningPlayer)
        //    {
        //        transform.SetParent(col.gameObject.GetComponent<Player>().eggTrans.transform);
        //    }
        //}
    }

    public void OnPooled()
    {
        if (getLaid)
        {
            ani.Play("Laid");
        }
        col.isTrigger = false;
        body.gravityScale = 1;
        body.constraints = RigidbodyConstraints2D.None;
        body.mass = 0.2f;
        _owningPlayer = 3;
        gameObject.SetActive(true);
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
   public void DisablePhysics(bool _disable)
    {
        if (_disable)
        {
            col.isTrigger = true;
            body.gravityScale = 0;
            body.constraints = RigidbodyConstraints2D.FreezePosition;
            body.mass = 0;

        }
        else
        {
            col.isTrigger = false;
            body.gravityScale = 1;
            body.constraints = RigidbodyConstraints2D.None;
            body.mass = 0.2f;
            _owningPlayer = 3;
        }
    }
}
