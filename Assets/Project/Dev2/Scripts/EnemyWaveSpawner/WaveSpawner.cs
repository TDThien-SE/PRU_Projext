using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDataConfig levelDataConfig;
    [SerializeField] private EnemyPool enemyPool;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool cycleSpawnPoints = true;

    private Coroutine waveRoutine;
    private int nextSpawnPointIndex;

    private void Start()
    {
        if (enemyPool == null)
        {
            enemyPool = FindFirstObjectByType<EnemyPool>();
        }

        if (enemyPool != null)
        {
            enemyPool.Prewarm(levelDataConfig);
        }

        waveRoutine = StartCoroutine(SpawnAllWavesRoutine());
    }

    private IEnumerator SpawnAllWavesRoutine()
    {
        if (levelDataConfig == null)
        {
            Debug.LogWarning("WaveSpawner has no LevelDataConfig assigned.");
            yield break;
        }

        for (int i = 0; i < levelDataConfig.Waves.Count; i++)
        {
            WaveConfig wave = levelDataConfig.Waves[i];
            if (wave == null)
            {
                continue;
            }

            yield return StartCoroutine(SpawnWaveRoutine(wave));

            bool hasNextWave = i < levelDataConfig.Waves.Count - 1;
            if (hasNextWave && wave.DelayBeforeNextWave > 0f)
            {
                yield return new WaitForSeconds(wave.DelayBeforeNextWave);
            }
        }

        waveRoutine = null;
    }

    private IEnumerator SpawnWaveRoutine(WaveConfig wave)
    {
        foreach (EnemyData enemyData in wave.EnemiesToSpawn)
        {
            SpawnEnemy(enemyData);

            if (wave.SpawnInterval > 0f)
            {
                yield return new WaitForSeconds(wave.SpawnInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        if (enemyPool == null)
        {
            Debug.LogWarning("WaveSpawner cannot spawn because EnemyPool is missing.");
            return;
        }

        GameObject enemy = enemyPool.GetEnemy(enemyData);
        if (enemy == null)
        {
            return;
        }

        enemy.transform.position = GetSpawnPosition();
        enemy.transform.rotation = Quaternion.identity;

        // UI, scoring, audio, and other systems can react without being referenced here.
        GameEvents.RaiseEnemySpawned(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return transform.position;
        }

        if (!cycleSpawnPoints)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex] != null ? spawnPoints[randomIndex].position : transform.position;
        }

        Transform spawnPoint = spawnPoints[nextSpawnPointIndex % spawnPoints.Length];
        nextSpawnPointIndex++;

        return spawnPoint != null ? spawnPoint.position : transform.position;
    }
}
