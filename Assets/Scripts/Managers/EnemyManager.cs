using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Wave
{
    public WaveSettings[] settings;
}

[System.Serializable]
public struct WaveSettings
{
    public EnemyBehaviour enemySpawnOrder;
    public float spawnRate;
    public float speed;
}

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager enemyManager = null;
    public static EnemyManager instance { get { return enemyManager; } }

    [SerializeField] private Enemy enemyPrefab = null;
    private ObjectPool<Enemy> objectPool = null;

    [SerializeField] private Transform[] spawnTransforms = null;

    private float spawnTime = 0;

    [SerializeField] private Wave[] waves = null;
    private int currentWave = 0;
    private int spawnIndex = 0;

    [SerializeField] private float nextWaveLength = 3.0f;
    private float nextWaveTime = 0.0f;


    private void Awake()
    {
        enemyManager = this;
    }

    private void Start()
    {
        objectPool = new ObjectPool<Enemy>(enemyPrefab, 10, transform);
        Physics2D.IgnoreLayerCollision(10, 11);
    }

    private void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (spawnIndex < waves[currentWave].settings.Length)
            {
                spawnTime += Time.deltaTime;
                if (spawnTime >= waves[currentWave].settings[spawnIndex].spawnRate)
                {
                    spawnTime = 0;
                    Enemy _e = objectPool.GetPooledObject();
                    _e.transform.position = spawnTransforms[Random.Range(0, spawnTransforms.Length)].position;
                    _e.Spawn(waves[currentWave].settings[spawnIndex].enemySpawnOrder, waves[currentWave].settings[spawnIndex].speed);
                    ++spawnIndex;
                }
            }
            else
            {
                if (Enemy.numActive == 0)
                {
                    nextWaveTime += Time.deltaTime;
                    if (nextWaveTime >= nextWaveLength)
                    {
                        nextWaveTime = 0.0f;
                        ++currentWave;
                        Debug.Log("Wave:" + currentWave);
                        spawnIndex = 0;
                        if (currentWave == waves.Length)
                        {
                            --currentWave;
                        }
                    }
                }
            }
        }
    }

    public Enemy EnemyPool()
    {
        Enemy _enemy = objectPool.GetPooledObject();
     //   _enemy.OnPooled();
        return _enemy;
    }
}
