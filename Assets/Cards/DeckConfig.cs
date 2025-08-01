using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Cards/DeckConfig", fileName="NewDeckConfig")]
public class DeckConfig : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public CardDefinition card;    // Ton ScriptableObject de définition de carte
        [Range(1,3)] 
        public int            count;   // nombre d’exemplaires (1 à 3 max)
    }

    [Header("Composition du deck")]
    [Tooltip("Glisse-dépose ici les cartes + nombre d’exemples")]
    public List<Entry> entries = new List<Entry>();

    [Header("Paramètres de pioche")]
    [Tooltip("Taille de la main au départ")]
    public int handSize = 3;
}
