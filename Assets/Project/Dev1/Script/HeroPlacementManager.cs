using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HeroPlacementManager : MonoBehaviour
{
    public static event System.Action<int, int, GameObject, HeroType> OnHeroPlaced;

    [Header("Grid Settings")]
    [Min(1)]
    public int rows = 5;
    [Min(1)]
    public int columns = 9;
    [Min(0.1f)]
    public float cellSize = 1f;
    public Vector2 gridOrigin;

    [Header("Hero Settings")]
    public HeroType selectedHeroType;
    public HeroPrefabEntry[] heroPrefabs;

    private GridCell[,] cells;

    private void Start()
    {
        BuildGrid();
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private void BuildGrid()
    {
        cells = new GridCell[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector2 center = GetCellCenterWorldPosition(row, column);
                cells[row, column] = new GridCell(row, column, center);
            }
        }

        Debug.Log($"Hero grid built: {rows} rows x {columns} columns.");
    }

    public Vector2 GetCellCenterWorldPosition(int row, int column)
    {
        float x = gridOrigin.x + (column + 0.5f) * cellSize;
        float y = gridOrigin.y + (row + 0.5f) * cellSize;
        return new Vector2(x, y);
    }

    public bool TryGetCellFromWorldPosition(Vector2 worldPosition, out int row, out int column)
    {
        Vector2 localPosition = worldPosition - gridOrigin;

        column = Mathf.FloorToInt(localPosition.x / cellSize);
        row = Mathf.FloorToInt(localPosition.y / cellSize);

        return IsValidCell(row, column);
    }

    public bool IsValidCell(int row, int column)
    {
        return row >= 0 && row < rows && column >= 0 && column < columns;
    }

    private void HandleMouseClick()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (IsPointerOverUI())
        {
            Debug.Log("Pointer is over UI. Placement ignored.");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogWarning("HeroPlacementManager needs a camera tagged MainCamera.");
            return;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, -Camera.main.transform.position.z)
        );

        if (TryGetCellFromWorldPosition(mouseWorldPosition, out int row, out int column))
        {
            Vector2 center = GetCellCenterWorldPosition(row, column);
            Debug.Log($"Clicked grid cell: row {row}, column {column}, center {center}");
            TryPlaceHero(row, column);
        }
        else
        {
            Debug.Log("Clicked outside hero placement grid.");
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void TryPlaceHero(int row, int column)
    {
        if (!IsValidCell(row, column))
        {
            Debug.Log("Cannot place Hero: invalid cell.");
            return;
        }

        GridCell cell = cells[row, column];

        if (cell.isOccupied)
        {
            Debug.Log($"Cannot place Hero: cell row {row}, column {column} is already occupied.");
            return;
        }

        GameObject heroPrefab = GetHeroPrefab(selectedHeroType);

        if (heroPrefab == null)
        {
            Debug.LogWarning($"Cannot place Hero: no prefab assigned for {selectedHeroType}.");
            return;
        }

        int heroCost = GetHeroCost(selectedHeroType);

        if (!TryPayHeroCost(heroCost))
        {
            Debug.Log($"Cannot place Hero: not enough Hao Khi for {selectedHeroType}.");
            return;
        }

        GameObject placedHero = Instantiate(heroPrefab, cell.centerWorldPosition, Quaternion.identity);
        cell.placedHero = placedHero;
        cell.isOccupied = true;

        OnHeroPlaced?.Invoke(row, column, placedHero, selectedHeroType);

        Debug.Log($"Placed {selectedHeroType} at row {row}, column {column}.");
    }

    private GameObject GetHeroPrefab(HeroType type)
    {
        if (heroPrefabs == null)
        {
            return null;
        }

        foreach (HeroPrefabEntry entry in heroPrefabs)
        {
            if (entry != null && entry.type == type)
            {
                return entry.prefab;
            }
        }

        return null;
    }

    private int GetHeroCost(HeroType type)
    {
        if (heroPrefabs == null)
        {
            return 0;
        }

        foreach (HeroPrefabEntry entry in heroPrefabs)
        {
            if (entry != null && entry.type == type)
            {
                return entry.cost;
            }
        }

        return 0;
    }

    private bool TryPayHeroCost(int cost)
    {
        return true;
    }

    private void OnDrawGizmos()
    {
        if (rows <= 0 || columns <= 0 || cellSize <= 0f)
        {
            return;
        }

        Gizmos.color = Color.green;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector2 center = GetCellCenterWorldPosition(row, column);
                Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, 0f));
            }
        }
    }
}
