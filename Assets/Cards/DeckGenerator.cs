using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gère la génération du deck, la pioche, l'affichage de la main,
/// et fournit des opérations de défausse/repioche pour les effets de carte.
/// Tirage sans remise, minimum 15 cartes, reshuffle quand la pioche est vide.
/// </summary>
public class FullDeckGenerator : MonoBehaviour
{
    private const int MinDeckSize = 15;

    [Header("Propriété d'équipe")]
    public BaseUnitScript.Team ownerTeam = BaseUnitScript.Team.Player;

    [Header("Deck par défaut (préfabs de Carte)")]
    public List<GameObject> defaultDeckPrefabs = new List<GameObject>();

    [Header("Paramètres de pioche")]
    public int deckSize = 20;
    public int initialHandSize = 3;
    public int maxHandSize = 8;

    [Header("Disposition de la main")]
    public float  cardSpacing = 2.5f;
    public float  rowSpacing  = 3f;
    public Vector2 handOrigin = new Vector2(0f, -4f);

    [Header("Options de visibilité")]
    [Tooltip("Si TRUE, cette main est masquée visuellement et non cliquable")]
    public bool hideHand = false;

    // --- interne ---
    private List<GameObject> deckPrefabs;
    private List<GameObject> deckPool    = new List<GameObject>();
    private List<GameObject> currentHand = new List<GameObject>();

    [HideInInspector] public List<GameObject> cardGOs       = new List<GameObject>();
    [HideInInspector] public List<Vector3>    slotPositions = new List<Vector3>();

    void Start()
    {
        if (DeckSelection.Instance != null && DeckSelection.Instance.selectedPrefabs.Count > 0)
            deckPrefabs = DeckSelection.Instance.selectedPrefabs;
        else
            deckPrefabs = defaultDeckPrefabs;

        BuildDeckPool();
        DrawInitialHand();
        DisplayHand();
    }

    private void BuildDeckPool()
    {
        deckPool.Clear();

        if (deckPrefabs == null || deckPrefabs.Count == 0)
        {
            Debug.LogError("❌ Aucun prefab assigné à deckPrefabs !");
            return;
        }

        var source = deckPrefabs.Where(p => p != null).ToList();
        if (source.Count == 0)
        {
            Debug.LogError("❌ Toutes les entrées de deckPrefabs sont null !");
            return;
        }

        deckPool.AddRange(source);

        while (deckPool.Count < MinDeckSize)
            deckPool.Add(source[Random.Range(0, source.Count)]);

        if (deckPool.Count != deckSize)
        {
            Debug.LogWarning($"⚠️ DeckSize ajusté de {deckSize} à {deckPool.Count} (min {MinDeckSize}).");
            deckSize = deckPool.Count;
        }

        for (int i = 0; i < deckPool.Count; i++)
        {
            int r = Random.Range(i, deckPool.Count);
            (deckPool[i], deckPool[r]) = (deckPool[r], deckPool[i]);
        }
    }

    private void DrawInitialHand()
    {
        currentHand.Clear();
        int drawCount = Mathf.Min(initialHandSize, deckPool.Count, maxHandSize);
        for (int i = 0; i < drawCount; i++)
        {
            currentHand.Add(deckPool[0]);
            deckPool.RemoveAt(0);
        }
    }

    public void DrawOneCard()
    {
        if (currentHand.Count >= maxHandSize) return;

        if (deckPool.Count == 0)
        {
            Debug.Log("🔄 Deck épuisé : reshuffle");
            BuildDeckPool();
        }

        currentHand.Add(deckPool[0]);
        deckPool.RemoveAt(0);
        DisplayHand();
    }

    public void RemoveCardAt(int idx)
    {
        if (idx < 0 || idx >= currentHand.Count) return;
        currentHand.RemoveAt(idx);
        DisplayHand();
    }

    public void DisplayHand()
    {
        foreach (Transform t in transform) Destroy(t.gameObject);
        cardGOs.Clear(); slotPositions.Clear();

        int count = currentHand.Count;
        int maxPerRow = 4;

        for (int i = 0; i < count; i++)
        {
            int row = i / maxPerRow;
            int idxRow = i % maxPerRow;
            int cardsInRow = Mathf.Min(maxPerRow, count - row * maxPerRow);
            float startX = handOrigin.x - cardSpacing * (cardsInRow - 1) / 2f;
            float x = startX + idxRow * cardSpacing;
            float y = handOrigin.y - row * rowSpacing;
            slotPositions.Add(new Vector3(x, y, 0f));
        }

        for (int i = 0; i < count; i++)
        {
            var prefab = currentHand[i];
            if (prefab == null) { Debug.LogError($"Carte n°{i} est null !"); continue; }

            var go = Instantiate(prefab, transform);
            go.transform.localPosition = slotPositions[i];
            cardGOs.Add(go);

            var drag = go.GetComponent<CardDragger>();
            if (drag != null)
            {
                drag.deckGen   = this;
                drag.ownerTeam = ownerTeam;
            }

            // visibilité/clics en fonction de hideHand
            bool visible = !hideHand;
            var sr  = go.GetComponent<SpriteRenderer>(); if (sr)  sr.enabled  = visible;
            var col = go.GetComponent<Collider2D>();     if (col) col.enabled = visible;
        }
    }

    public void SwapCards(int a, int b)
    {
        if (a < 0 || b < 0 || a >= currentHand.Count || b >= currentHand.Count) return;
        (currentHand[a], currentHand[b]) = (currentHand[b], currentHand[a]);
        (cardGOs[a],    cardGOs[b])      = (cardGOs[b],    cardGOs[a]);
        RepositionAll();
    }

    public void RepositionAll()
    {
        slotPositions.Clear();
        int count = cardGOs.Count;
        int maxPerRow = 4;

        for (int i = 0; i < count; i++)
        {
            int row = i / maxPerRow;
            int idxRow = i % maxPerRow;
            int cardsInRow = Mathf.Min(maxPerRow, count - row * maxPerRow);
            float startX = handOrigin.x - cardSpacing * (cardsInRow - 1) / 2f;
            float x = startX + idxRow * cardSpacing;
            float y = handOrigin.y - row * rowSpacing;
            slotPositions.Add(new Vector3(x, y, 0f));
        }

        for (int i = 0; i < count; i++)
            cardGOs[i].transform.localPosition = slotPositions[i];
    }

    // Effets (Espion, etc.)
    public void DiscardHandToDeck()
    {
        foreach (var cardPrefab in currentHand)
            deckPool.Add(cardPrefab);

        currentHand.Clear();
        DisplayHand();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deckPool.Count; i++)
        {
            int r = Random.Range(i, deckPool.Count);
            (deckPool[i], deckPool[r]) = (deckPool[r], deckPool[i]);
        }
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
            DrawOneCard();
    }

    // 🔵 NOUVEAU : contrôle runtime de la visibilité de la main
    public void SetHandVisible(bool visible)
    {
        hideHand = !visible;

        foreach (var go in cardGOs)
        {
            if (go == null) continue;
            var sr  = go.GetComponent<SpriteRenderer>(); if (sr)  sr.enabled  = visible;
            var col = go.GetComponent<Collider2D>();     if (col) col.enabled = visible;
        }
    }
}
