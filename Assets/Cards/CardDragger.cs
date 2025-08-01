using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardDragger : MonoBehaviour
{
    [HideInInspector] public FullDeckGenerator deckGen;

    private Vector3        offset;
    private bool           isDragging     = false;
    private SpriteRenderer sr;
    private Collider2D     myCollider;
    private Tile           dragHoverTile;

    // Hover effect
    private Vector3 baseScale;
    private int     baseOrder;
    private float   baseAlpha;

    [Tooltip("Alpha de la carte lors du survol ou drag au-dessus d'une case")]    
    [Range(0f,1f)] public float hoverAlpha       = 0.5f;
    public float    hoverScaleFactor  = 1.2f;
    public int      hoverSortingOrder = 200;

    void Awake()
    {
        sr         = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        baseOrder  = sr.sortingOrder;
        baseAlpha  = sr.color.a;
    }

    void OnMouseDown()
    {
        isDragging      = true;
        offset          = transform.position - GetMouseWorldPos();
        sr.sortingOrder = hoverSortingOrder;
        baseScale       = transform.localScale;
        transform.localScale = baseScale * hoverScaleFactor;
    }

    void Update()
    {
        if (!isDragging) return;
        transform.position = GetMouseWorldPos() + offset;
        DetectHoverTile();
    }

    void OnMouseUp()
    {
        isDragging      = false;
        sr.sortingOrder = baseOrder;
        transform.localScale = baseScale;
        ResetTransparency();

        if (dragHoverTile != null)
        {
            SelectionManager.Instance.OnTileUnhovered(dragHoverTile);
            dragHoverTile = null;
        }

        Tile dropTile = GetTileUnderMouse();
        var card      = GetComponent<Card>();

        // 1) Invocation sur case vide
        if (dropTile != null && !dropTile.IsOccupied() && card.summonPrefab != null)
        {
            var summoned = Instantiate(card.summonPrefab,
                                       dropTile.transform.position,
                                       Quaternion.identity);
            if (summoned.TryGetComponent<BaseUnitScript>(out var newUnit))
                dropTile.SetPiece(newUnit);

            CleanupAndDestroy();
            return;
        }

        // 2) Sort sur un pion
        if (dropTile != null && dropTile.currentPiece != null)
        {
            var unit = dropTile.currentPiece;
            switch (card.cardName)
            {
                case "Boule de Feu":     unit.TakeDamage(card.cardValue); break;
                case "Soin":             unit.SetCurrentHealth(unit.GetCurrentHealth() + card.cardValue); break;
                case "Defense Shield":   unit.SetMaxHealth(unit.GetMaxHealth() + card.cardValue); break;
                default: Debug.LogWarning($"Sort inconnu : {card.cardName}"); break;
            }
            CleanupAndDestroy();
            return;
        }

        // 3) Sinon, repositionnement dans la main
        SnapBackToClosestSlot();
    }

    void OnMouseEnter()
    {
        if (isDragging) return;
        baseScale            = transform.localScale;
        transform.localScale = baseScale * hoverScaleFactor;
        sr.sortingOrder      = hoverSortingOrder;
    }

    void OnMouseExit()
    {
        if (isDragging) return;
        transform.localScale = baseScale;
        sr.sortingOrder      = baseOrder;
    }

    private void DetectHoverTile()
    {
        var wp3 = GetMouseWorldPos();
        var wp2 = new Vector2(wp3.x, wp3.y);

        myCollider.enabled = false;
        var hits = Physics2D.OverlapPointAll(wp2);
        myCollider.enabled = true;

        Tile newTile = null;
        foreach (var c in hits)
            if (c.TryGetComponent<Tile>(out newTile))
                break;

        if (newTile != dragHoverTile)
        {
            if (dragHoverTile != null)
                SelectionManager.Instance.OnTileUnhovered(dragHoverTile);

            dragHoverTile = newTile;

            if (dragHoverTile != null)
            {
                SelectionManager.Instance.OnTileHovered(dragHoverTile);
                // Applique transparence seulement en drag au-dessus d'une case
                ApplyTransparency(hoverAlpha);
            }
            else
            {
                // Remet l'opacité normale si plus sur aucune case
                ResetTransparency();
            }
        }
    }

    private Tile GetTileUnderMouse()
    {
        var wp3 = GetMouseWorldPos();
        var wp2 = new Vector2(wp3.x, wp3.y);

        myCollider.enabled = false;
        var hits = Physics2D.OverlapPointAll(wp2);
        myCollider.enabled = true;

        foreach (var c in hits)
            if (c.TryGetComponent<Tile>(out var t))
                return t;
        return null;
    }

    private void ApplyTransparency(float alpha)
    {
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, alpha);
    }

    private void ResetTransparency()
    {
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, baseAlpha);
    }

    private void CleanupAndDestroy()
    {
        int idx = deckGen.cardGOs.IndexOf(gameObject);
        if (idx >= 0)
        {
            deckGen.RemoveCardAt(idx);
            deckGen.cardGOs.RemoveAt(idx);
        }
        Destroy(gameObject);
        deckGen.RepositionAll();
    }

    private void SnapBackToClosestSlot()
    {
        int originalIndex = deckGen.cardGOs.IndexOf(gameObject);
        float bestDist    = float.MaxValue;
        int   bestIdx     = originalIndex;
        Vector3 localPos  = transform.localPosition;

        for (int i = 0; i < deckGen.slotPositions.Count; i++)
        {
            float d = Vector2.Distance(
                new Vector2(localPos.x, localPos.y),
                new Vector2(deckGen.slotPositions[i].x, deckGen.slotPositions[i].y)
            );
            if (d < bestDist)
            {
                bestDist = d;
                bestIdx  = i;
            }
        }

        if (bestIdx != originalIndex)
            deckGen.SwapCards(originalIndex, bestIdx);
        else
            deckGen.RepositionAll();
    }

    private Vector3 GetMouseWorldPos()
    {
        var mp = Input.mousePosition;
        mp.z    = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mp);
    }
}
