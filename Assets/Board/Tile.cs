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

        // Cas 1 : clic sur une pièce au repos
        if (IsOccupied() && sm.currentState == PlayerActionState.None)
        {
            sm.SelectPiece(currentPiece, board);
            return;
        }

        // Cas 2 : aucune pièce sélectionnée => rien
        if (sm.selectedPiece == null)
            return;

        // Cas 3 : action sélectionnée, alors on agit selon la couleur
        if (sm.currentState == PlayerActionState.ActionSelected)
        {
            Color tileColor = highlightRenderer.color;

            if (tileColor == Color.blue)
            {
                // Déplacement
                MoveSelectedPieceToThisTile(board);
            }
            else if (tileColor == Color.red)
            {
                // Sélection d'une cible ennemie
                if (currentPiece != null && currentPiece.team != sm.selectedPiece.team)
                {
                    sm.selectedTile = this;
                    PieceInfoUI.instance.ShowTargetInfo(currentPiece);
                    UIButtons.Instance.ShowAttackOptions();
                }
            }
        }
    }

    private void MoveSelectedPieceToThisTile(Tile[,] board)
    {
        var sm = SelectionManager.Instance;
        var tm = TurnManager.Instance;

        if (!tm.HasEnoughPM())
        {
            Debug.Log("Pas assez de PM pour se déplacer.");
            return;
        }

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
}
