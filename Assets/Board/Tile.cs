using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isOccupied;
    public Color baseColor;
    public ChessPiece currentPiece;

    public void Init(Vector2Int position, Color color)
    {
        gridPosition = position;
        baseColor = color;
        isOccupied = false;

        GetComponent<SpriteRenderer>().color = baseColor;
    }

    public bool IsOccupied()
    {
        return currentPiece != null;
    }

}
