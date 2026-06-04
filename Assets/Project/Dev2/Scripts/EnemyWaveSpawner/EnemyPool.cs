using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [Header("Default Pool")]
    [SerializeField] private EnemyData defaultEnemyData;
    [SerializeField, Min(0)] private int initialDefaultPoolSize = 10;

    [Header("Hierarchy")]
    [SerializeField] private Transform poolRoot;

    private readonly Dictionary<EnemyData, Queue<GameObject>> availableEnemies = new Dictionary<EnemyData, Queue<GameObject>>();
    private readonly Dictionary<GameObject, EnemyData> enemyDataByInstance = new Dictionary<GameObject, EnemyData>();
    private readonly HashSet<GameObject> inactiveEnemies = new HashSet<GameObject>();

    private void Awake()
    {
        if (poolRoot == null)
        {
            poolRoot = transform;
        }

        if (defaultEnemyData != null)
        {
            Prewarm(defaultEnemyData, initialDefaultPoolSize);
        }
    }

    public void Prewarm(LevelDataConfig levelDataConfig)
    {
        if (levelDataConfig == null)
        {
            return;
        }

        Dictionary<EnemyData, int> enemyCounts = new Dictionary<EnemyData, int>();

        foreach (WaveConfig wave in levelDataConfig.Waves)
        {
            if (wave == null)
            {
                continue;
            }

            foreach (EnemyData enemyData in wave.EnemiesToSpawn)
            {
                if (enemyData == null)
                {
                    continue;
                }

                enemyCounts.TryGetValue(enemyData, out int currentCount);
                enemyCounts[enemyData] = currentCount + 1;
            }
        }

        foreach (KeyValuePair<EnemyData, int> enemyCount in enemyCounts)
        {
            Prewarm(enemyCount.Key, enemyCount.Value);
        }
    }

    public void Prewarm(EnemyData enemyData, int count)
    {
        if (!IsValidEnemyData(enemyData) || count <= 0)
        {
            return;
        }

        Queue<GameObject> pool = GetOrCreateQueue(enemyData);

        for (int i = pool.Count; i < count; i++)
        {
            GameObject enemy = CreateEnemy(enemyData);
            pool.Enqueue(enemy);
            inactiveEnemies.Add(enemy);
        }
    }

    public GameObject GetEnemy()
    {
        return GetEnemy(defaultEnemyData);
    }

    public GameObject GetEnemy(EnemyData enemyData)
    {
        if (!IsValidEnemyData(enemyData))
        {
            Debug.LogWarning("EnemyPool could not provide an enemy because EnemyData or Prefab is missing.");
            return null;
        }

        Queue<GameObject> pool = GetOrCreateQueue(enemyData);

        // The pool is prewarmed from LevelDataConfig, but this fallback keeps the game from breaking if data changes.
        GameObject enemy = pool.Count > 0 ? pool.Dequeue() : CreateEnemy(enemyData);

        inactiveEnemies.Remove(enemy);
        enemy.SetActive(true);

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.Initialize(enemyData, this);
        }

        return enemy;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null || inactiveEnemies.Contains(enemy))
        {
            return;
        }

        if (!enemyDataByInstance.TryGetValue(enemy, out EnemyData enemyData) || enemyData == null)
        {
            enemy.SetActive(false);
            enemy.transform.SetParent(poolRoot);
            return;
        }

        enemy.SetActive(false);
        enemy.transform.SetParent(poolRoot);
        enemy.transform.localPosition = Vector3.zero;

        GetOrCreateQueue(enemyData).Enqueue(enemy);
        inactiveEnemies.Add(enemy);
    }

    private GameObject CreateEnemy(EnemyData enemyData)
    {
        GameObject enemy = Instantiate(enemyData.Prefab, poolRoot);
        enemy.name = enemyData.Prefab.name;
        enemy.SetActive(false);

        enemyDataByInstance[enemy] = enemyData;

        // Enemy prefabs can include EnemyMovement manually, but adding it here keeps pooled enemies usable by default.
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement == null)
        {
            movement = enemy.AddComponent<EnemyMovement>();
        }

        movement.Initialize(enemyData, this);
        return enemy;
    }

    private Queue<GameObject> GetOrCreateQueue(EnemyData enemyData)
    {
        if (!availableEnemies.TryGetValue(enemyData, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            availableEnemies[enemyData] = pool;
        }

        return pool;
    }

    private bool IsValidEnemyData(EnemyData enemyData)
    {
        return enemyData != null && enemyData.Prefab != null;
    }
}
