using UnityEngine;
using System.Collections.Generic;

public abstract class ChessPiece : MonoBehaviour
{
    public enum PieceColor { White, Black }

    public PieceColor color;

    public int maxHealth;
    public int currentHealth;
    public int attackDamage;

    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
    public PieceType type;

    private void Start()
    {
        currentHealth = maxHealth;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 1;
    }

    public abstract List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int currentPos);

    public Vector2Int GetCurrentTilePosition(Tile[,] board)
    {
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (board[x, y].currentPiece == this)
                    return new Vector2Int(x, y);
            }
        }

        Debug.LogError("Position de la pièce introuvable sur le plateau !");
        return Vector2Int.zero;
    }

    public bool TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} a pris {amount} dégâts → PV restants : {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log($"{name} a été détruit !");
            Destroy(gameObject);
            return true; // ✅ est mort
        }

        return false; // ✅ encore vivant
    }
}
