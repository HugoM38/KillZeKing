// Assets/Cards/UIScript/CardButtonUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardButtonUI : MonoBehaviour
{
    [SerializeField] private Image artworkImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button plusBtn;
    [SerializeField] private Button minusBtn;

    private GameObject prefab;
    private Action<GameObject,int> onChange;
    private int currentCount;
    private int maxCount;

    public GameObject Prefab => prefab;

    public void Setup(
        GameObject prefab,
        Sprite artwork,
        int startCount,
        int maxCount,
        Action<GameObject,int> onChange
    ){
        this.prefab      = prefab;
        this.onChange    = onChange;
        this.maxCount    = maxCount;
        this.currentCount= startCount;

        artworkImage.sprite = artwork;
        countText.text      = $"{currentCount} / {maxCount}";

        plusBtn.onClick.AddListener(()=> { onChange(prefab, +1); });
        minusBtn.onClick.AddListener(()=>{ onChange(prefab, -1); });
    }

    public void SetPlusInteractable(bool v)  => plusBtn.interactable  = v;
    public void SetMinusInteractable(bool v) => minusBtn.interactable = v;

    // appelé par DeckBuilderUI après onChange
    public void UpdateCount(int newCount)
    {
        currentCount = newCount;
        countText.text = $"{currentCount} / {maxCount}";
    }
}
