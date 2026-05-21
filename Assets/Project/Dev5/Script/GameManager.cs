using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance { get; private set; }

    public int health;
    public int maxHealth;

    public Sprite emptyHealth;
    public Sprite fullHealth;

    public Image[] hearts;

    public GameObject dummyPrefab;
    public Camera mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        clickMinusHp();
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHealth;
            }
            else
            {
                hearts[i].sprite = emptyHealth;
            }
            if (i < maxHealth)
            {
                hearts[i].enabled = (true);
            }
            else
            {
                hearts[i].enabled = (false);
            }
        }
    }

    void clickMinusHp()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (dummyPrefab != null && mainCamera != null)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector3 spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
                spawnPosition.z = 0f;

                Instantiate(dummyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }
    }
}
