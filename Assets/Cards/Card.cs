// Assets/Scripts/Card.cs
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite frontSprite;
    public Sprite backSprite;
    public string cardName;
    public int    cardValue;
    public string infoMessage;
    private SpriteRenderer spriteRenderer;
    private InfoCardDisplay infoDisplay;
    

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Récupère ton panel InfoCardDisplay dans les enfants
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



    void OnMouseDown()
    {
        if (SpellManager.Instance != null 
            && SpellManager.Instance.pendingSpellValue == 0) // aucun sort en attente
        {
            SpellManager.Instance.ActivateSpell(cardName, cardValue);
            Destroy(gameObject);
        }
    }

    public void ShowFront() => spriteRenderer.sprite = frontSprite;
    public void ShowBack()  => spriteRenderer.sprite = backSprite;
}
