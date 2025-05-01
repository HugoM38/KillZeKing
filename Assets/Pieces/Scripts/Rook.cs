using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class Rook : ChessPiece
{
    public override List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int pos)
    {
        return GetLinearMoves(board, pos, new Vector2Int[] {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        });
    }

    protected List<Tile> GetLinearMoves(Tile[,] board, Vector2Int pos, Vector2Int[] directions)
    {
        List<Tile> moves = new List<Tile>();

        foreach (var dir in directions)
        {
            Vector2Int current = pos + dir;
            while (IsOnBoard(current.x, current.y, board))
            {
                Tile tile = board[current.x, current.y];
                if (tile.IsOccupied())
                {
                    if (tile.currentPiece.color != this.color)
                        moves.Add(tile); // attaque
                    break; // bloqué
                }
                moves.Add(tile);
                current += dir;
            }
        }

        return moves;
    }

    protected bool IsOnBoard(int x, int y, Tile[,] board)
    {
        return x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1);
    }
}
