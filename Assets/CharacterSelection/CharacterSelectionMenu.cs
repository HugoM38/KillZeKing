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
}
