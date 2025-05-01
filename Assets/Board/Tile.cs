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
        if (sm == null || sm.selectedPiece == null)
            return;

        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        if (!sm.validMoves.Contains(this))
            return;

        ChessPiece attacker = sm.selectedPiece;
        Vector2Int oldPos = attacker.GetCurrentTilePosition(board);
        Tile oldTile = board[oldPos.x, oldPos.y];

        switch (sm.currentState)
        {
            case PlayerActionState.Moving:
                if (!IsOccupied())
                {
                    attacker.transform.position = transform.position;
                    currentPiece = attacker;
                    oldTile.currentPiece = null;

                    sm.ClearSelection(board); // ✅ Réinitialise après déplacement
                }
                break;

            case PlayerActionState.Attacking:
                if (IsOccupied() && currentPiece.color != attacker.color)
                {
                    currentPiece.TakeDamage(attacker.attackDamage);

                    if (currentPiece == null) // la pièce ennemie a été détruite
                    {
                        attacker.transform.position = transform.position;
                        currentPiece = attacker;
                        oldTile.currentPiece = null;
                    }

                    sm.ClearSelection(board); // ✅ Réinitialise après attaque
                }
                break;
        }
    }

    public void ExecuteAttack()
    {
        var sm = SelectionManager.Instance;
        if (sm == null || sm.selectedPiece == null || sm.currentState != PlayerActionState.Attacking)
            return;

        ChessPiece attacker = sm.selectedPiece;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
        Vector2Int attackerPos = attacker.GetCurrentTilePosition(board);
        Tile oldTile = board[attackerPos.x, attackerPos.y];

        bool killed = currentPiece.TakeDamage(attacker.attackDamage);

        if (killed)
        {
            attacker.transform.position = transform.position;
            currentPiece = attacker;
            oldTile.currentPiece = null;
        }

        sm.ClearSelection(board);
    }

}
