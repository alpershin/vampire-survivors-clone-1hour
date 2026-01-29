using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    private class EnemyPoolEntry
    {
        public string type;
        public GameObject prefab;
        public int initialSize = 10;
        [NonSerialized] public ObjectPool pool;
    }

    [SerializeField] private Transform target;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private float spawnRadiusPadding = 2f;
    [SerializeField] private List<EnemyPoolEntry> enemyPrefabs = new List<EnemyPoolEntry>();
    [SerializeField] private ObjectPool experienceGemPool;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        foreach (var entry in enemyPrefabs)
        {
            if (entry.prefab == null)
            {
                continue;
            }

            GameObject poolObj = new GameObject($"{entry.type}_Pool");
            poolObj.transform.SetParent(transform);
            var pool = poolObj.AddComponent<ObjectPool>();
            pool.Configure(entry.prefab, entry.initialSize);
            entry.pool = pool;
        }
    }

    private void Start()
    {
        if (target == null)
        {
            var player = FindObjectOfType<PlayerController>();
            target = player != null ? player.transform : null;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnWave(enemiesPerWave);
        }
    }

    private void SpawnWave(int count)
    {
        if (enemyPrefabs.Count == 0 || target == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var entry = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
            if (entry.pool == null)
            {
                continue;
            }

            GameObject enemyObj = entry.pool.Get();
            enemyObj.transform.position = GetSpawnPosition();

            var ai = enemyObj.GetComponent<EnemyAI>();
            var stats = ConfigManager.Instance?.GetEnemyStats(entry.type);
            ai?.Init(stats, target, e => OnEnemyDeath(e, entry.pool));
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        float radius = mainCamera != null
            ? (mainCamera.orthographicSize * mainCamera.aspect) + spawnRadiusPadding
            : 10f;

        float angle = UnityEngine.Random.value * Mathf.PI * 2f;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        return target != null ? target.position + offset : offset;
    }

    private void OnEnemyDeath(EnemyAI enemy, ObjectPool pool)
    {
        if (enemy == null)
        {
            return;
        }

        SpawnGem(enemy.transform.position, enemy.GetXpValue());
        pool?.Release(enemy.gameObject);
    }

    private void SpawnGem(Vector3 position, int xpValue)
    {
        if (experienceGemPool == null)
        {
            return;
        }

        GameObject gem = experienceGemPool.Get();
        gem.transform.position = position;
        var xp = gem.GetComponent<ExperienceGem>();
        xp?.Init(xpValue, experienceGemPool);
    }
}
