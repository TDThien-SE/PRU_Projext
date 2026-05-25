using System.Collections;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    private void Start()
    {
        // Start the timer as soon as the object is created/spawned
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        // Wait for exactly 3 seconds
        yield return new WaitForSeconds(3f);
        WaveManager.Instance.OnEnemyDestroyed(); // Thông báo cho WaveManager rằng một con quái đã bị tiêu diệt
        gameObject.SetActive(false);
    }
}