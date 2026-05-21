using Unity.VisualScripting;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem vật thể vừa đâm vào vùng này có phải là Quái vật không
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Quái đã lọt lưới!");

            // 1. Gọi GameManager để trừ 1 máu của người chơi
            GameManager.Instance.takeDamage(1);

            // 3. Tạm thời tự hủy vật thể đó trên Scene của bạn để tránh nó bay vô tận
            Destroy(collision.gameObject);
        }
    }
}