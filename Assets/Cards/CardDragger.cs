using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardDragger : MonoBehaviour
{
    private Vector3       offset;
    private bool          isDragging = false;
    private SpriteRenderer sr;

    [HideInInspector] public FullDeckGenerator deckGen;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        isDragging      = true;
        offset          = transform.position - GetMouseWorldPos();
        sr.sortingOrder = 100;  // passe devant
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging      = false;
        sr.sortingOrder = 0;

        // 1) Récupère la tuile survolée dans le SelectionManager
        var hoverTile = SelectionManager.Instance.hoveredTile;
        if (hoverTile != null && hoverTile.currentPiece != null)
        {
            var unit = hoverTile.currentPiece;
            var card = GetComponent<Card>();

            Debug.Log($"[CardDragger] Applique {card.cardName} sur {unit.name}");

            // 2) Applique l’effet en fonction du nom de la carte
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

            // 3) Détruit la carte et mets à jour la main
            int idx = deckGen.cardGOs.IndexOf(gameObject);
            if (idx >= 0) deckGen.cardGOs.RemoveAt(idx);
            Destroy(gameObject);
            deckGen.RepositionAll();
            return;
        }

        // 4) Sinon, échange ou replace la carte dans la main
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
        Vector3 mp = Input.mousePosition;
        mp.z        = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mp);
    }
}
