using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public Sprite frontSprite;
    public Sprite backSprite;
    public string cardName;
    public int cardValue;
    public string infoMessage = "Je suis une carte...";

    private SpriteRenderer spriteRenderer;
    private InfoCardDisplay infoDisplay;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Recherche récursive de InfoPanel dans les enfants
        Transform panelTransform = transform.FindDeepChild("InfoPanel");

        if (panelTransform != null)
        {
            infoDisplay = panelTransform.GetComponent<InfoCardDisplay>();
            if (infoDisplay == null)
            {
                Debug.LogWarning("Le composant InfoCardDisplay est manquant sur InfoPanel !");
            }
        }
        else
        {
            Debug.LogWarning("InfoPanel introuvable dans la hiérarchie de la carte !");
        }
    }

    void Start()
    {
        ShowFront();
    }

    public void ShowFront() => spriteRenderer.sprite = frontSprite;

    public void ShowBack() => spriteRenderer.sprite = backSprite;

    void OnMouseDown()
    {
        Debug.Log("Carte cliquée : " + cardName); // Affiche le nom de la carte cliquée dans la console
        if (infoDisplay != null)
        {
            // infoDisplay.Show(cardName); // Affiche le nom de la carte dans l'InfoCardDisplay
        }
        else
        {
            Debug.LogWarning("infoDisplay est null, impossible d'afficher le message.");
        }
    }
}
