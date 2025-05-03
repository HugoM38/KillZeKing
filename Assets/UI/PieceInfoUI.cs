using UnityEngine;
using TMPro;

public class PieceInfoUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI infoText;

    public TextMeshProUGUI turnText;


    public static PieceInfoUI instance;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    public static void ShowInfo(ChessPiece piece)
    {
        if (instance == null) return;

        instance.panel.SetActive(true);
        instance.infoText.text =
            $"{piece.type} ({piece.color})\n" +
            $"PV : {piece.currentHealth} / {piece.maxHealth}\n" +
            $"ATK : {piece.attackDamage}";
    }

    public static void Hide()
    {
        if (instance != null)
            instance.panel.SetActive(false);
    }

    public void UpdateTurnDisplay(ChessPiece.PieceColor currentPlayer)
    {
        turnText.text = $"Tour : {(currentPlayer == ChessPiece.PieceColor.White ? "Blanc" : "Noir")}";
    }

}
