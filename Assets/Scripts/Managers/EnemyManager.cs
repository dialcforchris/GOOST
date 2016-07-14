using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager enemyManager = null;
    public static EnemyManager instance { get { return enemyManager; } }

    [SerializeField] private Enemy enemyPrefab = null;
    private ObjectPool<Enemy> objectPool = null;

    private Vector3[] spawnTransforms = null;

    private int numWave = 2;
    [SerializeField] private float spawnRate = 2.0f;
    private float spawnTime = 0;


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
        spawnTime += Time.deltaTime;
        if(spawnTime >= spawnRate)
        {
            if (numWave != 0)
            {
                --numWave;
                spawnTime = 0;
                Enemy _e = objectPool.GetPooledObject();
                _e.transform.position = SpawnPoint();
                _e.Spawn(EnemyBehaviour.RANDOM);
            }
        }
    }

    private Vector3 SpawnPoint()
    {
        int _spawnPoint = Random.Range(0, 3) * 2;
        return Vector3.Lerp(spawnTransforms[_spawnPoint], spawnTransforms[_spawnPoint + 1], Random.Range(0.0f, 1.0f));
    }
}
