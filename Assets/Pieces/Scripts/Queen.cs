using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class Queen : Rook
{
    public override List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int pos)
    {
        return GetLinearMoves(board, pos, new Vector2Int[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        });
    }
}
