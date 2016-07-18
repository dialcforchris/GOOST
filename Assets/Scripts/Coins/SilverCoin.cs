using UnityEngine;
using System.Collections;

public class SilverCoin : MonoBehaviour, IPoolable<SilverCoin>
{
    #region IPoolable
    public PoolData<SilverCoin> poolData { get; set; }
    #endregion

    [SerializeField] private Rigidbody2D body = null;
    [SerializeField] private Collider2D col = null;
    [SerializeField] private CoinSuction suction = null;

    private float pickupTime = 0.0f;
    [SerializeField] private float pickupLength = 0.5f;

    [SerializeField] private float minSpawnX = 2.0f;
    [SerializeField] private float maxSpawnX = 4.5f;
    [SerializeField] private float minSpawnY = 5.0f;
    [SerializeField] private float maxSpawnY = 8.0f;

    [SerializeField] private int score;

    [SerializeField] private AnimationCurve animCurve = null;

    IEnumerator expandCoin()
    {
        while (pickupTime < pickupLength)
        {
            pickupTime = pickupTime + Time.deltaTime < pickupLength ? pickupTime + Time.deltaTime : pickupLength;

            transform.localScale = Vector3.one * animCurve.Evaluate(pickupTime / pickupLength);

            yield return new WaitForEndOfFrame();
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
        suction.Enable();
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if (pickupTime == pickupLength)
        {
            if (_col.collider.tag == "Player")
            {
                ISegmentable<Actor> rigSegment = _col.collider.GetComponent<ISegmentable<Actor>>();
                if (rigSegment != null)
                {
                    Player p = (Player)rigSegment.rigBase;
                    p.ChangeScore(score);
                    col.enabled = false;
                    suction.Disable();
                    ReturnPool();
                }
            }
        }
    }
    
    //IEnumerator collectCoin()
    //{
    //    //shrink in size
    //    //Move towards target

    //    float lerpy = 0;
    //    while (lerpy < 1)
    //    {
    //        if (transform.localScale.x > 0)
    //            transform.localScale = Vector3.one * (1 - (lerpy * 1.5f));

    //        transform.position = Vector3.Lerp(transform.position, target.position, lerpy);
    //        lerpy += Time.deltaTime;
    //        yield return new WaitForEndOfFrame();
    //    }
 
    //    transform.localScale = Vector3.one;
    //    ReturnPool();
    //}

    public void OnPooled()
    {
        gameObject.SetActive(true);
        pickupTime = 0.0f;
        StartCoroutine(expandCoin());
        Random.seed = System.DateTime.Now.Millisecond;
        body.AddForce(new Vector2(Random.Range(minSpawnX, maxSpawnX) * (Random.Range(0,2) == 0 ? 1 : -1), Random.Range(minSpawnY, maxSpawnY)), ForceMode2D.Impulse);
        col.enabled = true;
        Physics2D.IgnoreLayerCollision(8, 9, true);
        suction.Disable();
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
