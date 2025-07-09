using UnityEngine;
using System.Collections.Generic;

public class UIButtons : MonoBehaviour
{
    public static UIButtons Instance;

    public GameObject buttonsPanel;

    private void Awake()
    {
        Instance = this;
        ShowActionButtons(false);
    }

    public void ShowActionButtons(bool show)
    {
        if (buttonsPanel != null)
            buttonsPanel.SetActive(show);
    }

    public void OnClickMove()
    {
        if (!TurnManager.Instance.HasEnoughPM())
        {
            Debug.Log("Pas assez de PM pour se déplacer.");
            return;
        }

        SelectionManager.Instance.currentState = PlayerActionState.Moving;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        var piece = SelectionManager.Instance.selectedPiece;
        var positions = piece.GetAvailableMoves(piece.GetCurrentTilePosition(), board);

        int boardWidth = board.GetLength(0);
        int boardHeight = board.GetLength(1);
        List<Tile> tiles = new List<Tile>();

        foreach (Vector2Int pos in positions)
        {
            if (pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight)
            {
                tiles.Add(board[pos.x, pos.y]);
            }
        }

        SelectionManager.Instance.validMoves = tiles;

        foreach (Tile tile in board)
            tile.ClearHighlight();

        foreach (Tile tile in tiles)
        {
            if (!tile.IsOccupied())
                tile.SetHighlight(Color.green);
        }
    }

    public void OnClickAttack()
    {
        if (!TurnManager.Instance.HasEnoughPA())
        {
            Debug.Log("Pas assez de PA pour attaquer.");
            return;
        }

        SelectionManager.Instance.currentState = PlayerActionState.Attacking;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        var piece = SelectionManager.Instance.selectedPiece;
        var positions = piece.GetAttackableTiles(piece.GetCurrentTilePosition(), board);

        int boardWidth = board.GetLength(0);
        int boardHeight = board.GetLength(1);
        List<Tile> tiles = new List<Tile>();

        foreach (Vector2Int pos in positions)
        {
            if (pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight)
            {
                tiles.Add(board[pos.x, pos.y]);
            }
        }

        SelectionManager.Instance.validMoves = tiles;

        foreach (Tile tile in board)
            tile.ClearHighlight();

        foreach (Tile tile in tiles)
        {
            if (tile.IsOccupied() && tile.currentPiece.team != piece.team)
                tile.SetHighlight(Color.red);
        }
    }

    public void OnClickCancel()
    {
        SelectionManager.Instance.currentState = PlayerActionState.None;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        foreach (Tile tile in board)
            tile.ClearHighlight();
    }

    public void OnClickEndTurn()
    {
        TurnManager.Instance.NextTurn();
        var tm = TurnManager.Instance;
        var stats = tm.CurrentStats;
        PieceInfoUI.instance.UpdateTurnDisplay(tm.currentPlayer, stats.pa, stats.maxPA, stats.pm, stats.maxPM);
    }
}
