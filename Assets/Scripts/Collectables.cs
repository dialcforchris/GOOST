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
 
    

    void OnTriggerEnter2D(Collider2D col)
    {
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
                    p.ChangeScore(type == PickUpType.HARDDRIVE ? altScore : score);
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

    }
}
public enum PickUpType
{
    MONEY,
    HARDDRIVE,
}