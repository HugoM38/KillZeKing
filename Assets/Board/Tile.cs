using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int coordinates;
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer highlightRenderer;
    public ChessPiece currentPiece;

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
        var tm = TurnManager.Instance;
        if (sm == null || tm == null) return;

        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        if (IsOccupied() && sm.currentState == PlayerActionState.None)
        {
            sm.SelectPiece(currentPiece, board);
            return;
        }

        if (sm.selectedPiece == null)
            return;

        if (!sm.validMoves.Contains(this))
            return;

        ChessPiece attacker = sm.selectedPiece;
        Vector2Int oldPos = attacker.GetCurrentTilePosition();
        Tile oldTile = board[oldPos.x, oldPos.y];

        switch (sm.currentState)
        {
            case PlayerActionState.Moving:
                if (!IsOccupied())
                {
                    attacker.transform.position = transform.position;
                    currentPiece = attacker;
                    oldTile.currentPiece = null;

                    sm.ClearSelection(board);
                    tm.NextTurn();
                }
                break;

            case PlayerActionState.Attacking:
                if (IsOccupied() && currentPiece.color != attacker.color)
                {
                    bool killed = currentPiece.TakeDamage(attacker.attackDamage);

                    if (killed)
                    {
                        attacker.transform.position = transform.position;
                        currentPiece = attacker;
                        oldTile.currentPiece = null;
                    }

                    sm.ClearSelection(board);
                    tm.NextTurn();
                }
                break;
        }
    }
}
