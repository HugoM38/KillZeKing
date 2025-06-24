using UnityEngine;
using TMPro;

public class PieceInfoUI : MonoBehaviour
{
    public static PieceInfoUI instance;

    public TextMeshProUGUI infoText;
    public TextMeshProUGUI turnText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(true); // 🔒 Forcer actif (même si désactivé dans la scène)
        ShowNoSelection();
    }

    public void ShowInfo(ChessPiece piece)
    {
        if (piece == null)
        {
            ShowNoSelection();
            return;
        }

        infoText.text =
            $"{piece.type} ({piece.color})\n" +
            $"PV : {piece.currentHealth} / {piece.maxHealth}\n" +
            $"ATK : {piece.attackDamage}";
    }

    public void ShowNoSelection()
    {
        infoText.text = "Aucune pièce sélectionnée";
    }

    public void UpdateTurnDisplay(ChessPiece.PieceColor player, int pa, int maxPA, int pm, int maxPM)
    {
        string line1 = $"Tour : {(player == ChessPiece.PieceColor.White ? "Blanc" : "Noir")}";
        string line2 = $"PA : {pa} / {maxPA}   |   PM : {pm} / {maxPM}";
        turnText.text = $"{line1}\n{line2}";
    }

}
