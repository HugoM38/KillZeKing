using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Knight : ChessPiece
{
    private static readonly Vector2Int[] moves = new Vector2Int[]
    {
        new Vector2Int(2, 1), new Vector2Int(1, 2),
        new Vector2Int(-1, 2), new Vector2Int(-2, 1),
        new Vector2Int(-2, -1), new Vector2Int(-1, -2),
        new Vector2Int(1, -2), new Vector2Int(2, -1)
    };

    public override List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int pos)
    {
        List<Tile> result = new List<Tile>();

        foreach (var move in moves)
        {
            Vector2Int dest = pos + move;
            if (IsOnBoard(dest.x, dest.y, board))
            {
                Tile tile = board[dest.x, dest.y];
                if (!tile.IsOccupied() || tile.currentPiece.color != this.color)
                    result.Add(tile);
            }
        }

        return result;
    }

    private bool IsOnBoard(int x, int y, Tile[,] board)
    {
        return x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1);
    }
}
