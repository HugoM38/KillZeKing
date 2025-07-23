using System.Collections.Generic;
using UnityEngine;

public class MageScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Mage] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);

        // Ajout manuel de sa propre case si le Mage est blessé
        if (originTile.IsOccupied() && originTile.currentPiece == this &&
            GetCurrentHealth() < GetMaxHealth())
        {
            tilesInRange.Add(originTile);
        }

        // Filtrer uniquement les alliés blessés
        List<Tile> potentialTargets = new List<Tile>();
        foreach (Tile tile in tilesInRange)
        {
            if (tile.IsOccupied() && tile.currentPiece.team == this.team)
            {
                if (tile.currentPiece.GetCurrentHealth() < tile.currentPiece.GetMaxHealth())
                {
                    potentialTargets.Add(tile);
                }
            }
        }

        foreach (Tile tile in potentialTargets)
        {
            tile.SetHighlight(Color.yellow);
        }

        return potentialTargets;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        if (targetTile != null && targetTile.IsOccupied() && targetTile.currentPiece.team == this.team)
        {
            area.Add(targetTile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null || target.team != this.team)
        {
            Debug.LogWarning("[Mage] La cible n'est pas un allié valide.");
            return;
        }

        int amount = 2;
        int before = target.GetCurrentHealth();
        target.SetCurrentHealth(before + amount);

        Debug.Log($"{name} soigne {target.name} de {amount} PV.");

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
