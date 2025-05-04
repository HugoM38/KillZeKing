using System.Collections.Generic;
using UnityEngine;

public enum PlayerActionState
{
    None,
    Moving,
    Attacking
}

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public ChessPiece selectedPiece;
    public List<Tile> validMoves = new List<Tile>();
    public PlayerActionState currentState = PlayerActionState.None;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectPiece(ChessPiece piece, Tile[,] board)
    {
        selectedPiece = piece;
        currentState = PlayerActionState.None;
        validMoves.Clear();

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.instance.ShowInfo(piece);

        // ✅ N'affiche les boutons que si la pièce appartient au joueur actif
        bool isOwnPiece = piece.color == TurnManager.Instance.currentPlayer;
        UIButtons.Instance.ShowActionButtons(isOwnPiece);
    }

    public void ClearSelection(Tile[,] board)
    {
        selectedPiece = null;
        currentState = PlayerActionState.None;
        validMoves.Clear();

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.instance.ShowNoSelection();
        UIButtons.Instance.ShowActionButtons(false);
    }
}
