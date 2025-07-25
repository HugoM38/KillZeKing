using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardDragger : MonoBehaviour
{
    private Vector3       offset;
    private bool          isDragging = false;
    private SpriteRenderer sr;
    private Collider2D    myCollider;

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
        sr.sortingOrder = 100;  // Bring card to front
    }

    void Update()
    {
        if (isDragging)
            transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        isDragging      = false;
        sr.sortingOrder = 0;

        // **Désactive le collider de la carte** pour que le raycast n'accroche pas dessus
        myCollider.enabled = false;

        // 1) Raycast sous le curseur
        Vector3 worldPos  = GetMouseWorldPos();
        Vector2 worldPos2 = new Vector2(worldPos.x, worldPos.y);
        RaycastHit2D hit  = Physics2D.Raycast(worldPos2, Vector2.zero);

        // **Réactive tout de suite** le collider
        myCollider.enabled = true;

        if (hit.collider != null)
        {
            Tile dropTile = hit.collider.GetComponent<Tile>();
            if (dropTile != null && dropTile.currentPiece != null)
            {
                // 2) Applique l'effet
                BaseUnitScript unit = dropTile.currentPiece;
                Card card           = GetComponent<Card>();

                Debug.Log($"[CardDragger] Applique {card.cardName} sur {unit.name}");
                switch (card.cardName)
                {
                    case "Boule de Feu":
                        unit.TakeDamage(card.cardValue);
                        break;
                    case "Healing Light":
                        unit.SetCurrentHealth(unit.GetCurrentHealth() + card.cardValue);
                        break;
                    case "Defense Shield":
                        unit.SetMaxHealth(unit.GetMaxHealth() + card.cardValue);
                        break;
                    default:
                        Debug.LogWarning($"Sort inconnu : {card.cardName}");
                        break;
                }

                // 3) Retire la carte de la main
                int idx = deckGen.cardGOs.IndexOf(gameObject);
                if (idx >= 0) deckGen.cardGOs.RemoveAt(idx);
                Destroy(gameObject);
                deckGen.RepositionAll();
                return;
            }
        }

        // 4) Sinon, swap/snap dans la main
        int originalIndex = deckGen.cardGOs.IndexOf(gameObject);
        float bestDist    = float.MaxValue;
        int   bestIdx     = originalIndex;
        Vector3 localPos  = transform.localPosition;

        for (int i = 0; i < deckGen.slotPositions.Count; i++)
        {
            Vector3 slotPos = deckGen.slotPositions[i];
            float d = Vector2.Distance(
                new Vector2(localPos.x, localPos.y),
                new Vector2(slotPos.x, slotPos.y)
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
