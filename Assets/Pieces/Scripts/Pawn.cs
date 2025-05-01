using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class Pawn : ChessPiece
{
    public override List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int pos)
    {
        List<Tile> moves = new List<Tile>();
        int dir = (color == PieceColor.White) ? 1 : -1;

        int x = pos.x;
        int y = pos.y;

        if (IsOnBoard(x, y + dir, board) && !board[x, y + dir].IsOccupied())
        {
            moves.Add(board[x, y + dir]);
        }

        foreach (int dx in new int[] { -1, 1 })
        {
            int tx = x + dx;
            int ty = y + dir;

            if (IsOnBoard(tx, ty, board))
            {
                Tile target = board[tx, ty];
                if (target.IsOccupied() && target.currentPiece.color != this.color)
                    moves.Add(target);
            }
        }

        return moves;
    }

    private bool IsOnBoard(int x, int y, Tile[,] board)
    {
        return x >= 0 && x < board.GetLength(0) && y >= 0 && y < board.GetLength(1);
    }
}
