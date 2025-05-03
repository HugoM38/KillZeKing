using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]


public class Bishop : Rook // hérite de GetLinearMoves
{
    public override List<Tile> GetAvailableMoves(Tile[,] board, Vector2Int pos)
    {
        return GetLinearMoves(board, pos, new Vector2Int[] {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1)
        });
    }
}
