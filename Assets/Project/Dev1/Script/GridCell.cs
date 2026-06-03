using UnityEngine;

public class GridCell
{
    public int row;
    public int column;
    public Vector2 centerWorldPosition;
    public bool isOccupied;
    public GameObject placedHero;

    public GridCell(int row, int column, Vector2 centerWorldPosition)
    {
        this.row = row;
        this.column = column;
        this.centerWorldPosition = centerWorldPosition;
        isOccupied = false;
        placedHero = null;
    }
}
