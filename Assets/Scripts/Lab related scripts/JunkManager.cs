using UnityEngine;
using System.Collections;

namespace GOOST
{
    public class JunkManager : MonoBehaviour
    {

        float[] junkTime, maxJunks;
        [SerializeField]
        float maxJunkTime;
        [SerializeField]
        private Transform[] spawnPos;

        [SerializeField]
        float maxJunkPerSpawner, minJunkPerSpawner;

        void Awake()
        {
            junkTime = new float[spawnPos.Length];
            maxJunks = new float[spawnPos.Length];
            for (int i = 0; i < spawnPos.Length; i++)
            {
                Randomise(i);
            }
        }
        // Update is called once per frame
        void Randomise(int currentSpawn)
        {
            maxJunks[currentSpawn] = Random.Range(minJunkPerSpawner, maxJunkPerSpawner);
        }

        void Update()
        {
            if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY && MainMenu.instance.getLevel() == 1)
            {
                JunkTimer();
            }
        }
        void JunkTimer()
        {
            for (int i = 0; i < spawnPos.Length; i++)
            {
                if (junkTime[i] < maxJunks[i])
                {
                    junkTime[i] += Time.deltaTime;
                }
                else
                {
                    JunkPool.instance.PoolJunk(spawnPos[Random.Range(0, spawnPos.Length)].position);
                    junkTime[i] = 0;
                    Randomise(i);
                }
            }
        }
    }
}