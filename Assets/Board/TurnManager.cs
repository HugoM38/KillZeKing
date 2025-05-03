using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public ChessPiece.PieceColor currentPlayer = ChessPiece.PieceColor.White;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void NextTurn()
    {
        currentPlayer = (currentPlayer == ChessPiece.PieceColor.White)
            ? ChessPiece.PieceColor.Black
            : ChessPiece.PieceColor.White;

        PieceInfoUI.instance?.UpdateTurnDisplay(currentPlayer);
        Debug.Log($"🔁 Nouveau tour : {currentPlayer}");
    }
}
