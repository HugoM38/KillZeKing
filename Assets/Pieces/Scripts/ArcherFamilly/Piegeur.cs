using System.Collections.Generic;
using UnityEngine;

public class PiegeurScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Piégeur] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRange = GetTilesInRange(originTile, GetAttackRange(), board);
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

        if (targetTile.currentPiece == null || targetTile.currentPiece.team == this.team)
            return area;

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRangeTiles = GetTilesInRange(originTile, GetAttackRange(), board);

        if (!inRangeTiles.Contains(targetTile))
            return area;

        area.Add(targetTile);
        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning("[Piégeur] SpecialAttack échoué : aucune cible.");
            return;
        }

        Debug.Log($"{name} utilise Piège sur {target.name} et inflige {GetAttackDamage()} dégâts.");
        target.SetCurrentHealth(target.GetCurrentHealth() - GetAttackDamage());

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est éliminé !");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
        }

        target.SetMovementRange(GetMovementRange() - 1);
        SetCurrentEnergy(0);
        TurnManager.Instance.SpendPA();
    }
}
