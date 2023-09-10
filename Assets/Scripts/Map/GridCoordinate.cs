using UnityEngine;

[System.Serializable]
public class GridCoordinate
{
    public int x;
    public int y;

    public GridCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    //显示转化网格坐标int Vector2为float Vector2
    public static explicit operator Vector2 (GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }

    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }

    public static explicit operator Vector3 (GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0);
    }

    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}
