using System.Collections.Generic;
using UnityEngine;

public class FullDeckGenerator : MonoBehaviour
{
    [Header("Types de cartes (parallel arrays)")]
    [Tooltip("Sprites de face")]
    public Sprite[] cardFrontSprites;
    [Tooltip("Noms affichés pour chaque sprite")]
    public string[] cardNames;
    [Tooltip("Valeurs (dégât, soin, bouclier…) pour chaque sprite")]
    public int[]    cardValues;

    [Header("Paramètres du deck")]
    [Tooltip("Taille totale du deck (mélange avec remise)")]
    public int      deckSize = 20;
    [Tooltip("Nombre de cartes à tirer en main")]
    public int      handSize = 3;

    [Header("Visuels & Layout")]
    public Sprite   cardBackSprite;
    public GameObject cardPrefab;
    [Tooltip("Échelle relative de chaque carte (1 = taille du prefab)")]
    public float      cardScale   = 0.2f;
    [Tooltip("Espacement horizontal entre cartes")]
    public float      cardSpacing = 2.5f;
    [Tooltip("Décalage vertical local")]
    public float      yOffset     = -4f;

    private class CardData { public Sprite front; public string name; public int value; }
    private List<CardData> allCards    = new List<CardData>();
    private List<CardData> currentHand = new List<CardData>();

    [HideInInspector] public List<GameObject> cardGOs       = new List<GameObject>();
    [HideInInspector] public List<Vector3>    slotPositions = new List<Vector3>();

    private float prevScale, prevSpacing, prevYOffset;

    void Start()
    {
        if (cardFrontSprites.Length != cardNames.Length || cardNames.Length != cardValues.Length)
            Debug.LogError("Le tableau cardFrontSprites, cardNames et cardValues doivent être de même longueur !");

        prevScale   = cardScale;
        prevSpacing = cardSpacing;
        prevYOffset = yOffset;

        GenerateDeck();
        DrawHand();
        DisplayHand();
    }

    void Update()
    {
        if (cardScale != prevScale || cardSpacing != prevSpacing || yOffset != prevYOffset)
        {
            prevScale   = cardScale;
            prevSpacing = cardSpacing;
            prevYOffset = yOffset;
            RepositionAll();
        }
    }

    void GenerateDeck()
    {
        allCards.Clear();

        List<int> pool = new List<int>();
        for (int i = 0; i < cardFrontSprites.Length; i++)
            pool.Add(i);

        for (int i = 0; i < deckSize; i++)
        {
            if (pool.Count == 0)
            {
                pool.Clear();
                for (int j = 0; j < cardFrontSprites.Length; j++)
                    pool.Add(j);
            }

            int pick = Random.Range(0, pool.Count);
            int idx  = pool[pick];
            pool.RemoveAt(pick);

            allCards.Add(new CardData {
                front = cardFrontSprites[idx],
                name  = cardNames[idx],
                value = cardValues[idx]
            });
        }
    }

    void DrawHand()
    {
        currentHand.Clear();
        var temp = new List<CardData>(allCards);
        int draw  = Mathf.Min(handSize, temp.Count);
        for (int i = 0; i < draw; i++)
        {
            int r = Random.Range(0, temp.Count);
            currentHand.Add(temp[r]);
            temp.RemoveAt(r);
        }
    }

    public void DisplayHand()
    {
        foreach (Transform c in transform)
            Destroy(c.gameObject);
        cardGOs.Clear();
        slotPositions.Clear();

        float startX = -(cardSpacing * (currentHand.Count - 1) / 2f);
        for (int i = 0; i < currentHand.Count; i++)
            slotPositions.Add(new Vector3(startX + i * cardSpacing, yOffset, 0f));

        for (int i = 0; i < currentHand.Count; i++)
        {
            var data = currentHand[i];
            var go   = Instantiate(cardPrefab, transform);
            cardGOs.Add(go);

            go.transform.localPosition = slotPositions[i];
            go.transform.localScale    = Vector3.one * cardScale;

            var card = go.GetComponent<Card>();
            card.frontSprite = data.front;
            card.backSprite  = cardBackSprite;
            card.cardName    = data.name;
            card.cardValue   = data.value;
            card.infoMessage = $"{data.name} : {data.value}";

            var drag = go.GetComponent<CardDragger>() ?? go.AddComponent<CardDragger>();
            drag.deckGen = this;
            if (go.GetComponent<Collider2D>() == null)
                go.AddComponent<BoxCollider2D>();
        }
    }

    public void SwapCards(int idxA, int idxB)
    {
        var tmpData       = currentHand[idxA];
        currentHand[idxA] = currentHand[idxB];
        currentHand[idxB] = tmpData;

        var tmpGO         = cardGOs[idxA];
        cardGOs[idxA]     = cardGOs[idxB];
        cardGOs[idxB]     = tmpGO;

        RepositionAll();
    }

    public void RepositionAll()
    {
        slotPositions.Clear();
        int count = cardGOs.Count;
        float startX = -(cardSpacing * (count - 1) / 2f);
        for (int i = 0; i < count; i++)
            slotPositions.Add(new Vector3(startX + i * cardSpacing, yOffset, 0f));

        for (int i = 0; i < count; i++)
        {
            var go = cardGOs[i];
            go.transform.localScale    = Vector3.one * cardScale;
            go.transform.localPosition = slotPositions[i];
        }
    }
}
