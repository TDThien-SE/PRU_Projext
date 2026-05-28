using System;
using UnityEngine;

namespace HVTThanh.Combat
{
    /// <summary>
    /// Script máu quốc dân — gắn vào BẤT KỲ thực thể nào có máu (Cây hoặc Quái).
    /// Cung cấp hàm TakeDamage() và phát C# Event OnDeath khi máu về 0.
    /// </summary>
    public class Health : MonoBehaviour
    {
        // ─── Cấu hình trên Inspector ───────────────────────────────────────────
        [Header("Chỉ số máu")]
        [Tooltip("Lượng máu tối đa của thực thể này")]
        [SerializeField] private float maxHP = 100f;

        // ─── C# Events ─────────────────────────────────────────────────────────
        /// <summary>Phát ra khi thực thể chết — Dev 2 và Dev 5 có thể lắng nghe.</summary>
        public event Action OnDeath;

        /// <summary>Phát ra mỗi khi máu thay đổi (currentHP, maxHP) — dùng cho UI thanh máu.</summary>
        public event Action<float, float> OnHealthChanged;

        // ─── Trạng thái nội bộ ─────────────────────────────────────────────────
        private float currentHP;
        private bool isDead = false; // Cờ chống kích hoạt OnDeath nhiều lần

        // ─── Property công khai ────────────────────────────────────────────────
        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public bool IsDead => isDead;

        // ───────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            // Nhân với hệ số buff từ ModifierManager của Dev 4 (nếu đã có)
            // float hpMultiplier = ModifierManager.Instance?.HPMultiplier ?? 1f;
            currentHP = maxHP; // * hpMultiplier; ← mở comment khi gộp với Dev 4
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Gọi hàm này để gây sát thương lên thực thể.
        /// Được Projectile.cs và MeleeComponent.cs gọi tới.
        /// </summary>
        /// <param name="damage">Lượng sát thương (luôn dương).</param>
        public void TakeDamage(float damage)
        {
            // Bỏ qua nếu đã chết rồi
            if (isDead) return;

            // Trừ máu, đảm bảo không xuống dưới 0
            currentHP = Mathf.Max(0f, currentHP - damage);

            // Thông báo cho UI cập nhật thanh máu
            OnHealthChanged?.Invoke(currentHP, maxHP);

            // Kiểm tra chết
            if (currentHP <= 0f)
            {
                Die();
            }
        }

        // ───────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Hồi máu (ví dụ: dùng cho skill của anh hùng sau này).
        /// </summary>
        public void Heal(float amount)
        {
            if (isDead) return;

            currentHP = Mathf.Min(maxHP, currentHP + amount);
            OnHealthChanged?.Invoke(currentHP, maxHP);
        }

        // ───────────────────────────────────────────────────────────────────────
        private void Die()
        {
            isDead = true;

            // Phát event cho Dev 2 (OnEnemyKilled) và Dev 5 (cộng tiền, cộng điểm)
            OnDeath?.Invoke();

            // Tự hủy GameObject sau 1 frame nhỏ để các listener kịp xử lý
            Destroy(gameObject, 0.05f);
        }
    }
}
