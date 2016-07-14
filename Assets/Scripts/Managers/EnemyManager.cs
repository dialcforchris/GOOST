using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager enemyManager = null;
    public static EnemyManager instance { get { return enemyManager; } }

    [SerializeField] private Enemy enemyPrefab = null;
    private ObjectPool<Enemy> objectPool = null;

    [SerializeField] private Transform[] spawnTransforms = null;

    private int numWave = 1;
    [SerializeField] private float spawnRate = 2.0f;
    private float spawnTime = 0;


    private void Awake()
    {
        enemyManager = this;
        objectPool = new ObjectPool<Enemy>(enemyPrefab, 10, transform);
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
                _e.transform.position = spawnTransforms[Random.Range(0, spawnTransforms.Length)].position;
                _e.Spawn(EnemyBehaviour.RANDOM);
            }
        }
    }
}
