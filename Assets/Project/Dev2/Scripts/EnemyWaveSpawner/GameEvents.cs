using System;
using UnityEngine;

public static class GameEvents
{
    // Other modules should subscribe to these events instead of taking direct dependencies on the spawner or enemies.
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<EnemyData> OnEnemyKilled;
    public static event Action OnEnemyReachedBase;

    public static void RaiseEnemySpawned(GameObject enemy)
    {
        OnEnemySpawned?.Invoke(enemy);
    }

    public static void RaiseEnemyKilled(EnemyData data)
    {
        OnEnemyKilled?.Invoke(data);
    }

    public static void RaiseEnemyReachedBase()
    {
        OnEnemyReachedBase?.Invoke();
    }
}
