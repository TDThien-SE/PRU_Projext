using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Path End")]
    [SerializeField] private float baseXPosition = -8f;

    private EnemyData enemyData;
    private EnemyPool enemyPool;
    private bool hasReturnedToPool;

    public EnemyData Data => enemyData;

    public void Initialize(EnemyData data, EnemyPool pool)
    {
        enemyData = data;
        enemyPool = pool;
        hasReturnedToPool = false;
    }

    private void Update()
    {
        if (enemyData == null || hasReturnedToPool)
        {
            return;
        }

        transform.Translate(Vector3.left * enemyData.MoveSpeed * Time.deltaTime, Space.World);

        // The enemy path is strictly right-to-left; reaching this X value means the base was breached.
        if (transform.position.x <= baseXPosition)
        {
            ReachBase();
        }
    }

    public void Die()
    {
        if (hasReturnedToPool)
        {
            return;
        }

        // Health and damage are owned by another module; this method only broadcasts and returns to the pool.
        GameEvents.RaiseEnemyKilled(enemyData);
        ReturnToPool();
    }

    private void ReachBase()
    {
        if (hasReturnedToPool)
        {
            return;
        }

        GameEvents.RaiseEnemyReachedBase();
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        hasReturnedToPool = true;

        if (enemyPool != null)
        {
            enemyPool.ReturnEnemy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
