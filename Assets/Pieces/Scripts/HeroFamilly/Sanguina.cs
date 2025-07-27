using System.Collections.Generic;
using UnityEngine;

public class SanguinaScript : BaseUnitScript
{
    private int bonusMaxHealthGained = 0;
    private const int maxBonusHealth = 5;

    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null) return new List<Tile>();

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);
        List<Tile> enemies = FilterTiles(tilesInRange, originTile, TileFilter.Enemy);

        foreach (Tile tile in enemies)
        {
            tile.SetHighlight(Color.yellow);
        }

        return enemies;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        if (targetTile?.currentPiece == null || targetTile.currentPiece.team == this.team)
            return area;

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null) return area;

        if (!GetTilesInRange(originTile, GetAttackRange(), BoardGenerator.Instance.GetBoard()).Contains(targetTile))
            return area;

        area.Add(targetTile);
        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null) return;

        int damage = GetAttackDamage();
        target.SetCurrentHealth(target.GetCurrentHealth() - damage);

        if (target.GetCurrentHealth() <= 0)
        {
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);

            if (bonusMaxHealthGained < maxBonusHealth)
            {
                bonusMaxHealthGained++;
                SetMaxHealth(GetMaxHealth() + 1);
            }

            SetCurrentHealth(GetCurrentHealth() + 3);
        }

        SetCurrentEnergy(0);
        TurnManager.Instance.SpendPA();
    }
}
