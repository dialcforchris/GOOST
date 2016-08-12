using UnityEngine;
using System.Collections;

public class FeatherManager : MonoBehaviour
{
    private static FeatherManager featherManager =null;
    public static FeatherManager instance
    {
        get { return featherManager; }
    }
    [SerializeField]
    ParticleSystem feathers;
	// Use this for initialization
	void Start()
    {
        if (featherManager == null)
        {
            featherManager = this;
        }
    }

    public void HaveSomeFeathers(Vector2 _pos)
    {
        feathers.transform.position = _pos;
        feathers.Emit(1);
    }
}
