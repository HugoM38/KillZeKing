// Assets/Cards/UIScript/DeckBuilderUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckBuilderUI : MonoBehaviour
{
    [Header("References in Hierarchy")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject cardButtonPrefab;
    [SerializeField] private Button     confirmBtn;
    [SerializeField] private TMP_Text   deckSizeText;

    [Header("All available card prefabs")]
    [SerializeField] private List<GameObject> allCardPrefabs;
    [SerializeField] private int maxPerCard    = 4;
    [SerializeField] private int maxDeckSize   = 20;
    [SerializeField] private int minDeckSize   = 15; // ← nouveau

    // State
    private Dictionary<GameObject,int> counts     = new();
    private List<CardButtonUI>         buttonUIs  = new();
    private int                        totalCount = 0;

    void Start()
    {
        // Init counts & UI
        foreach (var prefab in allCardPrefabs)
            counts[prefab] = 0;

        foreach (var prefab in allCardPrefabs)
        {
            var go = Instantiate(cardButtonPrefab, gridContainer);
            var ui = go.GetComponent<CardButtonUI>();
            var cardComp = prefab.GetComponent<Card>();
            if (ui == null || cardComp == null)
            {
                Debug.LogError("Prefab ou script manquant");
                continue;
            }
            ui.Setup(prefab, cardComp.frontSprite, 0, maxPerCard, OnCardCountChanged);
            buttonUIs.Add(ui);
        }

        confirmBtn.onClick.AddListener(OnConfirm);

        // Initial update
        UpdateDeckSizeText();
        RefreshAllButtons();
        UpdateConfirmInteractable();
    }

    private void OnCardCountChanged(GameObject prefab, int delta)
    {
        if (delta > 0)
        {
            if (totalCount >= maxDeckSize) return;
            counts[prefab] = Mathf.Min(counts[prefab] + 1, maxPerCard);
            totalCount++;
        }
        else if (counts[prefab] > 0)
        {
            counts[prefab]--;
            totalCount--;
        }

        UpdateDeckSizeText();
        RefreshAllButtons();
        UpdateConfirmInteractable();
    }

    private void UpdateDeckSizeText()
    {
        if (deckSizeText != null)
            deckSizeText.text = $"Deck size: {totalCount} / {maxDeckSize} (min {minDeckSize})";
    }

    private void RefreshAllButtons()
    {
        foreach (var ui in buttonUIs)
        {
            var prefab = ui.Prefab;
            int cnt    = counts[prefab];
            ui.UpdateCount(cnt);
            ui.SetPlusInteractable(cnt < maxPerCard && totalCount < maxDeckSize);
            ui.SetMinusInteractable(cnt > 0);
        }
    }

    // Nouveau : on désactive le Confirm tant que totalCount < minDeckSize
    private void UpdateConfirmInteractable()
    {
        if (confirmBtn != null)
            confirmBtn.interactable = (totalCount >= minDeckSize);
    }

    private void OnConfirm()
    {
        var sel = DeckSelection.Instance.selectedPrefabs;
        sel.Clear();
        foreach (var kv in counts)
            for (int i = 0; i < kv.Value; i++)
                sel.Add(kv.Key);

        SceneManager.LoadScene("MainMenu");
    }
}
