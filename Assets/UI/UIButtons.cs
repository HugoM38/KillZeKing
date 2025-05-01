using UnityEngine;

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
        SelectionManager.Instance.currentState = PlayerActionState.Moving;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        var piece = SelectionManager.Instance.selectedPiece;
        var moves = piece.GetAvailableMoves(board, piece.GetCurrentTilePosition(board));

        SelectionManager.Instance.validMoves = moves;

        foreach (Tile tile in board)
            tile.ClearHighlight();

        foreach (Tile tile in moves)
        {
            if (!tile.IsOccupied())
                tile.SetHighlight(Color.green);
        }
    }

    public void OnClickAttack()
    {
        SelectionManager.Instance.currentState = PlayerActionState.Attacking;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        var piece = SelectionManager.Instance.selectedPiece;
        var moves = piece.GetAvailableMoves(board, piece.GetCurrentTilePosition(board));

        SelectionManager.Instance.validMoves = moves;

        foreach (Tile tile in board)
            tile.ClearHighlight();

        foreach (Tile tile in moves)
        {
            if (tile.IsOccupied() && tile.currentPiece.color != piece.color)
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
}
