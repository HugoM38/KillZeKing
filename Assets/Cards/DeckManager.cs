using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPrefab;          // Assigne ton prefab de carte ici via l'Inspector.
    public Sprite[] cardFrontSprites;        // Tableau des sprites pour l'avant des cartes.
    public Sprite cardBackSprite;            // Sprite pour le dos des cartes.

    private List<GameObject> deck = new List<GameObject>();

    void Start()
    {
        CreateDeck();
        ShuffleDeck();
        // Ici, tu peux éventuellement instancier quelques cartes pour les afficher dans la scène.
    }

    // Création d'un deck standard
    void CreateDeck()
    {
        string[] cardNames = {"As", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Valet", "Dame", "Roi"};
        for (int i = 0; i < cardFrontSprites.Length; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab);
            Card card = cardObj.GetComponent<Card>();
            card.frontSprite = cardFrontSprites[i];
            card.backSprite = cardBackSprite;

            if (i < cardNames.Length)
                card.cardName = cardNames[i];
            else
                card.cardName = "Carte " + (i + 1);

            card.cardValue = (i == 0) ? 11 : i + 1; // Par exemple : l'As vaut 11

            deck.Add(cardObj);
        }
    }

    // Mélange le deck
    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            GameObject temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Méthode pour tirer une carte
    public GameObject DrawCard()
    {
        if (deck.Count == 0)
            return null;
        
        GameObject drawnCard = deck[0];
        deck.RemoveAt(0);
        return drawnCard;
    }
}
