using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || originTile.currentPiece != this)
        {
            Debug.LogWarning("[Shield] Aucun selectedTile valide.");
            return new List<Tile>();
        }

        originTile.SetHighlight(Color.yellow);
        return new List<Tile> { originTile };
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        if (targetTile != SelectionManager.Instance.selectedTile)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRange = GetTilesInRange(targetTile, GetAttackRange(), board);

        foreach (Tile tile in inRange)
        {
            if (tile.IsOccupied() && tile.currentPiece.team == this.team)
            {
                area.Add(tile);
            }
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript _)
    {
        List<Tile> affectedTiles = GetSpecialAttackArea(SelectionManager.Instance.selectedTile);

        foreach (Tile tile in affectedTiles)
        {
            BaseUnitScript ally = tile.currentPiece;
            if (ally != null)
            {
                Debug.Log($"{name} soigne {ally.name} de 1 PV avec son attaque spéciale.");
                ally.SetCurrentHealth(ally.GetCurrentHealth() + 1);
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    public override bool Attack(BaseUnitScript target)
    {
        if (target == null || target.team != this.team)
        {
            Debug.LogWarning("[Shield] Impossible de soigner : cible invalide ou ennemie.");
            return false;
        }

        int heal = GetAttackDamage();
        Debug.Log($"{name} soigne {target.name} de {heal} PV avec son attaque de base.");
        target.SetCurrentHealth(target.GetCurrentHealth() + heal);
        return false;
    }

    public override List<Tile> ShowMoveOptions()
    {
        Debug.Log("[Shield] Ne peut pas se déplacer.");
        return new List<Tile>();
    }

    public override List<Tile> ShowAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Shield] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);

        List<Tile> allyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Ally);
        if (!allyTiles.Contains(originTile))
            allyTiles.Add(originTile); // Permet de se soigner soi-même

        foreach (Tile tile in allyTiles)
        {
            tile.SetHighlight(Color.green); // Vert pour les soins
        }

        Debug.Log($"{name} a {allyTiles.Count} cibles alliées pour le soin.");
        return allyTiles;
    }

}
