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
    public int rand = 0;
    //0, metal computer terminal
    //1, goose cloning vat, metal + glass
    //2, wooden crate
    //3, metal barrel

    [SerializeField]
    AudioClip[] metalImpactSounds, bigMetalImpactSounds, woodImpactSounds;

  void Awake()
    {
        //Sorry, but what in the actual fuck is this
        dead = -6;// Camera.main.ViewportToWorldPoint(new Vector2(0, 1.1f)).y;
    }
    void Update()
    {
        if (transform.position.y <= dead)
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
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Junk>())
        {
            //col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 0.5f), ForceMode2D.Impulse);
        }
        //col.gameObject.tag == "Platform" || col.gameObject.tag == "Junk" &&
        if (GetComponent<Rigidbody2D>().velocity.y < -3.5f)
        {
            if (rand == 1)
                SoundManager.instance.playSound(bigMetalImpactSounds[Random.Range(0, bigMetalImpactSounds.Length)],0.2f,Random.Range(0.8f,1.2f));
            else if (rand != 2)
                SoundManager.instance.playSound(metalImpactSounds[Random.Range(0, metalImpactSounds.Length)], 0.2f, Random.Range(0.8f, 1.2f));
            else
                SoundManager.instance.playSound(woodImpactSounds[Random.Range(0, woodImpactSounds.Length)], 0.2f, Random.Range(0.8f, 1.2f));
        }
    }
}
