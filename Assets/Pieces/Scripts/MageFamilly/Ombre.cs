using System.Collections.Generic;
using UnityEngine;

public class OmbreScript : BaseUnitScript
{

    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Ombre] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange() + 2, board);
        List<Tile> enemyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Enemy);

        foreach (Tile tile in enemyTiles)
        {
            tile.SetHighlight(Color.yellow);
        }

        return enemyTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || originTile.currentPiece == null)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange() + 2, board);

        if (tilesInRange.Contains(targetTile) && targetTile.IsOccupied() && targetTile.currentPiece.team != team)
        {
            area.Add(targetTile);
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
                Debug.Log($"{name} inflige {GetAttackDamage()} à {tile.currentPiece.name} avec son attaque spéciale !");
                tile.currentPiece.SetCurrentHealth(tile.currentPiece.GetCurrentHealth() - GetAttackDamage());

                if (tile.currentPiece.GetCurrentHealth() <= 0)
                {
                    Debug.Log($"{tile.currentPiece.name} est éliminé par l'ombre !");
                    Destroy(tile.currentPiece.gameObject);
                    tile.SetPiece(null);
                }
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
