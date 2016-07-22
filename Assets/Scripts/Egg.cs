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
    private Collider2D col;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private Enemy magpie;
    [SerializeField]
    private Animator ani;
    [SerializeField]
    GameObject broken;
    float hatchTime = 0;
    float maxHatchTime = 6;
    float invinsible = 0;
    public int score = 50;
    private int _owningPlayer = 0;
    public int owningPlayer
    {
        get { return _owningPlayer; }
        set { _owningPlayer = value; }
    }
    Transform[] shellPos;
	void Start()
    {
        shellPos = broken.transform.GetComponentsInChildren<Transform>();
    }

   // Update is called once per frame
	void Update ()
    {
        if (getLaid)
        {
            getLaid = false;
        }
        if (InvincibilityTimer())
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
               GameObject brokenEgg = (GameObject)Instantiate(broken);
               brokenEgg.transform.position = transform.position;
               brokenEgg.transform.rotation = transform.rotation;
                hatchTime = 0;
                Enemy e = EnemyManager.instance.EnemyPool();
                e.transform.position = new Vector2(transform.position.x,transform.position.y+0.8f);
                e.Spawn((EnemyBehaviour)Random.Range(0, 5));
                ReturnPool();
            }
    }

 
    void OnCollisionEnter2D(Collision2D col)
    {
        if (InvincibilityTimer())
        {
            if (col.gameObject.tag == "Player")
            {
                ISegmentable<Actor> rigSegment = col.gameObject.GetComponent<ISegmentable<Actor>>();
                if (rigSegment != null)
                {
                    Player p = (Player)rigSegment.rigBase;
                    p.ChangeScore(score);
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
        body.AddForce(new Vector2(Random.Range(-0.8f,0.8f),Random.Range(1, 3)));
        body.constraints = RigidbodyConstraints2D.None;
        body.mass = 0.2f;
        _owningPlayer = 3;
        gameObject.SetActive(true);
    }

    public void ReturnPool()
    {
        
        invinsible = 0;
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

   bool InvincibilityTimer()
   {
        if (invinsible<0.2f)
        {
            invinsible += Time.deltaTime;
            return false;
        }
        return true;
   }
}
