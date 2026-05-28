using System.Collections;
using UnityEngine;

namespace HVTThanh.Combat
{
    /// <summary>
    /// Gắn vào Prefab Cây bắn súng. Tự động sinh ra đạn theo chu kỳ Fire Rate.
    /// Yêu cầu: kéo Prefab đạn vào ô bulletPrefab trên Inspector.
    /// </summary>
    public class ShooterComponent : MonoBehaviour
    {
        // ─── Cấu hình trên Inspector ───────────────────────────────────────────
        [Header("Cấu hình bắn")]
        [Tooltip("Prefab viên đạn — kéo Prefab_Bullet vào đây")]
        [SerializeField] private GameObject bulletPrefab;

        [Tooltip("Số giây giữa mỗi lần bắn")]
        [SerializeField] private float fireRate = 1.5f;

        [Tooltip("Vị trí đầu nòng súng — kéo một Transform con vào đây")]
        [SerializeField] private Transform firePoint;

        [Header("Tối ưu — chỉ bắn khi có quái")]
        [Tooltip("Bật = chỉ bắn khi CombatManager báo hàng này có quái")]
        [SerializeField] private bool useRowCheck = false;

        // ─── Trạng thái nội bộ ─────────────────────────────────────────────────
        private bool hasEnemyInRow = true; // Mặc định true để test độc lập
        private Coroutine shootCoroutine;

        // ───────────────────────────────────────────────────────────────────────
        private void Start()
        {
            // Nhân hệ số fire rate từ ModifierManager của Dev 4 (mở comment khi gộp)
            // fireRate /= ModifierManager.Instance?.FireRateMultiplier ?? 1f;

            // Nếu không gán firePoint thì dùng vị trí của chính Cây
            if (firePoint == null)
            {
                firePoint = transform;
                Debug.LogWarning($"[ShooterComponent] {gameObject.name}: chưa gán FirePoint, dùng vị trí Cây.");
            }

            // Bắt đầu vòng lặp bắn
            shootCoroutine = StartCoroutine(ShootRoutine());
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Coroutine vòng lặp bắn — chạy mãi cho đến khi Cây bị hủy.
        /// </summary>
        private IEnumerator ShootRoutine()
        {
            // Chờ một chút trước khi bắn phát đầu tiên
            yield return new WaitForSeconds(fireRate);

            while (true)
            {
                // Chỉ bắn nếu hàng có quái (hoặc đang test nên bỏ qua check)
                if (!useRowCheck || hasEnemyInRow)
                {
                    Shoot();
                }

                // Chờ đủ fire rate rồi bắn tiếp
                yield return new WaitForSeconds(fireRate);
            }
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Sinh ra một viên đạn tại vị trí firePoint.
        /// </summary>
        private void Shoot()
        {
            if (bulletPrefab == null)
            {
                Debug.LogError($"[ShooterComponent] {gameObject.name}: chưa gán bulletPrefab!");
                return;
            }

            // Instantiate đạn tại vị trí đầu nòng, không xoay
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// CombatManager gọi hàm này để báo hàng có/không có quái.
        /// Giai đoạn sau khi gộp với Dev 1 và Dev 2.
        /// </summary>
        public void SetEnemyInRow(bool hasEnemy)
        {
            hasEnemyInRow = hasEnemy;
        }

        // ───────────────────────────────────────────────────────────────────────
        private void OnDestroy()
        {
            // Dừng Coroutine khi Cây bị hủy tránh lỗi
            if (shootCoroutine != null)
                StopCoroutine(shootCoroutine);
        }
    }
}