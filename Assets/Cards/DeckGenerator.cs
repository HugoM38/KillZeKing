using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FullDeckGenerator : MonoBehaviour
{
    [Header("Composition du deck (préfabs de Carte)")]
    [Tooltip("Glisse-dépose ici tes prefabs de carte (Card + CardDragger déjà configurés)")]
    public List<GameObject> deckPrefabs = new List<GameObject>();

    [Header("Paramètres de pioche")]
    [Tooltip("Taille totale du deck (pioche avec remise)")]
    public int deckSize = 20;
    [Tooltip("Nombre de cartes à tirer dans la main au départ (<=4)")]
    public int initialHandSize = 3;
    [Tooltip("Nombre maximal de cartes en main (<=8)")]
    public int maxHandSize = 8;

    [Header("Disposition de la main")]
    [Tooltip("Espacement horizontal entre cartes")]
    public float cardSpacing = 2.5f;
    [Tooltip("Espacement vertical entre lignes de cartes")]
    public float rowSpacing = 3f;
    [Tooltip("Point (X,Y) centre de la première ligne de la main")]
    public Vector2 handOrigin = new Vector2(0f, -4f);

    [Header("Options de visibilité")]
    [Tooltip("Masquer complètement l'affichage de cette main (utile pour l'ennemi)")]
    public bool hideHand = false;

    // **Ne rien toucher en-dessous**
    private List<GameObject> deckPool    = new List<GameObject>();
    private List<GameObject> currentHand = new List<GameObject>();
    [HideInInspector] public List<GameObject> cardGOs       = new List<GameObject>();
    [HideInInspector] public List<Vector3>    slotPositions = new List<Vector3>();

    void Start()
    {
        BuildDeckPool();
        DrawInitialHand();
        DisplayHand(); // on instancie toujours, puis on cache si besoin
    }

    /// <summary>1) Constitue et mélange la pioche à partir de deckPrefabs.</summary>
    private void BuildDeckPool()
    {
        deckPool.Clear();
        if (deckPrefabs == null || deckPrefabs.Count == 0)
        {
            Debug.LogError("❌ Aucun prefab assigné à deckPrefabs !");
            return;
        }
        var validPrefabs = deckPrefabs.Where(p => p != null).ToList();
        if (validPrefabs.Count == 0)
        {
            Debug.LogError("❌ Toutes les entrées de deckPrefabs sont null !");
            return;
        }

        // Tirage avec remise
        for (int i = 0; i < deckSize; i++)
            deckPool.Add(validPrefabs[Random.Range(0, validPrefabs.Count)]);

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

    /// <summary>3) Pioche une carte (appelé par TurnManager).</summary>
    public void DrawOneCard()
    {
        if (deckPool.Count == 0 || currentHand.Count >= maxHandSize)
            return;

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
        // a) cleanup des anciennes cartes
        foreach (Transform t in transform) Destroy(t.gameObject);
        cardGOs.Clear();
        slotPositions.Clear();

        int count = currentHand.Count;
        int maxPerRow = 4;

        // b) calcul des positions
        for (int i = 0; i < count; i++)
        {
            int row          = i / maxPerRow;
            int indexInRow   = i % maxPerRow;
            int cardsInRow   = Mathf.Min(maxPerRow, count - row * maxPerRow);
            float startX     = handOrigin.x - cardSpacing * (cardsInRow - 1) / 2f;
            float x          = startX + indexInRow * cardSpacing;
            float y          = handOrigin.y - row * rowSpacing;
            slotPositions.Add(new Vector3(x, y, 0f));
        }

        // c) instanciation & visibilité
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

            // **HERE** : on cache seulement visuellement si hideHand activé
            go.SetActive(!hideHand);

            // on relie le deckGen à chaque CardDragger
            var drag = go.GetComponent<CardDragger>();
            if (drag != null)
                drag.deckGen = this;
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

        // échange visuals
        var tmpG = cardGOs[a];
        cardGOs[a] = cardGOs[b];
        cardGOs[b] = tmpG;

        RepositionAll();
    }

    /// <summary>7) Repositionne simplement la main sans ré-instancier.</summary>
    public void RepositionAll()
    {
        slotPositions.Clear();
        int count = cardGOs.Count;
        int maxPerRow = 4;

        for (int i = 0; i < count; i++)
        {
            int row        = i / maxPerRow;
            int indexInRow = i % maxPerRow;
            int cardsInRow = Mathf.Min(maxPerRow, count - row * maxPerRow);
            float startX   = handOrigin.x - cardSpacing * (cardsInRow - 1) / 2f;
            float x        = startX + indexInRow * cardSpacing;
            float y        = handOrigin.y - row * rowSpacing;
            slotPositions.Add(new Vector3(x, y, 0f));
        }

        for (int i = 0; i < count; i++)
            cardGOs[i].transform.localPosition = slotPositions[i];
    }
}
