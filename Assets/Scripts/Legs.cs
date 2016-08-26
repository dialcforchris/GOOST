using UnityEngine;
using System.Collections;

public class Legs : MonoBehaviour, ISegmentable<Actor>
{
    [SerializeField]
    private Actor actor = null;
    public Actor legsActor { get { return actor; } }
    [SerializeField]
    private Collider2D col = null;
    public Collider2D legsCollider { get { return col; } }

    private bool legsActive = true;
    [SerializeField] private string[] affectTags = null;

    #region ISegmentable
    public Actor rigBase { get { return actor; } }
    public string segmentName { get { return "Legs"; } }
    #endregion

    [SerializeField]
    private float knockPower = 50.0f;

    public void ActorSpawned()
    {
        legsActive = true;
        col.enabled = true;
    }

    public void ActorDefeated()
    {
        legsActive = false;
        col.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D _col)
    {
        if (!legsActive)
        {
            return;
        }

        if (_col.collider.tag == "Platform")
        {
            if (_col.contacts[0].normal.y > 0)
            {
                actor.LandedOnPlatform(_col.collider);
            }
            else if (_col.contacts[0].normal.x != 0.0f)
            {
                if (tag == "Enemy")
                {
                    ((Enemy)actor).PlatformSideCollision(_col);
                }
            }
        } 
    }

    private void OnCollisionStay2D(Collision2D _col)
    {
        if (!legsActive)
        {
            return;
        }
    }

    //bool isExiting;
    //float exitTimer;

    //void Update()
    //{
    //    if (isExiting)
    //        exitTimer += Time.deltaTime;

    //    if (exitTimer > 0.1f)
    //    {
    //        exitTimer = 0;
    //        isExiting = false;
    //        actor.TakeOffFromPlatform();
    //    }
    //}

    private void OnCollisionExit2D(Collision2D _col)
    {
        if (_col.collider.tag == "Platform")
        {
            actor.TakeOffFromPlatform();
            //isExiting = true;
        }
    }
}
