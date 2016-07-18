using UnityEngine;
using System.Collections;

public class SilverCoin : MonoBehaviour, IPoolable<SilverCoin>
{
    #region IPoolable
    public PoolData<SilverCoin> poolData { get; set; }
    #endregion

    [SerializeField]
    Rigidbody2D rig;

    public int score;
    Transform target;
    bool collected;

    void spawn()
    {
        StartCoroutine(expandCoin());
        Random.seed = System.DateTime.Now.Millisecond;
        rig.AddForce(new Vector2(Random.Range(-3.5f, 3.5f), 5), ForceMode2D.Impulse);
    }

    void Start()
    {
        platformManager.instance.NoCollisionsPlease(rig.GetComponent<Collider2D>());
        spawn();
    }

    IEnumerator expandCoin()
    {
        float lerpy = 0;
        while (lerpy < 1)
        {
            transform.localScale = Vector3.one * lerpy;

            lerpy += Time.deltaTime*2;
            yield return new WaitForEndOfFrame();
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && !collected)
        {
           ISegmentable < Actor > rigSegment = col.GetComponent<ISegmentable<Actor>>();
            if (rigSegment != null)
            {
                Player p = (Player)rigSegment.rigBase;
                p.ChangeScore(score);
            }
            target = col.transform;
            collected = true;
            StartCoroutine(collectCoin());
        }
    }
    
    IEnumerator collectCoin()
    {
        //shrink in size
        //Move towards target

        float lerpy = 0;
        while (lerpy < 1)
        {
            if (transform.localScale.x > 0)
                transform.localScale = Vector3.one * (1 - (lerpy * 1.5f));

            transform.position = Vector3.Lerp(transform.position, target.position, lerpy);
            lerpy += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ////Apply score addition or w/e to player.
        //if (target.GetComponent<Player>())
        //    target.GetComponent<Player>().ChangeScore(score);
        //else
        //    Debug.Log("Player component not found");

        //should probably do something with object pooling now.
        gameObject.SetActive(false);
        transform.localScale = Vector3.one;
        collected = false;
    }
    public void OnPooled()
    {
       gameObject.SetActive(true);
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
