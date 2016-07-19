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
    private Enemy magpie;
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
	}
    
    void Hatch()
    {
            if (hatchTime<maxHatchTime)
            {
                hatchTime += Time.deltaTime;
            }
            else
            {
                Instantiate(brokenEgg,new Vector2(transform.position.x,transform.position.y),transform.rotation);
                Destroy(gameObject);
                //spawn a magpie
                Enemy e = EnemyManager.instance.EnemyPool();
                e.transform.position = transform.position;
                e.Spawn((EnemyBehaviour)Random.Range(0, 5));
            }
    }

 
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            ISegmentable<Actor> rigSegment = col.gameObject.GetComponent<ISegmentable<Actor>>();
            if (rigSegment != null)
            {
                Player p = (Player)rigSegment.rigBase;
                if (!p.inNest&&!inNest&&!p.carryingEgg)
                {
                    p.carryingEgg = true;
                    ReturnPool();
                }
            }
        }
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
