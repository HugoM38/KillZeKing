using System.Collections.Generic;
using UnityEngine;

public class SentinelleScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Sentinelle] Aucun selectedTile défini.");
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
            Debug.LogWarning("[Sentinelle] SpecialAttack échoué : aucune cible.");
            return;
        }

        int damage = GetAttackDamage();
        target.SetCurrentHealth(target.GetCurrentHealth() - damage);
        Debug.Log($"{name} inflige {damage} à {target.name}");

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est vaincu.");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
            SetMaxHealth(GetMaxHealth() + 1);
            SetCurrentHealth(GetCurrentHealth() + 3);
            Debug.Log($"{name} gagne +3 PV et +1 PV max !");
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
