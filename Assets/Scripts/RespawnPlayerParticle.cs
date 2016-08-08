using UnityEngine;
using System.Collections;

public class RespawnPlayerParticle : MonoBehaviour {
   
    private static RespawnPlayerParticle respawnManager = null;
    public static RespawnPlayerParticle instance
    {
        get { return respawnManager; }
    }
    [SerializeField]
    ParticleSystem respawnLookNice;
    // Use this for initialization
    void Start()
    {
        if (respawnManager == null)
        {
            respawnManager = this;
        }
    }

    public void MakeLookNice(Vector2 _pos,Color _colour)
    {
        respawnLookNice.transform.position = _pos;
        if (respawnLookNice.isPlaying)
        {
            respawnLookNice.Stop();
        }
        respawnLookNice.startColor = _colour;
        respawnLookNice.Play();
        respawnLookNice.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }
}
