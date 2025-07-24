using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullDeckGenerator : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardFrontSprites;   // 52 sprites dans l’ordre
    public Sprite cardBackSprite;

    private List<CardData> allCards = new List<CardData>();
    private List<CardData> currentHand = new List<CardData>();

    private class CardData
    {
        public string name;
        public int value;
        public Sprite front;
    }

    void Start()
    {
        GenerateAllCards();
        SelectRandomHand();
        DisplayHand();  // <-- appel direct au lancement
    }

    void GenerateAllCards()
    {
        string[] suits  = { "Trèfle", "Carreau", "Cœur", "Pique" };
        string[] values = { "As", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Valet", "Dame", "Roi" };
        int index = 0;

        foreach (string suit in suits)
        {
            foreach (string value in values)
            {
                var card = new CardData
                {
                    name  = $"{value} de {suit}",
                    value = (value == "As") ? 11 : System.Array.IndexOf(values, value) + 1,
                    front = cardFrontSprites[index]
                };
                allCards.Add(card);
                index++;
            }
        }
    }

    void SelectRandomHand()
    {
        currentHand.Clear();
        var tempDeck = new List<CardData>(allCards);

        for (int i = 0; i < 5; i++)
        {
            int rand = Random.Range(0, tempDeck.Count);
            currentHand.Add(tempDeck[rand]);
            tempDeck.RemoveAt(rand);
        }
    }

    void DisplayHand()
    {
        // Supprime d'abord d'éventuelles vieilles instances
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        float spacing = 1.5f;
        float startX  = -(spacing * 2);

        for (int i = 0; i < currentHand.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab, transform);
            go.transform.localPosition = new Vector3(startX + i * spacing, 0f, 0f);

            var card = go.GetComponent<Card>();
            card.frontSprite  = currentHand[i].front;
            card.backSprite   = cardBackSprite;
            card.cardName     = currentHand[i].name;
            card.cardValue    = currentHand[i].value;
            card.infoMessage  = $"Texte perso carte {i+1}";
        }
    }
}
