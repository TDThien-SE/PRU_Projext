using UnityEngine;

namespace HVTThanh.Combat
{
    /// <summary>
    /// Gắn vào Prefab đạn. Điều khiển đạn bay thẳng sang phải,
    /// phát hiện va chạm với Quái (Tag "Enemy") và gọi TakeDamage.
    /// Yêu cầu: Rigidbody2D (Kinematic) + Collider2D (Is Trigger) trên cùng GameObject.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        // ─── Cấu hình trên Inspector ───────────────────────────────────────────
        [Header("Chỉ số đạn")]
        [Tooltip("Tốc độ bay của đạn (đơn vị/giây)")]
        [SerializeField] private float speed = 8f;

        [Tooltip("Sát thương gây ra khi trúng Quái")]
        [SerializeField] private float damage = 20f;

        [Tooltip("Khoảng cách tối đa đạn bay trước khi tự hủy (tránh rò rỉ bộ nhớ)")]
        [SerializeField] private float maxTravelDistance = 20f;

        // ─── Trạng thái nội bộ ─────────────────────────────────────────────────
        private Vector2 startPosition;
        private bool hasHit = false; // Cờ chống va chạm nhiều lần cùng lúc

        // ───────────────────────────────────────────────────────────────────────
        private void Start()
        {
            startPosition = transform.position;

            // Nhân hệ số damage từ ModifierManager của Dev 4 (mở comment khi gộp)
            // damage *= ModifierManager.Instance?.DamageMultiplier ?? 1f;
        }

        // ───────────────────────────────────────────────────────────────────────
        private void Update()
        {
            // Bay thẳng sang phải theo trục X
            transform.Translate(speed * Time.deltaTime, 0f, 0f);

            // Tự hủy nếu bay quá xa (phòng trường hợp không trúng ai)
            float distanceTraveled = Vector2.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Unity gọi hàm này khi Collider (IsTrigger) của đạn chạm vào Collider khác.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Tránh xử lý nhiều lần nếu trigger kích hoạt liên tiếp
            if (hasHit) return;

            // Chỉ xử lý khi chạm vào vật thể có Tag "Enemy"
            if (!other.CompareTag("Enemy")) return;

            hasHit = true;

            // Tìm component Health trên Quái và gây sát thương
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                // Cảnh báo nếu Quái của Dev 2 chưa gắn Health
                Debug.LogWarning($"[Projectile] {other.name} có Tag 'Enemy' nhưng thiếu component Health!");
            }

            // Đạn tự hủy sau khi trúng
            Destroy(gameObject);
        }
    }
}