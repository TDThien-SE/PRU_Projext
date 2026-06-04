using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    public static ModifierManager Instance { get; private set; }

    public float DamageMultiplier { get; private set; } = 1f;
    public float FireRateMultiplier { get; private set; } = 1f;
    public float HealthMultiplier { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyAugment(AugmentData data)
    {
        if (data == null) return;

        switch (data.type)
        {
            case AugmentType.DamageMultiplier:
                DamageMultiplier += data.value;
                break;

            case AugmentType.FireRateMultiplier:
                FireRateMultiplier += data.value;
                break;

            case AugmentType.HealthMultiplier:
                HealthMultiplier += data.value;
                break;

            case AugmentType.HealAmount:
                break;
        }
    }

    public void ResetModifiers()
    {
        DamageMultiplier = 1f;
        FireRateMultiplier = 1f;
        HealthMultiplier = 1f;
    }
}
