﻿using UnityEngine;
using System.Collections;

namespace GOOST
{
    public class Egg : MonoBehaviour, IPoolable<Egg>
    {
        #region IPoolable
        public PoolData<Egg> poolData { get; set; }
        #endregion

        public bool getLaid = false;
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
        [SerializeField]
        AudioClip eggHatch;
        [SerializeField]
        AudioClip eggGet;

        private EnemyBehaviour parentType;
        private float parentSpeed;

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
        void Update()
        {
            if (!EggPool.instance.OneBasket.Contains(this))
                EggPool.instance.OneBasket.Add(this);
            if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
            {
                if (getLaid)
                {
                    getLaid = false;
                }
                if (InvincibilityTimer())
                    Hatch();
            }
        }

        void Hatch()
        {
            if (hatchTime < maxHatchTime)
            {
                hatchTime += Time.deltaTime;
            }
            else
            {
                SoundManager.instance.playSound(eggHatch);
                GameObject brokenEgg = (GameObject)Instantiate(broken);
                brokenEgg.transform.position = transform.position;
                brokenEgg.transform.rotation = transform.rotation;
                hatchTime = 0;
                Enemy e = EnemyManager.instance.EnemyPool();
                EnemyManager.instance.AllEnemies.Add(e);
                e.transform.position = new Vector2(transform.position.x, transform.position.y + 0.8f);
                e.Spawn(parentType, parentSpeed);
                ReturnPool();
            }
        }

        void OnCollisionStay2D(Collision2D col)
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
                        FloatingTextPool.instance.PoolText("" + score, transform.position, Color.white);
                        ReturnPool();
                        SoundManager.instance.playSound(eggGet);
                        GameStats.instance.eggs[p.playerId]++;
                        StatTracker.instance.stats.eggsCollected++;
                    }
                }
            }
        }

        public void OnPooled(EnemyBehaviour _parentType, float _parentSpeed)
        {
            if (getLaid)
            {
                ani.Play("Laid");
            }
            //  col.isTrigger = false;
            body.gravityScale = 1;
            body.AddForce(new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(1, 3)));
            body.constraints = RigidbodyConstraints2D.None;
            body.mass = 0.2f;
            _owningPlayer = 3;
            parentType = _parentType;
            parentSpeed = _parentSpeed;
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
            if (invinsible < 0.2f)
            {
                invinsible += Time.deltaTime;
                return false;
            }
            return true;
        }
    }
}