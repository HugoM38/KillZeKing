using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionMenu : MonoBehaviour
{
    public GameObject[] enemyHeroes;

    private GameObject selectedHero;

    public void SelectHero(GameObject hero)
    {
        Debug.LogWarning("Héros sélectionné : " + hero.name);
        selectedHero = hero;
    }

    public void PlayGame()
    {
        var deckSelection = DeckSelection.Instance;
        int deckCount = 0;
        if (deckSelection != null && deckSelection.selectedPrefabs != null && deckSelection.selectedPrefabs.Count > 0)
            deckCount = deckSelection.selectedPrefabs.Count;
        else
        {
            var fullDeck = FindObjectOfType<FullDeckGenerator>();
            deckCount = fullDeck != null ? fullDeck.defaultDeckPrefabs.Count : 0;
        }

        if (deckCount < 15)
        {
            Debug.LogWarning($"Votre deck contient {deckCount} cartes ; il doit en contenir au moins 15 pour jouer.");
            return;
        }

        if (selectedHero == null)
        {
            Debug.LogWarning("Aucun héros sélectionné !");
            return;
        }

        Debug.LogWarning("Play game with hero : " + selectedHero.name);
        GameManager.Instance.heroSelected = selectedHero;
        GameManager.Instance.enemySelected = enemyHeroes[Random.Range(0, enemyHeroes.Length)];

        SceneManager.LoadScene("Game");
    }
    public void OpenDeckBuilder()
    {
        SceneManager.LoadScene("DeckBuilder");
    }
}
