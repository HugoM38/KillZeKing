using System.Collections.Generic;
using UnityEngine;

public class EnchanteurScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Enchanteur] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRange = GetTilesInRange(originTile, GetAttackRange(), board);

        // On filtre pour trouver tous les alliés, y compris soi-même
        List<Tile> allyTiles = new List<Tile>();
        foreach (Tile tile in inRange)
        {
            if (tile.IsOccupied() && tile.currentPiece.team == this.team)
                allyTiles.Add(tile);
        }

        if (!allyTiles.Contains(originTile))
            allyTiles.Add(originTile); // S'assurer que l'unité elle-même peut se cibler

        foreach (Tile tile in allyTiles)
        {
            tile.SetHighlight(Color.yellow);
        }

        return allyTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || targetTile == null || targetTile.currentPiece == null)
            return area;

        if (targetTile.currentPiece.team != this.team)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRange = GetTilesInRange(originTile, GetAttackRange(), board);
        if (!inRange.Contains(targetTile) && targetTile != originTile)
            return area;

        area.Add(targetTile);
        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null || target.team != this.team)
        {
            Debug.LogWarning("[Enchanteur] Cible invalide pour le buff.");
            return;
        }

        Debug.Log($"{name} lance un enchantement sur {target.name}.");

        int pvBonus = Random.Range(0, 3);
        int pvMaxBonus = Random.Range(0, 3);
        int atkBonus = Random.Range(0, 3);
        int moveBonus = Random.Range(0, 3);
        int rangeBonus = Random.Range(0, 3);

        target.SetMaxHealth(target.GetMaxHealth() + pvMaxBonus);
        target.SetCurrentHealth(target.GetCurrentHealth() + pvBonus);
        target.SetAttackDamage(target.GetAttackDamage() + atkBonus);
        target.SetMovementRange(target.GetMovementRange() + moveBonus);
        target.SetAttackRange(target.GetAttackRange() + rangeBonus);

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
