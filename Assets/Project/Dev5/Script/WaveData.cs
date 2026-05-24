using System;
using System.Collections.Generic;
using UnityEngine;

// 1. Thông tin của từng con quái lẻ
[Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab; // Loại quái (Kéo Prefab vào)
    public float spawnDelay;       // Xuất hiện sau con trước đó bao nhiêu giây
    public int laneIndex;          // Xuất hiện ở hàng số mấy (0 đến 4)
}

// 2. Cấu trúc của một Wave (Đợt quái)
[Serializable]
public class Wave
{
    public string waveName; // Tên hiển thị (Ví dụ: Wave 1, Đại nạn)
    public List<EnemySpawnInfo> enemiesToSpawn; // Danh sách các con quái trong wave này
}

// 3. Tạo file chứa toàn bộ các Wave của một màn chơi
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Wave System/Level Data")]
public class LevelData : ScriptableObject
{
    public List<Wave> allWaves; // Danh sách các wave từ đầu đến cuối trận
}