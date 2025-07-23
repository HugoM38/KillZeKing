using System.Collections.Generic;
using UnityEngine;

public class ArcherScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Archer] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();

        int boostedRange = GetAttackRange() * 2;
        List<Tile> inRange = GetTilesInRange(originTile, boostedRange, board);
        List<Tile> enemyTiles = FilterTiles(inRange, originTile, TileFilter.Enemy);

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
        if (originTile == null || targetTile == null || targetTile.currentPiece == null)
            return area;

        if (targetTile.currentPiece.team == this.team)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        int boostedRange = GetAttackRange() * 2;
        List<Tile> inRange = GetTilesInRange(originTile, boostedRange, board);

        if (inRange.Contains(targetTile))
        {
            area.Add(targetTile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null || target.team == this.team)
        {
            Debug.LogWarning("[Archer] Cible invalide pour l’attaque spéciale.");
            return;
        }

        int boostedDamage = GetAttackDamage() * 2;
        Debug.Log($"{name} tire une flèche spéciale sur {target.name} avec {boostedDamage} dégâts !");

        target.SetCurrentHealth(target.GetCurrentHealth() - boostedDamage);

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est vaincu par l’attaque spéciale !");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
