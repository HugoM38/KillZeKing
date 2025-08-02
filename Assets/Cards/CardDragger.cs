using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardDragger : MonoBehaviour
{
    [HideInInspector] public FullDeckGenerator deckGen;

    private Vector3        offset;
    private bool           isDragging = false;
    private SpriteRenderer sr;
    private Collider2D     myCollider;
    private Tile           dragHoverTile;

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
        if (deckGen == null)
            deckGen = FindObjectOfType<FullDeckGenerator>();
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
        isDragging           = false;
        sr.sortingOrder      = baseOrder;
        transform.localScale = baseScale;
        ResetTransparency();

        if (dragHoverTile != null)
        {
            SelectionManager.Instance.OnTileUnhovered(dragHoverTile);
            dragHoverTile = null;
        }

        var card = GetComponent<Card>();
        if (card == null)
        {
            Debug.LogError("[CardDragger] Pas de composant Card sur " + name);
            SnapBackToClosestSlot();
            return;
        }

        Tile dropTile = GetTileUnderMouse();

        // --- 0) ÉVOLUTION sur unité existante ---
        if (dropTile != null &&
            dropTile.currentPiece != null &&
            card.evolutionPrefab != null)
        {
            // Récupère les familles
            var existingUnit = dropTile.currentPiece;
            var existingFamily = existingUnit.Family;

            if (card.evolutionPrefab.TryGetComponent<BaseUnitScript>(out var evoUnit) &&
                evoUnit.Family == existingFamily)
            {
                // Remplacement de l'unité
                Destroy(existingUnit.gameObject);
                var evoGO = Instantiate(
                    card.evolutionPrefab,
                    dropTile.transform.position,
                    Quaternion.identity,
                    dropTile.transform
                );
                if (evoGO.TryGetComponent<BaseUnitScript>(out var newUnit))
                    dropTile.SetPiece(newUnit);

                TurnManager.Instance.SpendPA();
                CleanupAndDestroy();
                return;
            }
        }

        // --- 1) Invocation sur case vide ---
        if (dropTile != null && !dropTile.IsOccupied() && card.summonPrefab != null)
        {
            var summoned = Instantiate(
                card.summonPrefab,
                dropTile.transform.position,
                Quaternion.identity,
                dropTile.transform
            );
            if (summoned.TryGetComponent<BaseUnitScript>(out var newUnit))
                dropTile.SetPiece(newUnit);

            TurnManager.Instance.SpendPA();
            CleanupAndDestroy();
            return;
        }

        // --- 2) Sort ciblé sur un pion ---
        if (dropTile != null && dropTile.currentPiece != null)
        {
            var unit = dropTile.currentPiece;
            switch (card.cardName)
            {
                case "Boule de Feu":
                    unit.TakeDamage(card.cardValue);
                    TurnManager.Instance.SpendPA();
                    break;
                case "Soin":
                    unit.SetCurrentHealth(unit.GetCurrentHealth() + card.cardValue);
                    TurnManager.Instance.SpendPA();
                    break;
                case "Defense Shield":
                    unit.SetMaxHealth(unit.GetMaxHealth() + card.cardValue);
                    TurnManager.Instance.SpendPA();
                    break;
                case "Entrave":
                    unit.ApplyStatus(BaseUnitScript.StatusEffect.Stun, 1);
                    TurnManager.Instance.SpendPA();
                    break;
                case "Bouclier anti-attaque":
                    unit.ApplyStatus(BaseUnitScript.StatusEffect.NoAttack, 1);
                    TurnManager.Instance.SpendPA();
                    break;
                case "Sceau de silence":
                    unit.ApplyStatus(BaseUnitScript.StatusEffect.Silence, 1);
                    unit.SetCurrentEnergy(0);
                    TurnManager.Instance.SpendPA();
                    SelectionManager.Instance.OnTileSelected(dropTile);
                    break;
                default:
                    Debug.LogWarning($"[CardDragger] Sort inconnu : {card.cardName}");
                    break;
            }

            CleanupAndDestroy();
            return;
        }

        // --- 3) Aucune action valide → replace la carte ---
        SnapBackToClosestSlot();
    }

    void OnMouseEnter()
    {
        if (isDragging) return;
        baseScale            = transform.localScale;
        transform.localScale = baseScale * hoverScaleFactor;
        sr.sortingOrder      = hoverSortingOrder;
        ApplyTransparency(hoverAlpha);
    }

    void OnMouseExit()
    {
        if (isDragging) return;
        transform.localScale = baseScale;
        sr.sortingOrder      = baseOrder;
        ResetTransparency();
    }

    private void DetectHoverTile()
    {
        var wp3 = GetMouseWorldPos();
        var hits = Physics2D.OverlapPointAll(new Vector2(wp3.x, wp3.y));
        myCollider.enabled = false;

        Tile newTile = null;
        foreach (var c in hits)
            if (c.TryGetComponent<Tile>(out newTile)) break;

        myCollider.enabled = true;

        if (newTile != dragHoverTile)
        {
            if (dragHoverTile != null)
                SelectionManager.Instance.OnTileUnhovered(dragHoverTile);

            dragHoverTile = newTile;

            if (dragHoverTile != null)
            {
                SelectionManager.Instance.OnTileHovered(dragHoverTile);
                ApplyTransparency(hoverAlpha);
            }
            else ResetTransparency();
        }
    }

    private Tile GetTileUnderMouse()
    {
        var wp3 = GetMouseWorldPos();
        myCollider.enabled = false;
        var hits = Physics2D.OverlapPointAll(new Vector2(wp3.x, wp3.y));
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
        if (deckGen != null)
        {
            int idx = deckGen.cardGOs.IndexOf(gameObject);
            if (idx >= 0) deckGen.RemoveCardAt(idx);
        }
        Destroy(gameObject);
    }

    private void SnapBackToClosestSlot()
    {
        if (deckGen == null) return;

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
