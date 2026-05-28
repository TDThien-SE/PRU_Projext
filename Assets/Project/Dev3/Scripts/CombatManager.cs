using System.Collections.Generic;
using UnityEngine;

namespace HVTThanh.Combat
{
    /// <summary>
    /// Bộ não quản lý chiến đấu theo Hàng (Row).
    /// Lắng nghe event từ Dev 1 (OnPlantPlaced) và Dev 2 (OnEnemySpawned)
    /// để đưa Cây/Quái vào đúng hàng, từ đó tối ưu lệnh bắn.
    /// Đặt script này lên một GameObject rỗng tên "CombatManager" trong Scene.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        // ─── Singleton ─────────────────────────────────────────────────────────
        public static CombatManager Instance { get; private set; }

        // ─── Dữ liệu theo hàng ────────────────────────────────────────────────
        // Key = chỉ số hàng (0, 1, 2...), Value = danh sách GameObject trong hàng đó
        private Dictionary<int, List<GameObject>> plantRows = new();
        private Dictionary<int, List<GameObject>> enemyRows = new();

        // ───────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            // Thiết lập Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // ───────────────────────────────────────────────────────────────────────
        private void Start()
        {
            // Đăng ký lắng nghe event từ Dev 1 và Dev 2
            // Mở comment khi gộp project với Dev 1 và Dev 2
            // PlacementManager.OnPlantPlaced  += HandlePlantPlaced;
            // EnemySpawner.OnEnemySpawned     += HandleEnemySpawned;
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Dev 1 gọi khi Cây được đặt lên bản đồ.
        /// </summary>
        public void HandlePlantPlaced(int row, int col, GameObject plant)
        {
            // Tạo danh sách cho hàng này nếu chưa có
            if (!plantRows.ContainsKey(row))
                plantRows[row] = new List<GameObject>();

            plantRows[row].Add(plant);

            // Lắng nghe khi Cây chết để xóa khỏi danh sách
            Health plantHealth = plant.GetComponent<Health>();
            if (plantHealth != null)
                plantHealth.OnDeath += () => HandlePlantDied(row, plant);

            // Cập nhật trạng thái bắn cho hàng này
            UpdateRowShootState(row);
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Dev 2 gọi khi Quái xuất hiện trên bản đồ.
        /// </summary>
        public void HandleEnemySpawned(int row, GameObject enemy)
        {
            if (!enemyRows.ContainsKey(row))
                enemyRows[row] = new List<GameObject>();

            enemyRows[row].Add(enemy);

            // Lắng nghe khi Quái chết để xóa khỏi danh sách
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
                enemyHealth.OnDeath += () => HandleEnemyDied(row, enemy);

            // Cập nhật trạng thái bắn cho hàng này
            UpdateRowShootState(row);
        }

        // ───────────────────────────────────────────────────────────────────────
        private void HandlePlantDied(int row, GameObject plant)
        {
            if (plantRows.ContainsKey(row))
            {
                plantRows[row].Remove(plant);

                // Xóa hàng nếu không còn Cây nào
                if (plantRows[row].Count == 0)
                    plantRows.Remove(row);
            }

            UpdateRowShootState(row);
        }

        // ───────────────────────────────────────────────────────────────────────
        private void HandleEnemyDied(int row, GameObject enemy)
        {
            if (enemyRows.ContainsKey(row))
            {
                enemyRows[row].Remove(enemy);

                // Xóa hàng nếu không còn Quái nào
                if (enemyRows[row].Count == 0)
                    enemyRows.Remove(row);
            }

            UpdateRowShootState(row);
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Cập nhật lệnh bắn cho tất cả Cây trong một hàng.
        /// Cây chỉ bắn khi hàng đó có Quái.
        /// </summary>
        private void UpdateRowShootState(int row)
        {
            // Kiểm tra hàng có Quái không
            bool hasEnemy = enemyRows.ContainsKey(row) && enemyRows[row].Count > 0;

            // Nếu hàng không có Cây thì bỏ qua
            if (!plantRows.ContainsKey(row)) return;

            // Báo cho tất cả Cây trong hàng biết
            foreach (GameObject plant in plantRows[row])
            {
                if (plant == null) continue;

                ShooterComponent shooter = plant.GetComponent<ShooterComponent>();
                if (shooter != null)
                    shooter.SetEnemyInRow(hasEnemy);
            }
        }

        // ───────────────────────────────────────────────────────────────────────
        private void OnDestroy()
        {
            // Hủy đăng ký event khi CombatManager bị hủy
            // PlacementManager.OnPlantPlaced  -= HandlePlantPlaced;
            // EnemySpawner.OnEnemySpawned     -= HandleEnemySpawned;
        }
    }
}