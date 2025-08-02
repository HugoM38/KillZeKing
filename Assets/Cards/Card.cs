using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite frontSprite;
    public Sprite backSprite;

    [Header("Card Data")]
    public string cardName;
    public int    cardValue;
    public string infoMessage;     // e.g. "Boule de Feu : inflige 1 dégât à un pion"

    [Header("Summon (optional)")]
    public GameObject summonPrefab;    // assigner le prefab de l’unité à invoquer, ou laisser null

    [Header("Evolution (optional)")]
    public GameObject evolutionPrefab; // assigner le prefab de l’évolution, ou laisser null

    private SpriteRenderer spriteRenderer;
    private InfoCardDisplay infoDisplay;

    void Awake()
    {
        // Récupère le SpriteRenderer et le panneau d’info en enfant (InfoPanel)
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
        // Affiche le tooltip/info
        if (infoDisplay != null)
            infoDisplay.Show(infoMessage);
    }

    void OnMouseExit()
    {
        if (infoDisplay != null)
            infoDisplay.Hide();
    }

    void OnMouseDown()
    {
        // Gestion des sorts via SpellManager
        if (SpellManager.Instance != null 
            && SpellManager.Instance.pendingSpellValue == 0)
        {
            SpellManager.Instance.ActivateSpell(cardName, cardValue);
            Destroy(gameObject);
            return;
        }

        // Gestion de l’évolution via EvolutionManager
        if (EvolutionManager.Instance != null 
            && evolutionPrefab != null)
        {
            EvolutionManager.Instance.ActivateEvolution(evolutionPrefab);
            Destroy(gameObject);
            return;
        }

        // Gestion de l’invocation via SummonManager
        if (SummonManager.Instance != null 
            && summonPrefab != null)
        {
            SummonManager.Instance.ActivateSummon(summonPrefab);
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>Affiche la face avant de la carte.</summary>
    public void ShowFront() => spriteRenderer.sprite = frontSprite;

    /// <summary>Affiche la face arrière de la carte.</summary>
    public void ShowBack()  => spriteRenderer.sprite = backSprite;
}
