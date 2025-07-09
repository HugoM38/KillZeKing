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

    public BaseUnitScript selectedPiece;
    public Tile selectedTile;
    public List<Tile> validMoves = new List<Tile>();
    public PlayerActionState currentState = PlayerActionState.None;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectPiece(BaseUnitScript piece, Tile[,] board)
    {
        selectedPiece = piece;
        selectedTile = null;

        // Trouver la tile sur laquelle se trouve la pièce
        foreach (Tile tile in board)
        {
            if (tile.currentPiece == piece)
            {
                selectedTile = tile;
                break;
            }
        }

        currentState = PlayerActionState.None;
        validMoves.Clear();

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.instance.ShowInfo(piece);

        bool isOwnPiece = piece.team == TurnManager.Instance.currentPlayer;
        UIButtons.Instance.ShowActionButtons(isOwnPiece);
    }

    public void ClearSelection(Tile[,] board)
    {
        selectedPiece = null;
        selectedTile = null;
        currentState = PlayerActionState.None;
        validMoves.Clear();

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.instance.ShowNoSelection();
        UIButtons.Instance.ShowActionButtons(false);
    }
}
