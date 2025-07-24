using UnityEngine;
using TMPro;

public class InfoCardDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(string cardName)
    {
        infoText.text = cardName; // Affiche le nom de la carte
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
