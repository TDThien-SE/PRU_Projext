using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static UiManager Instance { get; private set; }

    public int health;
    public int maxHealth;

    public Sprite emptyHealth;
    public Sprite fullHealth;

    public Image[] hearts;

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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            health--;
        }
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
}
