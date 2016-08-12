using UnityEngine;
using System.Collections;

public class Junk : MonoBehaviour, IPoolable<Junk>
{
    #region IPoolable
    public PoolData<Junk> poolData { get; set; }
    #endregion

    float dead;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private Collider2D[] colliders;
    [SerializeField]
    private SpriteRenderer spRend;
    int rand = 0;

  void Awake()
    {
        dead = -6;// Camera.main.ViewportToWorldPoint(new Vector2(0, 1.1f)).y;
    }
    void Update()
    {
        if (transform.position.y<=dead)
        {
            ReturnPool();
        }
    }
    public void ReturnPool()
    {
        colliders[rand].enabled = false;
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
    public void OnPooled()
    {
        gameObject.SetActive(true);
        rand = Random.Range(0, 4);
        spRend.sprite = sprites[rand];
        colliders[rand].enabled = true;
    }
   
}
