using UnityEngine;

/// <summary>
/// Composant gérant les données et l'affichage d'une carte.
/// La logique de jeu (invocation, évolution, sorts) est déléguée à CardDragger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Card : MonoBehaviour
{
    [Header("Visuel")]
    public Sprite frontSprite;
    public Sprite backSprite;

    [Header("Données de la carte")]
    public string cardName;
    public int    cardValue;
    public string infoMessage;

    [Header("Invocation (optionnel)")]
    public GameObject summonPrefab;

    [Header("Évolution (optionnel)")]
    public GameObject evolutionPrefab;

    private SpriteRenderer spriteRenderer;
    private InfoCardDisplay infoDisplay;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var panel = transform.FindDeepChild("InfoPanel");
        if (panel != null)
            infoDisplay = panel.GetComponent<InfoCardDisplay>();
    }

    void Start()
    {
        ShowFront();
    }

    void OnMouseEnter()
    {
        if (infoDisplay != null)
            infoDisplay.Show(infoMessage);
    }

    void OnMouseExit()
    {
        if (infoDisplay != null)
            infoDisplay.Hide();
    }

    /// <summary>
    /// Prépare un sort/invocation ou évolution via SpellManager ou EvolutionManager.
    /// La carte n'est pas détruite ici : CardDragger gère la suppression au drop.
    /// </summary>
    void OnMouseDown()
    {
        // On ne prépare un nouveau sort qu'après qu'aucun sort ne soit déjà en attente
        if (SpellManager.Instance != null && SpellManager.Instance.pendingSpellValue == 0)
        {
            SpellManager.Instance.ActivateSpell(cardName, cardValue);
        }
    }

    public void ShowFront() => spriteRenderer.sprite = frontSprite;
    public void ShowBack()  => spriteRenderer.sprite = backSprite;
}
