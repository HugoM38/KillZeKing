using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionMenu : MonoBehaviour
{
    public GameObject playerSelectionPanel;
    public GameObject enemySelectionPanel;

    private GameObject selectedHero;
    private bool selectingEnemy;

    public void SelectHero(GameObject hero)
    {
        Debug.LogWarning("Héros sélectionné : " + hero.name);
        selectedHero = hero;

        if (selectingEnemy)
            GameManager.Instance.enemySelected = hero;
        else
            GameManager.Instance.heroSelected = hero;
    }

    public void PlayGame()
    {
        if (!selectingEnemy)
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

            if (playerSelectionPanel != null)
                playerSelectionPanel.SetActive(false);
            if (enemySelectionPanel != null)
                enemySelectionPanel.SetActive(true);
            else
                SceneManager.LoadScene("EnemyCharacterSelection");

            selectingEnemy = true;
            selectedHero = null;
            Debug.LogWarning("Sélectionnez le héros ennemi.");
        }
        else
        {
            if (selectedHero == null)
            {
                Debug.LogWarning("Aucun héros ennemi sélectionné !");
                return;
            }

            Debug.LogWarning("Play game with heroes : " + GameManager.Instance.heroSelected.name + " vs " + selectedHero.name);
            SceneManager.LoadScene("Game");
        }
    }

    public void OpenDeckBuilder()
    {
        SceneManager.LoadScene("DeckBuilder");
    }
}
