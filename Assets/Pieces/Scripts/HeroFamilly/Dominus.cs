using System.Collections.Generic;
using UnityEngine;

public class DominusScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Dominus] Aucun selectedTile défini.");
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

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRangeTiles = GetTilesInRange(originTile, GetAttackRange(), board);

        if (!inRangeTiles.Contains(targetTile))
            return area;

        if (targetTile.IsOccupied() && targetTile.currentPiece.team != this.team)
        {
            area.Add(targetTile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning("[Dominus] SpecialAttack échoué : cible nulle.");
            return;
        }

        int fixedDamage = 1;
        Debug.Log($"{name} tente de contrôler {target.name} en infligeant {fixedDamage} dégâts.");

        target.SetCurrentHealth(target.GetCurrentHealth() - fixedDamage);

        if (target.GetCurrentHealth() <= 0)
        {
            // Changement d’équipe et reset des stats
            target.team = this.team;

            target.name = target.name + " (Esclave)";
            target.SetCurrentHealth(target.GetMaxHealth());

            TextMesh text = target.GetComponentInChildren<TextMesh>();
            if (text != null)
            {
                text.color = team == Team.Player ? Color.white : Color.black;
                text.text = target.name;
            }

            Debug.Log($"{name} a pris le contrôle de {target.name}.");
        }

        SetCurrentEnergy(0);
        TurnManager.Instance.SpendPA();
    }
}
