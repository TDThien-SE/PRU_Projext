using UnityEngine;
using UnityEngine.InputSystem;

public class HeroPlacementManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Min(1)]
    public int rows = 5;
    [Min(1)]
    public int columns = 9;
    [Min(0.1f)]
    public float cellSize = 1f;
    public Vector2 gridOrigin;

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
        }
        else
        {
            Debug.Log("Clicked outside hero placement grid.");
        }
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
