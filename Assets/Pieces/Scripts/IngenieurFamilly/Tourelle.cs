using System.Collections.Generic;
using UnityEngine;

public class TourelleScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || originTile.currentPiece != this)
        {
            Debug.LogWarning("[Tourelle] Aucun selectedTile valide.");
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
            if (tile.IsOccupied() && tile.currentPiece.team != this.team)
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
            BaseUnitScript target = tile.currentPiece;
            if (target != null)
            {
                Debug.Log($"{name} inflige 1 dégât à {target.name}.");
                target.SetCurrentHealth(target.GetCurrentHealth() - 1);

                if (target.GetCurrentHealth() <= 0)
                {
                    Debug.Log($"{target.name} est détruit !");
                    Destroy(target.gameObject);
                    tile.SetPiece(null);
                }
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    public override List<Tile> ShowMoveOptions()
    {
        Debug.Log("[Tourelle] Ne peut pas se déplacer.");
        return new List<Tile>();
    }
}
