using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Data Configuration")]
    [SerializeField] private LevelData levelData; // Bỏ file LevelData đã tạo ở Bước 2 vào đây
    [SerializeField] private Transform[] spawnPoints; // Mảng chứa các vị trí xuất hiện (5 hàng ở rìa phải)

    [Header("Runtime State")]
    private int currentWaveIndex = 0;
    private int activeEnemiesCount = 0; // Số lượng quái đang còn sống trên sân
    private bool isSpawningWave = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Thử thách: Bắt đầu Wave 1 sau 3 giây chuẩn bị
        StartCoroutine(StartNextWaveRoutine());
    }

    // Coroutine điều khiển vòng lặp của từng Wave
    private IEnumerator StartNextWaveRoutine()
    {
        if (currentWaveIndex >= levelData.allWaves.Count)
        {
            Debug.Log("CHÚC MỪNG! Bạn đã vượt qua tất cả các Wave. THẮNG TRẬN!");
            yield break;
        }

        isSpawningWave = true;
        Wave currentWave = levelData.allWaves[currentWaveIndex];
        Debug.Log("Bắt đầu: " + currentWave.waveName);

        // Chạy qua từng con quái trong danh sách của Wave đó để Spawn
        foreach (EnemySpawnInfo enemyInfo in currentWave.enemiesToSpawn)
        {
            yield return new WaitForSeconds(enemyInfo.spawnDelay);

            // Xác định vị trí xuất hiện dựa trên số hàng (Lane Index)
            Transform spawnPoint = spawnPoints[enemyInfo.laneIndex];

            // Tiến hành sinh ra con quái
            GameObject spawnedEnemy = Instantiate(enemyInfo.enemyPrefab, spawnPoint.position, Quaternion.identity);

            activeEnemiesCount++;
        }

        isSpawningWave = false;
    }

    // HÀM QUAN TRỌNG: Gọi hàm này mỗi khi có 1 con quái bị tiêu diệt hoặc lọt lưới
    public void OnEnemyDestroyed()
    {
        activeEnemiesCount--;

        // Nếu tất cả quái trong Wave đã chết VÀ đã gọi hết quái ra sân
        if (activeEnemiesCount <= 0 && !isSpawningWave)
        {
            Debug.Log("Wave đã sạch bóng quái!");
            currentWaveIndex++;

        }
    }
    public void TriggerNextWave()
    {
        if (!isSpawningWave && activeEnemiesCount <= 0)
        {
            StartCoroutine(StartNextWaveRoutine());
        }
    }
}