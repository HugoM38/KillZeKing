using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardDragger : MonoBehaviour
{
    private Vector3    offset;
    private bool       isDragging = false;
    private SpriteRenderer sr;
    private Collider2D myCollider;

    private Tile       dragHoverTile;

    [HideInInspector] public FullDeckGenerator deckGen;

    void Awake()
    {
        sr         = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        isDragging      = true;
        offset          = transform.position - GetMouseWorldPos();
        sr.sortingOrder = 100;
    }

    void Update()
    {
        if (!isDragging) return;

        transform.position = GetMouseWorldPos() + offset;

        Vector3 wp3 = GetMouseWorldPos();
        Vector2 wp2 = new Vector2(wp3.x, wp3.y);

        myCollider.enabled = false;
        Collider2D[] hits  = Physics2D.OverlapPointAll(wp2);
        myCollider.enabled = true;

        Tile newTile = null;
        foreach (var c in hits)
        {
            var t = c.GetComponent<Tile>();
            if (t != null)
            {
                newTile = t;
                break;
            }
        }

        if (newTile != dragHoverTile)
        {
            if (dragHoverTile != null)
                SelectionManager.Instance.OnTileUnhovered(dragHoverTile);

            dragHoverTile = newTile;

            if (dragHoverTile != null)
                SelectionManager.Instance.OnTileHovered(dragHoverTile);
        }
    }

    void OnMouseUp()
    {
        isDragging      = false;
        sr.sortingOrder = 0;

        if (dragHoverTile != null)
        {
            SelectionManager.Instance.OnTileUnhovered(dragHoverTile);
            dragHoverTile = null;
        }

        Vector3 wp3  = GetMouseWorldPos();
        Vector2 wp2  = new Vector2(wp3.x, wp3.y);

        myCollider.enabled = false;
        Collider2D[] hits  = Physics2D.OverlapPointAll(wp2);
        myCollider.enabled = true;

        Tile dropTile = null;
        foreach (var c in hits)
        {
            var t = c.GetComponent<Tile>();
            if (t != null)
            {
                dropTile = t;
                break;
            }
        }

        if (dropTile != null && dropTile.currentPiece != null)
        {
            var unit = dropTile.currentPiece;
            var card = GetComponent<Card>();
            Debug.Log($"[CardDragger] Applique {card.cardName} sur {unit.name}");

            switch (card.cardName)
            {
                case "Boule de Feu":
                    unit.TakeDamage(card.cardValue);
                    break;
                case "Soin":
                    unit.SetCurrentHealth(unit.GetCurrentHealth() + card.cardValue);
                    break;
                case "Defense Shield":
                    unit.SetMaxHealth(unit.GetMaxHealth() + card.cardValue);
                    break;
                default:
                    Debug.LogWarning($"Sort inconnu : {card.cardName}");
                    break;
            }

            int idx = deckGen.cardGOs.IndexOf(gameObject);
            if (idx >= 0) deckGen.cardGOs.RemoveAt(idx);
            Destroy(gameObject);
            deckGen.RepositionAll();
            return;
        }

        int originalIndex = deckGen.cardGOs.IndexOf(gameObject);
        float bestDist    = float.MaxValue;
        int   bestIdx     = originalIndex;
        Vector3 localPos  = transform.localPosition;

        for (int i = 0; i < deckGen.slotPositions.Count; i++)
        {
            Vector3 slot = deckGen.slotPositions[i];
            float d = Vector2.Distance(
                new Vector2(localPos.x, localPos.y),
                new Vector2(slot.x, slot.y)
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
        Vector3 mp = Input.mousePosition;
        mp.z        = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mp);
    }
}
