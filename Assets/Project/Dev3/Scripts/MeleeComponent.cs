using System.Collections;
using UnityEngine;

namespace HVTThanh.Combat
{
    /// <summary>
    /// Gắn vào Prefab Quái vật. Khi chạm vào Cây (Tag "Plant"), Quái dừng lại
    /// và liên tục cắn Cây theo chu kỳ. Khi Cây chết, Quái tiếp tục di chuyển.
    /// Yêu cầu: Rigidbody2D + Collider2D (Is Trigger) trên Quái.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MeleeComponent : MonoBehaviour
    {
        // ─── Cấu hình trên Inspector ───────────────────────────────────────────
        [Header("Chỉ số chiến đấu")]
        [Tooltip("Sát thương mỗi đòn cắn lên Cây")]
        [SerializeField] private float attackDamage = 10f;

        [Tooltip("Số giây giữa mỗi đòn cắn")]
        [SerializeField] private float attackRate = 1f;

        [Header("Di chuyển")]
        [Tooltip("Tốc độ di chuyển sang trái")]
        [SerializeField] private float moveSpeed = 2f;

        // ─── Trạng thái nội bộ ─────────────────────────────────────────────────
        private bool isAttacking = false;       // Đang cắn Cây hay không
        private Health currentTargetHealth;     // Health của Cây đang bị cắn
        private Coroutine attackCoroutine;
        private Rigidbody2D rb;

        // ───────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Nhân hệ số từ ModifierManager của Dev 4 (mở comment khi gộp)
            // attackDamage *= ModifierManager.Instance?.DamageMultiplier ?? 1f;
        }

        // ───────────────────────────────────────────────────────────────────────
        private void Update()
        {
            // Chỉ di chuyển khi không đang cắn Cây
            if (!isAttacking)
            {
                // Di chuyển sang trái theo trục X
                transform.Translate(-moveSpeed * Time.deltaTime, 0f, 0f);
            }
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Khi Collider (IsTrigger) của Quái chạm vào Cây có Tag "Plant".
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Chỉ xử lý khi chưa đang cắn và chạm đúng Tag "Plant"
            if (isAttacking) return;
            if (!other.CompareTag("Plant")) return;

            // Tìm Health trên Cây
            Health plantHealth = other.GetComponent<Health>();
            if (plantHealth == null)
            {
                Debug.LogWarning($"[MeleeComponent] {other.name} có Tag 'Plant' nhưng thiếu component Health!");
                return;
            }

            // Bắt đầu tấn công
            currentTargetHealth = plantHealth;
            isAttacking = true;

            // Lắng nghe event khi Cây chết để tiếp tục di chuyển
            currentTargetHealth.OnDeath += OnPlantDied;

            // Bắt đầu Coroutine cắn
            attackCoroutine = StartCoroutine(AttackRoutine());
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Coroutine liên tục gọi TakeDamage lên Cây theo chu kỳ attackRate.
        /// </summary>
        private IEnumerator AttackRoutine()
        {
            while (isAttacking && currentTargetHealth != null)
            {
                currentTargetHealth.TakeDamage(attackDamage);

                yield return new WaitForSeconds(attackRate);
            }
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Được gọi khi Cây chết — Quái tiếp tục di chuyển sang trái.
        /// </summary>
        private void OnPlantDied()
        {
            // Hủy đăng ký event tránh gọi lại
            if (currentTargetHealth != null)
                currentTargetHealth.OnDeath -= OnPlantDied;

            // Dừng Coroutine cắn
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            // Reset trạng thái — Quái tiếp tục đi
            isAttacking = false;
            currentTargetHealth = null;
        }

        // ───────────────────────────────────────────────────────────────────────
        private void OnDestroy()
        {
            // Dọn dẹp event khi Quái bị hủy tránh memory leak
            if (currentTargetHealth != null)
                currentTargetHealth.OnDeath -= OnPlantDied;
        }
    }
}