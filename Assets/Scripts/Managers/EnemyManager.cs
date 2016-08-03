using UnityEngine;
using System.Collections.Generic;

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
    public List<Enemy> AllEnemies = new List<Enemy>();

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
        Enemy.total = 0;
    }

    private void Start()
    {
        objectPool = new ObjectPool<Enemy>(enemyPrefab, 10, transform);
        Physics2D.IgnoreLayerCollision(10, 11);
    }

    public void Reset()
    {
        Enemy.total = 0;
        Enemy.numActive = 0;
        currentWave = 0;
        for (int i = 0; i < AllEnemies.Count; i++)
        {
            AllEnemies[i].gameObject.SetActive(false);
            AllEnemies[i].poolData.ReturnPool(AllEnemies[i]);
        }
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
                    if (!AllEnemies.Contains(_e))
                        AllEnemies.Add(_e);
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
                        spawnIndex = 0;
                        if (currentWave == waves.Length)                     
                        {
                            //////////////////////////////////////
                            //////////////END THE GAME
                            //////////////////////////////////////
                            EndGameLogic.instance.TriggerGameEnd(true);
                           GameStateManager.instance.ChangeState(GameStates.STATE_GAMEOVER);
                           
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
