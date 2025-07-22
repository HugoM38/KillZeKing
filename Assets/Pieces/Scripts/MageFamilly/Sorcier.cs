using System.Collections.Generic;
using UnityEngine;

public class Sorcier : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Sorcier] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);
        List<Tile> enemyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Enemy);

        foreach (Tile tile in enemyTiles)
        {
            tile.SetHighlight(Color.yellow);
        }

        Debug.Log($"{name} a {enemyTiles.Count} cibles possibles pour l'attaque spéciale.");
        return enemyTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || originTile.currentPiece == null)
            return area;

        // Empêcher de cibler soi-même
        if (targetTile == originTile)
        {
            Debug.Log("[Sorcier] Ciblage refusé : on ne peut pas se cibler soi-même.");
            return area;
        }

        if (targetTile.currentPiece == null || targetTile.currentPiece.team == this.team)
        {
            return area;
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();

        // Vérifier que la cible est dans la portée d'attaque
        List<Tile> inRangeTiles = GetTilesInRange(originTile, GetAttackRange(), board);
        if (!inRangeTiles.Contains(targetTile))
        {
            Debug.Log($"[Sorcier] La tile ciblée {targetTile.coordinates} est hors de portée.");
            return area;
        }

        area.Add(targetTile);

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int adjacentPos = targetTile.coordinates + dir;
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
        List<Tile> affectedTiles = GetSpecialAttackArea(SelectionManager.Instance.targetTile);

        foreach (Tile tile in affectedTiles)
        {
            if (tile.IsOccupied() && tile.currentPiece.team != this.team)
            {
                Debug.Log($"{name} inflige {GetAttackDamage()} à {tile.currentPiece.name} avec attaque spéciale !");
                tile.currentPiece.SetCurrentHealth(tile.currentPiece.GetCurrentHealth() - GetAttackDamage());

                if (tile.currentPiece.GetCurrentHealth() <= 0)
                {
                    Debug.Log($"{tile.currentPiece.name} est vaincu par l'attaque spéciale !");
                    Destroy(tile.currentPiece.gameObject);
                    tile.SetPiece(null);
                }
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
