﻿using UnityEngine;
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

    float life = 0;
    float maxLife = 2;
    float flash = 0;
    float invincible = 0;
    float invincibleDuration = 0.1f;
    bool canFlash = false;
    bool visible = true;
    
    void Update()
    {
        Life();
        FlashTime(canFlash);
        Flash(visible);
        if(invincible < invincibleDuration)
        {
            InvincibilityTimer();
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        //if (col.gameObject.tag == "Enemy")
        //{
        //    Physics2D.IgnoreCollision(colli, col.collider, true);
        //}

        //if (InvincibilityTimer())
        //{
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
                        p.ChangeScore(type == PickUpType.HARDDRIVE ? score : altScore);
                    }
                    ReturnPool();
                }
            }
        //}
    }

    public void ReturnPool()
    {
        
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }

    public void OnPooled(PickUpType _type)
    {
       life = 0;
       flash = 0;
       canFlash = false;
       visible = true;
       gameObject.SetActive(true);
       type = _type;
       invincible = 0;
       spRend.sprite = sprites[type == PickUpType.HARDDRIVE ? 0 : 1];
          
        gameObject.SetActive(true);

        body.drag = 1.0f;
        body.gravityScale = 1.0f;
       
      
        Physics2D.IgnoreLayerCollision(8, 11, true);
        Physics2D.IgnoreLayerCollision(9, 11, true);

        body.AddForce(new Vector2(Random.Range(-0.5f, 0.5f) * (Random.Range(0, 2) == 0 ? 1 : -1), Random.Range(0.5f, 1.8f)), ForceMode2D.Impulse);
    }

    void Life()
    {
        if (life<maxLife)
        {
            life += Time.deltaTime;
        }
        
        else
        {
            if (!canFlash)
            {
                life = 0;
                canFlash = true;
                return;
            }
            else
            {
                ReturnPool();
            }
        }
    }

    void FlashTime(bool _canFlash)
    {
        if (_canFlash)
        {
            if (flash < 0.05f)
            {
                flash += Time.deltaTime;
            }
            else
            {
                visible = !visible;
                flash = 0;
            }
        }
    }
    void Flash(bool _on)
    {
        spRend.enabled = _on;
    }

    bool InvincibilityTimer()
    {
        invincible += Time.deltaTime;
        if (invincible >= invincibleDuration)
        {
            Physics2D.IgnoreLayerCollision(8, 11, false);
            Physics2D.IgnoreLayerCollision(9, 11, false);
            return true;
        }
        return false;
    }
}
public enum PickUpType
{
    MONEY,
    HARDDRIVE,
}