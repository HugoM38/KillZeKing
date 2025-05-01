using UnityEngine;
using System.Collections.Generic;

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

        PieceInfoUI.ShowInfo(piece);
        UIButtons.Instance.ShowActionButtons(true); // affichera les boutons
    }


    public void ClearSelection(Tile[,] board)
    {
        selectedPiece = null;
        validMoves.Clear();
        currentState = PlayerActionState.None;

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.Hide();
        UIButtons.Instance.ShowActionButtons(false);
    }

}
