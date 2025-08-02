// Assets/Scripts/DeckSelection.cs
using System.Collections.Generic;
using UnityEngine;

public class DeckSelection : MonoBehaviour
{
    public static DeckSelection Instance { get; private set; }

    // Liste des pré­fabs de cartes choisis (+répétitions possibles)
    public List<GameObject> selectedPrefabs = new List<GameObject>();

    void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
