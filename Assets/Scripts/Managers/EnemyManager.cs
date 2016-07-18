using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Wave
{
    public EnemyBehaviour[] enemySpawnOrder;
    public float[] enemySpawnOrderRate; 
}

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager enemyManager = null;
    public static EnemyManager instance { get { return enemyManager; } }

    [SerializeField] private Enemy enemyPrefab = null;
    private ObjectPool<Enemy> objectPool = null;

    private Vector3[] spawnTransforms = null;

    private float spawnTime = 0;

    [SerializeField] private Wave[] waves = null;
    private int currentWave = 0;
    private int spawnIndex = 0;

    [SerializeField] private float nextWaveLength = 3.0f;
    private float nextWaveTime = 0.0f;


    private void Awake()
    {
        enemyManager = this;
        objectPool = new ObjectPool<Enemy>(enemyPrefab, 10, transform);
    }

    private void Start()
    {
        spawnTransforms = new Vector3[6];
        spawnTransforms[0] = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0.1f, 10.0f));
        spawnTransforms[1] = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0.9f, 10.0f));
        spawnTransforms[2] = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.1f, 10.0f));
        spawnTransforms[3] = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0.9f, 10.0f));
        spawnTransforms[4] = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 1.1f, 10.0f));
        spawnTransforms[5] = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 1.1f, 10.0f));
    }

    private void Update()
    {
       
        if (spawnIndex < waves[currentWave].enemySpawnOrder.Length)
        {
            spawnTime += Time.deltaTime;
            if (spawnTime >= waves[currentWave].enemySpawnOrderRate[spawnIndex])
            {
                spawnTime = 0;
                Enemy _e = objectPool.GetPooledObject();
                _e.transform.position = SpawnPoint();
                _e.Spawn(waves[currentWave].enemySpawnOrder[spawnIndex]);
                ++spawnIndex;
            }
        }
        else
        {
            if (Enemy.numActive == 0)
            {
                nextWaveTime += Time.deltaTime;
                if(nextWaveTime >= nextWaveLength)
                {
                    nextWaveTime = 0.0f;
                    ++currentWave;
                    spawnIndex = 0;
                    if (currentWave == waves.Length)
                    {
                        --currentWave;
                    }
                }
            }
        }
        
    }

    private Vector3 SpawnPoint()
    {
        int _spawnPoint = Random.Range(0, 3) * 2;
        return Vector3.Lerp(spawnTransforms[_spawnPoint], spawnTransforms[_spawnPoint + 1], Random.Range(0.0f, 1.0f));
    }
}
