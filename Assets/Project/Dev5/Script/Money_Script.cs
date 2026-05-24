using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDestroyer : MonoBehaviour
{
    public int moneyValue = 10;

    private Rigidbody2D rb;

    private void Start()
    {
        // Automatically grab the Rigidbody2D component attached to this coin
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError($"ClickDestroyer on {gameObject.name} requires a Rigidbody2D component to handle gravity!");
        }
    }

    private void Update()
    {
        // 1. Check if the left mouse button (or screen tap) was pressed this frame
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // 2. Convert mouse screen position to a world position point
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));

            // 3. Cast a tiny mathematical point/ray to see if it hits this object's collider
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            // 4. If the thing we clicked is THIS object, destroy it!
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OnObjectClicked();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we overlapped with has the "ground_money" tag
        if (other.CompareTag("ground_money"))
        {
            if (rb != null)
            {
                rb.gravityScale = 0f;  // Turn off gravity so it stops accelerating downward
                rb.linearVelocity = Vector2.zero; // Completely freeze its current falling speed
            }
        }
    }

    private void OnObjectClicked()
    {
        if (GameManager.Instance != null)
        {
            // 1. Add money to the manager's current tracking pool
            GameManager.Instance.currentMoney += moneyValue;

            // 2. Force the manager to refresh the UI Text Mesh text box instantly
            GameManager.Instance.displayMoney();
        }
        else
        {
            Debug.LogError("ClickDestroyer couldn't find a GameManager in the scene!");
        }

        // Destroy this GameObject instantly
        Destroy(gameObject);
    }
}