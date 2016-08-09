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
    [SerializeField]
    private Material[] rotParticles;
    // Use this for initialization
    void Start()
    {
        if (respawnManager == null)
        {
            respawnManager = this;
        }
    }

    public void MakeLookNice(Vector2 _pos,int _playerId)//Color _colour)
    {
        respawnLookNice.transform.position = _pos;
        if (respawnLookNice.isPlaying)
        {
            respawnLookNice.Stop();
        }
        respawnLookNice.GetComponent<Renderer>().material = rotParticles[_playerId];
        respawnLookNice.Play();
        respawnLookNice.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }
}
