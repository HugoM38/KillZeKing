using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardDragger : MonoBehaviour
{
    [HideInInspector] public FullDeckGenerator deckGen;

    private Vector3 offset;
    private bool isDragging = false;
    private SpriteRenderer sr;
    private Collider2D myCollider;
    private Tile dragHoverTile;

    private Vector3 originalScale;
    private Vector3 hoverScale;
    private int baseOrder;
    private float baseAlpha;

    [Tooltip("Alpha de la carte lors du survol ou du drag")]
    [Range(0f,1f)] public float hoverAlpha = 0.5f;
    public float hoverScaleFactor = 1.2f;
    public int hoverSortingOrder = 200;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        baseOrder = sr.sortingOrder;
        baseAlpha = sr.color.a;
        originalScale = transform.localScale;
        hoverScale = originalScale * hoverScaleFactor;

        if (deckGen == null)
            deckGen = FindObjectOfType<FullDeckGenerator>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
        sr.sortingOrder = hoverSortingOrder;
        transform.localScale = hoverScale;
    }

    void Update()
    {
        if (!isDragging) return;
        transform.position = GetMouseWorldPos() + offset;
        DetectHoverTile();
    }

    void OnMouseUp()
    {
        isDragging = false;
        sr.sortingOrder = baseOrder;
        transform.localScale = originalScale;
        ResetTransparency();

        if (dragHoverTile != null)
        {
            SelectionManager.Instance.OnTileUnhovered(dragHoverTile);
            dragHoverTile = null;
        }

        // Seul le joueur peut utiliser la carte
        if (TurnManager.Instance.currentPlayer != BaseUnitScript.Team.Player)
        {
            SnapBackToClosestSlot();
            return;
        }

        Card card = GetComponent<Card>();
        if (card == null)
        {
            Debug.LogError("[CardDragger] Aucun composant Card trouvé.");
            SnapBackToClosestSlot();
            return;
        }

        Tile dropTile = GetTileUnderMouse();

        // 1) Évolution
        if (dropTile != null && dropTile.currentPiece != null && card.evolutionPrefab != null)
        {
            BaseUnitScript existing = dropTile.currentPiece;
            BaseUnitScript evoUnit = card.evolutionPrefab.GetComponent<BaseUnitScript>();
            if (evoUnit != null
                && existing.Family == evoUnit.Family
                && existing.team == TurnManager.Instance.currentPlayer)
            {
                Destroy(existing.gameObject);
                GameObject evoGO = Instantiate(card.evolutionPrefab,
                    dropTile.transform.position, Quaternion.identity, dropTile.transform);
                if (evoGO.TryGetComponent<BaseUnitScript>(out var newUnit))
                    dropTile.SetPiece(newUnit);

                TurnManager.Instance.SpendPA();
                CleanupAndDestroy();
                return;
            }
            SnapBackToClosestSlot();
            return;
        }

        // 2) Invocation
        if (dropTile != null && !dropTile.IsOccupied() && card.summonPrefab != null)
        {
            bool isPlayer = TurnManager.Instance.currentPlayer == BaseUnitScript.Team.Player;
            int y = dropTile.coordinates.y;
            int h = BoardGenerator.Instance.height;
            int maxPlayerRow = h/2 - 1;
            int minEnemyRow = h - h/2;
            bool valid = isPlayer ? (y <= maxPlayerRow) : (y >= minEnemyRow);
            if (valid)
            {
                GameObject summGO = Instantiate(card.summonPrefab,
                    dropTile.transform.position, Quaternion.identity, dropTile.transform);
                if (summGO.TryGetComponent<BaseUnitScript>(out var newUnit))
                    dropTile.SetPiece(newUnit);

                TurnManager.Instance.SpendPA();
                CleanupAndDestroy();
                return;
            }
            SnapBackToClosestSlot();
            return;
        }

        // 3) Sort ciblé avec validation de cible
        if (dropTile != null && dropTile.currentPiece != null)
        {
            BaseUnitScript unit = dropTile.currentPiece;
            BaseUnitScript.Team currentPlayer = TurnManager.Instance.currentPlayer;
            bool isAlly = unit.team == currentPlayer;
            bool isEnemy = !isAlly;
            bool validTarget = false;

            // Déterminer type de sort : offensif ou soutien
            switch (card.cardName)
            {
                // Offensifs → ennemis uniquement
                case "Boule de Feu":
                case "Entrave":
                case "Bouclier anti-attaque":
                case "Sceau de silence":
                    validTarget = isEnemy;
                    break;

                // Soutien → alliés uniquement
                case "Soin":
                case "Defense Shield":
                    validTarget = isAlly;
                    break;

                default:
                    Debug.LogWarning($"[CardDragger] Sort ciblé inconnu : {card.cardName}");
                    break;
            }

            if (!validTarget)
            {
                SnapBackToClosestSlot();
                return;
            }

            // Application de l'effet
            switch (card.cardName)
            {
                case "Boule de Feu":
                    unit.TakeDamage(card.cardValue);
                    break;
                case "Entrave":
                    unit.ApplyStatus(BaseUnitScript.StatusEffect.Stun, 1);
                    break;
                case "Bouclier anti-attaque":
                    unit.ApplyStatus(BaseUnitScript.StatusEffect.NoAttack, 1);
                    break;
                case "Sceau de silence":
                    unit.ApplyStatus(BaseUnitScript.StatusEffect.Silence, 1);
                    unit.SetCurrentEnergy(0);
                    SelectionManager.Instance.OnTileSelected(dropTile);
                    break;
                case "Soin":
                    unit.SetCurrentHealth(unit.GetCurrentHealth() + card.cardValue);
                    break;
                case "Defense Shield":
                    unit.SetMaxHealth(unit.GetMaxHealth() + card.cardValue);
                    break;
            }

            TurnManager.Instance.SpendPA();
            CleanupAndDestroy();
            return;
        }

        // 4) Échec → retour en main
        SnapBackToClosestSlot();
    }

    void OnMouseEnter()
    {
        if (isDragging) return;
        transform.localScale = hoverScale;
        sr.sortingOrder = hoverSortingOrder;
        ApplyTransparency(hoverAlpha);
    }

    void OnMouseExit()
    {
        if (isDragging) return;
        transform.localScale = originalScale;
        sr.sortingOrder = baseOrder;
        ResetTransparency();
    }

    private void DetectHoverTile()
    {
        Vector3 wp = GetMouseWorldPos();
        var hits = Physics2D.OverlapPointAll(wp);
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
                SelectionManager.Instance.OnTileHovered(dragHoverTile);
            ApplyTransparency(hoverAlpha);
        }
    }

    private Tile GetTileUnderMouse()
    {
        Vector3 wp = GetMouseWorldPos();
        myCollider.enabled = false;
        var hits = Physics2D.OverlapPointAll(wp);
        myCollider.enabled = true;
        foreach (var c in hits)
            if (c.TryGetComponent<Tile>(out var t)) return t;
        return null;
    }

    private void ApplyTransparency(float a)
    {
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, a);
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
        float bestDist = float.MaxValue;
        int bestIdx = originalIndex;
        Vector3 localPos = transform.localPosition;
        for (int i = 0; i < deckGen.slotPositions.Count; i++)
        {
            float d = Vector2.Distance(localPos, deckGen.slotPositions[i]);
            if (d < bestDist) { bestDist = d; bestIdx = i; }
        }
        if (bestIdx != originalIndex)
            deckGen.SwapCards(originalIndex, bestIdx);
        else
            deckGen.RepositionAll();
    }

    private Vector3 GetMouseWorldPos()
    {
        var mp = Input.mousePosition;
        mp.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mp);
    }
}
