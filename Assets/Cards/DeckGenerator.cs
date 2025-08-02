// FullDeckGenerator.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gère la génération du deck, la pioche, et l'affichage de la main.
/// Tirage sans remise, minimum 15 cartes, et reshuffle aléatoire quand le deck est épuisé.
/// </summary>
public class FullDeckGenerator : MonoBehaviour
{
    private const int MinDeckSize = 15; // Taille minimale du deck

    [Header("Deck par défaut (préfabs de Carte)")]
    [Tooltip("Glisse-dépose ici tes prefabs de carte (Card + CardDragger déjà configurés)")]
    public List<GameObject> defaultDeckPrefabs = new List<GameObject>();

    [Header("Paramètres de pioche")] 
    [Tooltip("Taille totale du deck, ajustée au besoin (>= MinDeckSize)")]
    public int deckSize = 20;
    [Tooltip("Nombre de cartes à tirer dans la main au départ (<=4)")]
    public int initialHandSize = 3;
    [Tooltip("Nombre maximal de cartes en main (<=8)")]
    public int maxHandSize = 8;

    [Header("Disposition de la main")]
    [Tooltip("Espacement horizontal entre cartes")] public float cardSpacing = 2.5f;
    [Tooltip("Espacement vertical entre lignes de cartes")] public float rowSpacing  = 3f;
    [Tooltip("Point (X,Y) centre de la première ligne de la main")] public Vector2 handOrigin = new Vector2(0f, -4f);

    [Header("Options de visibilité")]
    [Tooltip("Masquer complètement l'affichage de cette main (utile pour l'ennemi)")]
    public bool hideHand = false;

    // **Ne rien toucher en-dessous**
    private List<GameObject> deckPrefabs;        // Sélection finale: default ou personnalisée
    private List<GameObject> deckPool    = new List<GameObject>();
    private List<GameObject> currentHand = new List<GameObject>();

    [HideInInspector] public List<GameObject> cardGOs       = new List<GameObject>();
    [HideInInspector] public List<Vector3>    slotPositions = new List<Vector3>();

    void Start()
    {
        // 1) Choix du deck : custom si fourni, sinon le deck par défaut
        if (DeckSelection.Instance != null && DeckSelection.Instance.selectedPrefabs.Count > 0)
            deckPrefabs = DeckSelection.Instance.selectedPrefabs;
        else
            deckPrefabs = defaultDeckPrefabs;

        // 2) Génération du deck, pioche initiale, affichage
        BuildDeckPool();
        DrawInitialHand();
        DisplayHand();
    }

    /// <summary>
    /// 1) Constitue et mélange la pioche à partir de deckPrefabs (sans remise).
    ///    Assure un minimum de MinDeckSize cartes.
    /// </summary>
    private void BuildDeckPool()
    {
        deckPool.Clear();
        if (deckPrefabs == null || deckPrefabs.Count == 0)
        {
            Debug.LogError("❌ Aucun prefab assigné à deckPrefabs !");
            return;
        }

        // Liste valide
        var source = deckPrefabs.Where(p => p != null).ToList();
        if (source.Count == 0)
        {
            Debug.LogError("❌ Toutes les entrées de deckPrefabs sont null !");
            return;
        }

        // 1 carte par prefab
        deckPool.AddRange(source);

        // Ajout aléatoire pour atteindre la taille minimale
        while (deckPool.Count < MinDeckSize)
            deckPool.Add(source[Random.Range(0, source.Count)]);

        // Ajuste deckSize si nécessaire
        if (deckPool.Count != deckSize)
        {
            Debug.LogWarning($"⚠️ DeckSize ajusté de {deckSize} à {deckPool.Count} (min {MinDeckSize}).");
            deckSize = deckPool.Count;
        }

        // Mélange Fisher–Yates
        for (int i = 0; i < deckPool.Count; i++)
        {
            int r = Random.Range(i, deckPool.Count);
            var tmp = deckPool[i];
            deckPool[i] = deckPool[r];
            deckPool[r] = tmp;
        }
    }

    /// <summary>2) Pioche initiale.</summary>
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

    /// <summary>
    /// 3) Pioche une carte (appelé par TurnManager).
    ///    Reshuffle automatique si le deck est épuisé.
    /// </summary>
    public void DrawOneCard()
    {
        if (currentHand.Count >= maxHandSize)
            return;

        if (deckPool.Count == 0)
        {
            Debug.Log("🔄 Deck épuisé : reshuffle automatique");
            BuildDeckPool();
        }

        currentHand.Add(deckPool[0]);
        deckPool.RemoveAt(0);
        DisplayHand();
    }

    /// <summary>4) Retire la carte jouée de la main.</summary>
    public void RemoveCardAt(int idx)
    {
        if (idx < 0 || idx >= currentHand.Count)
            return;

        currentHand.RemoveAt(idx);
        DisplayHand();
    }

    /// <summary>5) (Ré)Affiche la main en deux lignes (max 4 cartes par ligne).</summary>
    public void DisplayHand()
    {
        // a) cleanup
        foreach (Transform t in transform)
            Destroy(t.gameObject);
        cardGOs.Clear(); slotPositions.Clear();

        int count = currentHand.Count;
        int maxPerRow = 4;

        // b) positions
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

        // c) instantiation & visibilité
        for (int i = 0; i < count; i++)
        {
            var prefab = currentHand[i];
            if (prefab == null)
            {
                Debug.LogError($"Carte n°{i} est null !");
                continue;
            }
            var go = Instantiate(prefab, transform);
            go.transform.localPosition = slotPositions[i];
            cardGOs.Add(go);
            go.SetActive(!hideHand);
            var drag = go.GetComponent<CardDragger>();
            if (drag != null) drag.deckGen = this;
        }
    }

    /// <summary>6) Échange deux cartes (drag & drop).</summary>
    public void SwapCards(int a, int b)
    {
        if (a < 0 || b < 0 || a >= currentHand.Count || b >= currentHand.Count)
            return;
        // échange données
        var tmpH = currentHand[a];
        currentHand[a] = currentHand[b];
        currentHand[b] = tmpH;
        // échange visuels
        var tmpG = cardGOs[a];
        cardGOs[a] = cardGOs[b];
        cardGOs[b] = tmpG;
        RepositionAll();
    }

    /// <summary>7) Repositionne la main sans ré-instancier.</summary>
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
}
