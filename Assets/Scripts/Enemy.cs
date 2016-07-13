using UnityEngine;
using System.Collections;

public enum EnemyBehavior
{
    RANDOM = 0, //Random movement, may attack when close
    AGGRESSIVE, //Progressively get more aggressive
    HUNTER, //Search and kill the player
    HIGH_FLYER, //Stay at the top of the screen
    COUNT
}

public class Enemy : Actor
{
    private void Start()
    {
        platformManager.instance.NoCollisionsPlease(col);
    }

    public void Spawn()
    {
        //anim.Play("fly");
    }

    private void FixedUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    anim.Play("goose_neck_up_extend");
        //}
    }

    private void Kill()
    {
        //anim.Stop();
    }
}
