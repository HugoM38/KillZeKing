using UnityEngine;
using System.Collections.Generic;

public class ChessPiece : MonoBehaviour
{
    public enum PieceColor { White, Black }

    public PieceColor color;

    public int maxHealth;
    public int currentHealth;
    public int attackDamage;

    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
    public PieceType type;

    public int movementRange = 1;
    public int attackRange = 1;

    private void Start()
    {
        currentHealth = maxHealth;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color == PieceColor.White ? Color.white : Color.black;
            sr.sortingOrder = 10; // pour affichage au-dessus des cases
        }
    }

    public virtual List<Vector2Int> GetAvailableMoves(Vector2Int origin, Tile[,] board)
    {
        return GetTilesInRange(origin, movementRange, board, isAttack: false);
    }

    public virtual List<Vector2Int> GetAttackableTiles(Vector2Int origin, Tile[,] board)
    {
        return GetTilesInRange(origin, attackRange, board, isAttack: true);
    }

    private List<Vector2Int> GetTilesInRange(Vector2Int origin, int range, Tile[,] board, bool isAttack)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<(Vector2Int pos, int dist)> queue = new Queue<(Vector2Int, int)>();

        visited.Add(origin);
        queue.Enqueue((origin, 0));

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        int boardWidth = board.GetLength(0);
        int boardHeight = board.GetLength(1);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            if (dist != 0)
                result.Add(current);

            if (dist >= range)
                continue;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                // Vérifie que la case est sur le plateau
                if (next.x < 0 || next.x >= boardWidth || next.y < 0 || next.y >= boardHeight)
                    continue;

                if (visited.Contains(next))
                    continue;

                visited.Add(next);
                Tile tile = board[next.x, next.y];

                if (tile.IsOccupied())
                {
                    // Pièce ennemie ? => attaquable uniquement
                    if (isAttack && tile.currentPiece.color != this.color)
                    {
                        result.Add(next);
                    }

                    // Peu importe l'équipe : on arrête la propagation ici
                    continue;
                }

                queue.Enqueue((next, dist + 1));
            }
        }

        return result;
    }

    public Vector2Int GetCurrentTilePosition()
    {
        if (SelectionManager.Instance.selectedPiece == this)
            return SelectionManager.Instance.selectedTile.coordinates;

        Debug.LogWarning("[ChessPiece] GetCurrentTilePosition() appelée sur une pièce non sélectionnée.");
        return new Vector2Int(-1, -1);
    }

    public bool TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}