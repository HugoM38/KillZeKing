using System.Collections.Generic;
using UnityEngine;

public class BerserkerScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile selfTile = SelectionManager.Instance.selectedTile;
        if (selfTile == null)
        {
            Debug.LogWarning("[Berserker] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        selfTile.SetHighlight(Color.yellow);
        return new List<Tile> { selfTile };
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || targetTile != originTile)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int adjacentPos = originTile.coordinates + dir;
            if (adjacentPos.x >= 0 && adjacentPos.x < board.GetLength(0) &&
                adjacentPos.y >= 0 && adjacentPos.y < board.GetLength(1))
            {
                Tile adjacentTile = board[adjacentPos.x, adjacentPos.y];
                if (adjacentTile.IsOccupied() && adjacentTile.currentPiece.team != this.team)
                {
                    area.Add(adjacentTile);
                }
            }
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
            return;

        List<Tile> affectedTiles = GetSpecialAttackArea(originTile);

        if (affectedTiles.Count == 0)
        {
            Debug.Log("[Berserker] Aucune cible valide à proximité.");
            return;
        }

        int healed = 0;

        foreach (Tile tile in affectedTiles)
        {
            if (tile.IsOccupied() && tile.currentPiece.team != this.team)
            {
                int damage = originTile.currentPiece.GetAttackDamage();
                tile.currentPiece.SetCurrentHealth(tile.currentPiece.GetCurrentHealth() - damage);
                Debug.Log($"{name} inflige {damage} à {tile.currentPiece.name} et se soigne de 1 PV.");

                if (tile.currentPiece.GetCurrentHealth() <= 0)
                {
                    Destroy(tile.currentPiece.gameObject);
                    tile.SetPiece(null);
                }

                healed++;
            }
        }

        SetCurrentHealth(GetCurrentHealth() + healed);
        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
