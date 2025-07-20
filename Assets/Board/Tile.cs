using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int coordinates;
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer highlightRenderer;
    public BaseUnitScript currentPiece;

    public void Init(Vector2Int coords, Color color)
    {
        coordinates = coords;
        backgroundRenderer = GetComponent<SpriteRenderer>();
        backgroundRenderer.color = color;
        ClearHighlight();
    }

    public bool IsOccupied()
    {
        return currentPiece != null;
    }

    public void SetHighlight(Color color)
    {
        if (highlightRenderer != null)
        {
            highlightRenderer.color = color;
            highlightRenderer.enabled = true;
        }
    }

    public void ClearHighlight()
    {
        if (highlightRenderer != null)
        {
            highlightRenderer.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        var sm = SelectionManager.Instance;
        if (sm == null) return;

        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        if (IsOccupied() && sm.currentState == PlayerActionState.None)
        {
            sm.SelectPiece(currentPiece, board);
            return;
        }

        if (sm.selectedPiece == null)
            return;

        if (sm.currentState == PlayerActionState.ActionSelected)
        {
            Color tileColor = highlightRenderer.color;

            if (tileColor == Color.blue)
            {
                MoveSelectedPieceToThisTile(board);
            }
            else if (tileColor == Color.red)
            {
                if (currentPiece != null && currentPiece.team != sm.selectedPiece.team)
                {
                    sm.selectedTile = this;
                    PieceInfoUI.instance.ShowTargetInfo(currentPiece);
                    UIButtons.Instance.ShowAttackOptions();
                }
            }
        }
    }

    public void MoveSelectedPieceToThisTile(Tile[,] board)
    {
        var sm = SelectionManager.Instance;
        var tm = TurnManager.Instance;

        BaseUnitScript selectedPiece = sm.selectedPiece;
        Vector2Int oldPos = selectedPiece.GetCurrentTilePosition();
        Tile oldTile = board[oldPos.x, oldPos.y];

        selectedPiece.transform.position = transform.position;
        currentPiece = selectedPiece;
        oldTile.currentPiece = null;

        tm.SpendPM();
        sm.ResetSelection();

        var stats = tm.CurrentStats;
        PieceInfoUI.instance.UpdateTurnDisplay(tm.currentPlayer, stats.pa, stats.maxPA, stats.pm, stats.maxPM);
    }

    public void MovePieceFrom(Tile sourceTile)
    {
        if (sourceTile == null || sourceTile.currentPiece == null)
        {
            Debug.LogWarning("Tentative de déplacer une pièce depuis une tile vide.");
            return;
        }

        BaseUnitScript movingPiece = sourceTile.currentPiece;

        movingPiece.transform.position = transform.position;
        currentPiece = movingPiece;
        sourceTile.currentPiece = null;

        Debug.Log($"{movingPiece.name} déplacé de {sourceTile.coordinates} à {coordinates}");
    }
}
