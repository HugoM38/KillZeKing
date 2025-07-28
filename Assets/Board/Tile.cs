using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Références")]
    public SpriteRenderer highlightRenderer;
    public BaseUnitScript currentPiece;
    public Vector2Int coordinates;
    public SpriteRenderer backgroundRenderer;
    public TileType tileType;

    public Sprite grassSprite;
    public Sprite waterSprite;
    public Sprite rockSprite;
    public Sprite sandSprite;
    public Sprite bridgeSprite;

    public void Init(Vector2Int coords, TileType type)
    {
        coordinates = coords;
        tileType = type;

        backgroundRenderer = GetComponent<SpriteRenderer>();
        backgroundRenderer.sprite = GetSpriteForType(type);
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

    private Sprite GetSpriteForType(TileType type)
    {
        return type switch
        {
            TileType.Grass => grassSprite,
            TileType.Water => waterSprite,
            TileType.Rock => rockSprite,
            TileType.Sand => sandSprite,
            TileType.Bridge => bridgeSprite,
            _ => null
        };
    }

    public bool IsWalkable()
    {
        return tileType != TileType.Water;
    }

}