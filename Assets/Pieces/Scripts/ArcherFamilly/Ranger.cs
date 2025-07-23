using System.Collections.Generic;
using UnityEngine;

public class RangerScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Ranger] Aucun selectedTile défini.");
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

        if (targetTile == null || !targetTile.IsOccupied() || targetTile.currentPiece.team == this.team)
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
        if (target == null || target.team == this.team)
        {
            Debug.LogWarning("[Ranger] SpecialAttack échoué : cible invalide.");
            return;
        }

        int damage = GetAttackDamage() * 2;
        Debug.Log($"{name} utilise Tir Esquive : inflige {damage} à {target.name}.");

        target.SetCurrentHealth(target.GetCurrentHealth() - damage);

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est vaincu !");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
        }

        TryRetreat();

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    private void TryRetreat()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        Tile targetTile = SelectionManager.Instance.targetTile;

        if (originTile == null || targetTile == null)
            return;

        Vector2Int dir = originTile.coordinates - targetTile.coordinates;
        Vector2Int retreatPos = originTile.coordinates + dir;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        int boardWidth = board.GetLength(0);
        int boardHeight = board.GetLength(1);

        if (retreatPos.x >= 0 && retreatPos.x < boardWidth &&
            retreatPos.y >= 0 && retreatPos.y < boardHeight)
        {
            Tile retreatTile = board[retreatPos.x, retreatPos.y];
            if (!retreatTile.IsOccupied())
            {
                Debug.Log($"{name} recule sur {retreatTile.coordinates}.");
                transform.position = retreatTile.transform.position;
                retreatTile.SetPiece(this);
                originTile.SetPiece(null);
            }
            else
            {
                Debug.Log($"{name} ne peut pas reculer (case occupée).");
            }
        }
        else
        {
            Debug.Log($"{name} ne peut pas reculer (hors plateau).");
        }
    }
}
