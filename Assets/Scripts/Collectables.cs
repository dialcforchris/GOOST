using UnityEngine;
using System.Collections;

public class Collectables : MonoBehaviour, IPoolable<Collectables>
{
    #region IPoolable
    public PoolData<Collectables> poolData { get; set; }
    #endregion

    [SerializeField]
    private int score;
    [SerializeField]
    private int altScore;
    [SerializeField]
    PickUpType type;
    Vector2 boost = Vector2.zero;
    
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Rigidbody2D body;
    [SerializeField]
    SpriteRenderer spRend;
    [SerializeField]
    Collider2D colli;

    void Awake()
    {
   
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(colli, col.collider, true);
            Debug.Log("Ignoreing");
        }
        if (col.gameObject.tag == "Player")
        {
            ISegmentable<Actor> rigSegment = col.gameObject.GetComponent<ISegmentable<Actor>>();
            if (rigSegment != null)
            {
                Player p = (Player)rigSegment.rigBase;

                if (p.playerType == PlayerType.BADGUY)
                {
                    p.collectable += type == PickUpType.MONEY ? 1 : 0;
                    p.ChangeScore(type == PickUpType.MONEY ? score : altScore);
                }
                else
                {
                    p.collectable += type == PickUpType.HARDDRIVE ? 1 : 0;
                    p.ChangeScore(type == PickUpType.HARDDRIVE ?  score: altScore);
                }
            }
            ReturnPool();
        }
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }

    public void OnPooled(PickUpType _type)
    {
       gameObject.SetActive(true);
       type = _type;

       spRend.sprite = sprites[type == PickUpType.HARDDRIVE ? 0 : 1];
          
        gameObject.SetActive(true);

        body.drag = 1.0f;
        body.gravityScale = 1.0f;
       
      
        Physics2D.IgnoreLayerCollision(8, 10, true);
        Physics2D.IgnoreLayerCollision(9, 10, true);

        body.AddForce(new Vector2(Random.Range(-1.5f, 1.5f) * (Random.Range(0, 2) == 0 ? 1 : -1), Random.Range(1.15f, 2.5f)), ForceMode2D.Impulse);

       // body.AddForce(new Vector2(10, 50));
       
    }
}
public enum PickUpType
{
    MONEY,
    HARDDRIVE,
}