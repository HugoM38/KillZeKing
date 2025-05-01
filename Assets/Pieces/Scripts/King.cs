using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]


public class King : ChessPiece
{
    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1, 1), new Vector2Int(-1, 1),
        new Vector2Int(1, -1), new Vector2Int(-1, -1)
    };

    public override List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int pos)
    {
        List<Tile> moves = new List<Tile>();

        foreach (var dir in directions)
        {
            Vector2Int target = pos + dir;
            if (IsOnBoard(target.x, target.y, board))
            {
                Tile tile = board[target.x, target.y];
                if (!tile.IsOccupied() || tile.currentPiece.color != this.color)
                    moves.Add(tile);
            }
        }

        return moves;
    }

    private bool IsOnBoard(int x, int y, Tile[,] board)
    {
        return x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1);
    }
}
