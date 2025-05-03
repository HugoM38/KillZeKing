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

        // ✅ Cas 1 : clic sur une case contenant une pièce
        if (IsOccupied())
        {
            // ❌ Si ce n’est pas ton tour, tu ne peux pas sélectionner cette pièce
            if (currentPiece.color != tm.currentPlayer)
                return;

            if (sm.currentState == PlayerActionState.None)
            {
                sm.SelectPiece(currentPiece, board);
                return;
            }

            return;
        }

        // ❌ Cas 2 : aucune pièce sélectionnée → rien à faire
        if (sm.selectedPiece == null)
            return;

        // ❌ Cas 3 : la case n’est pas dans les coups valides → ignorer
        if (!sm.validMoves.Contains(this))
            return;

        // ✅ Cas 4 : action valide (déplacement ou attaque)
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

                    sm.ClearSelection(board);
                    tm.NextTurn(); // 🔄 passer au joueur suivant
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
                    tm.NextTurn(); // 🔄 passer au joueur suivant
                }
                break;
        }
    }

}