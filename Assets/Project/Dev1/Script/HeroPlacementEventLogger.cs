using UnityEngine;

public class HeroPlacementEventLogger : MonoBehaviour
{
    private void OnEnable()
    {
        HeroPlacementManager.OnHeroPlaced += HandleHeroPlaced;
    }

    private void OnDisable()
    {
        HeroPlacementManager.OnHeroPlaced -= HandleHeroPlaced;
    }

    private void HandleHeroPlaced(int row, int column, GameObject hero, HeroType heroType)
    {
        Debug.Log($"[Placement Event] Hero {heroType} placed at row {row}, column {column}.");
    }
}
