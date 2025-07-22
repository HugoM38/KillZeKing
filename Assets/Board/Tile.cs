using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Références")]
    public SpriteRenderer highlightRenderer;
    public BaseUnitScript currentPiece;
    public Vector2Int coordinates;
    public SpriteRenderer backgroundRenderer;

    public void Init(Vector2Int coords, Color color)
    {
        coordinates = coords;

        if (backgroundRenderer == null)
            backgroundRenderer = GetComponent<SpriteRenderer>();

        backgroundRenderer.color = color;
        SetHighlightActive(false);
    }


    public void SetHighlight(Color color)
    {
        if (highlightRenderer != null)
        {
            highlightRenderer.color = color;
            highlightRenderer.enabled = true;
        }
    }

    public void SetHighlightActive(bool isActive)
    {
        if (highlightRenderer != null)
            highlightRenderer.enabled = isActive;
    }

    public void SetPiece(BaseUnitScript piece)
    {
        currentPiece = piece;
    }

    private void OnMouseDown()
    {
        SelectionManager.Instance.OnTileSelected(this);
    }

    public bool IsOccupied()
    {
        return currentPiece != null;
    }
    
    private void OnMouseEnter()
    {
        SelectionManager.Instance?.OnTileHovered(this);
    }

    private void OnMouseExit()
    {
        SelectionManager.Instance?.OnTileUnhovered(this);
    }

}