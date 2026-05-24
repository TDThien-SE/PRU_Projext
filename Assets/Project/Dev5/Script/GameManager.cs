using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public Sprite emptyHealth;
    public Sprite fullHealth;
    public Image[] hearts;

    [Header("Money Settings")]
    public int currentMoney;
    public TextMeshProUGUI currentMoneyText;
    public GameObject moneyPrefab;

    // Changed to float for precise sub-second cooldown tracking
    public float timeBetweenSpawn = 3f;
    private float spawnTimer; // Keeps track of the ticking clock

    [Header("Spawn Boundaries")]
    public Vector3 startPos;
    public Vector3 endPos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateHealthUI();
        displayMoney();

    }

    void Update()
    {
        displayMoney();
        UpdateHealthUI();

        // Handles the ticking clock and checking the cooldown
        HandleMoneySpawning();
    }

    private void HandleMoneySpawning()
    {
        // Time.deltaTime is the time in seconds it took to complete the last frame
        spawnTimer += Time.deltaTime;

        // If the clock has reached or passed our cooldown time...
        if (spawnTimer >= timeBetweenSpawn)
        {
            spawnMoney();

            // Reset the clock back to 0 so it has to wait another 3 seconds
            spawnTimer = 0f;
        }
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }
        UpdateHealthUI();
    }

    public void displayMoney()
    {
        if (currentMoneyText != null)
        {
            currentMoneyText.text = currentMoney.ToString();
        }
    }

    public void spawnMoney()
    {
        if (moneyPrefab == null)
        {
            Debug.LogWarning("Please assign a Money Prefab in the GameManager Inspector!");
            return;
        }

        float randomInterpolation = Random.Range(0f, 1f);
        Vector3 randomSpawnPosition = Vector3.Lerp(startPos, endPos, randomInterpolation);
        Instantiate(moneyPrefab, randomSpawnPosition, Quaternion.identity);
    }

    private void UpdateHealthUI()
    {
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

            hearts[i].enabled = (i < maxHealth);
        }
    }
}