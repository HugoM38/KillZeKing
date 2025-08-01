using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Prefab & Sprites")]
    public GameObject cardPrefab;     // Ton prefab de carte
    public Sprite[]  cardFrontSprites;
    public Sprite    cardBackSprite;

    private List<GameObject> deck = new List<GameObject>();

    void Start()
    {
        CreateDeck();
        ShuffleDeck();
    }

    void CreateDeck()
    {
        string[] cardNames = {
            "As", "2", "3", "4", "5", "6", "7",
            "8", "9", "10", "Valet", "Dame", "Roi"
        };

        for (int i = 0; i < cardFrontSprites.Length; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab);
            var card = cardObj.GetComponent<Card>();

            card.frontSprite = cardFrontSprites[i];
            card.backSprite  = cardBackSprite;
            card.cardName    = (i < cardNames.Length)
                ? cardNames[i]
                : "Carte " + (i + 1);
            card.cardValue   = (i == 0) ? 11 : i + 1;

            deck.Add(cardObj);
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            var tmp = deck[i];
            int idx = Random.Range(i, deck.Count);
            deck[i] = deck[idx];
            deck[idx] = tmp;
        }
    }

    public GameObject DrawCard()
    {
        if (deck.Count == 0) return null;
        var drawn = deck[0];
        deck.RemoveAt(0);
        return drawn;
    }
}
