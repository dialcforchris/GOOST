using UnityEngine;
using System.Collections;

public class Clash : MonoBehaviour//, IPoolable<Clash>
{
    [SerializeField]
    AudioClip clashSound;
    [SerializeField]
    private ParticleSystem clash;
    static public Clash instance = null;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void HaveClash(Vector2 _pos)
    {
        SoundManager.instance.playSound(clashSound);
        clash.transform.position = _pos;
        clash.Play();
    }
    //#region IPoolable
    //public PoolData<Clash> poolData { get; set; }
    //#endregion

    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
