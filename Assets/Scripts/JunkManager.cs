using UnityEngine;
using System.Collections;

public class JunkManager : MonoBehaviour
{

    float[] junkTime = new float[4];
    float maxJunkTime;
    float[] maxJunks = new float[4];
    [SerializeField]
    private Transform[] spawnPos;
   
    void Awake()
    {
        for (int i=0;i<4;i++)
        {
            Randomise(i);
        }
    }
    // Update is called once per frame
    void Randomise(int currentSpawn)
    {
        maxJunks[currentSpawn] = Random.Range(5.5f, 8.5f);
    }

    void Update ()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)//  && MainMenu.instance.getLevel() == 1)
        {
            JunkTimer();
        }
	}
   void JunkTimer()
    {
        for (int i = 0; i < junkTime.Length;i++)
        {
            if (junkTime[i]<maxJunks[i])
            {
                junkTime[i] += Time.deltaTime;
            }
            else
            {
                JunkPool.instance.PoolJunk(spawnPos[Random.Range(0,4)].position);
                junkTime[i] = 0;
                Randomise(i);

            }
        }
    }
}
