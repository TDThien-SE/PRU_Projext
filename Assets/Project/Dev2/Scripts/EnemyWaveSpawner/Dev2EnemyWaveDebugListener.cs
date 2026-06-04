using System.Collections.Generic;
using UnityEngine;

public class Dev2EnemyWaveDebugListener : MonoBehaviour
{
    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    private void OnEnable()
    {
        GameEvents.OnEnemySpawned += HandleEnemySpawned;
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.OnEnemyReachedBase += HandleEnemyReachedBase;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemySpawned -= HandleEnemySpawned;
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnEnemyReachedBase -= HandleEnemyReachedBase;
    }

    private void HandleEnemySpawned(GameObject enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }

        Debug.Log($"[Dev2 Test] Enemy spawned: {enemy.name}");
    }

    private void HandleEnemyKilled(EnemyData data)
    {
        Debug.Log($"[Dev2 Test] Enemy killed: {(data != null ? data.EnemyName : "Unknown")}");
    }

    private void HandleEnemyReachedBase()
    {
        Debug.Log("[Dev2 Test] Enemy reached base.");
    }

    [ContextMenu("Kill First Active Enemy")]
    private void KillFirstActiveEnemy()
    {
        activeEnemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);

        if (activeEnemies.Count == 0)
        {
            Debug.Log("[Dev2 Test] No active enemy to kill.");
            return;
        }

        EnemyMovement enemyMovement = activeEnemies[0].GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.Die();
        }
    }
}
