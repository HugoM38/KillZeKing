using System.Collections.Generic;
using UnityEngine;

public class SaboteurScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Saboteur] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);
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

        if (targetTile == null || targetTile.currentPiece == null || targetTile.currentPiece.team == this.team)
            return area;

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRange = GetTilesInRange(originTile, GetAttackRange(), board);

        if (!inRange.Contains(targetTile))
            return area;

        area.Add(targetTile);
        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning("[Saboteur] Aucune cible à attaquer.");
            return;
        }

        // Infliger 1 point de dégât
        Debug.Log($"{name} inflige 1 dégât et vide l'énergie de {target.name}.");
        target.SetCurrentHealth(target.GetCurrentHealth() - 1);

        // Vider l'énergie
        target.SetCurrentEnergy(0);

        // Si la cible meurt
        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est vaincu par sabotage !");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
