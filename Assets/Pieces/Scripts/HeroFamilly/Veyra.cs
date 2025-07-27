using System.Collections.Generic;
using UnityEngine;

public class VeyraScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Veyra] Aucun selectedTile défini.");
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

        if (targetTile == originTile)
        {
            Debug.Log("[Veyra] Ciblage refusé : on ne peut pas se cibler soi-même.");
            return area;
        }

        if (targetTile.currentPiece == null || targetTile.currentPiece.team == this.team)
        {
            return area;
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRangeTiles = GetTilesInRange(originTile, GetAttackRange(), board);
        if (!inRangeTiles.Contains(targetTile))
        {
            Debug.Log($"[Veyra] La tile ciblée {targetTile.coordinates} est hors de portée.");
            return area;
        }

        area.Add(targetTile); // Inclure la cible elle-même

        int boardWidth = board.GetLength(0);
        int boardHeight = board.GetLength(1);
        Vector2Int center = targetTile.coordinates;

        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) > 2)
                    continue; // Pour un losange de Manhattan distance 2

                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);

                if (pos.x >= 0 && pos.x < boardWidth && pos.y >= 0 && pos.y < boardHeight)
                {
                    Tile tile = board[pos.x, pos.y];
                    if (tile.IsOccupied() && tile.currentPiece.team != this.team)
                    {
                        area.Add(tile);
                    }
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
                if (Random.value <= 0.5f)
                {
                    Debug.Log($"{name} inflige {GetAttackDamage()} à {tile.currentPiece.name} (50% réussi).");
                    tile.currentPiece.SetCurrentHealth(tile.currentPiece.GetCurrentHealth() - GetAttackDamage());

                    if (tile.currentPiece.GetCurrentHealth() <= 0)
                    {
                        Debug.Log($"{tile.currentPiece.name} est éliminé par l’attaque spéciale !");
                        Destroy(tile.currentPiece.gameObject);
                        tile.SetPiece(null);
                    }
                }
                else
                {
                    Debug.Log($"{tile.currentPiece.name} esquive l’attaque spéciale de Veyra !");
                }
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
