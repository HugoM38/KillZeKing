using System.Collections.Generic;
using UnityEngine;

public class AssassinScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Assassin] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetMovementRange(), board);
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
        List<Tile> reachableTiles = GetTilesInRange(originTile, GetMovementRange(), board);

        if (!reachableTiles.Contains(targetTile))
            return area;

        area.Add(targetTile);
        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning("[Assassin] Pas de cible pour l'attaque spéciale.");
            return;
        }

        Tile targetTile = SelectionManager.Instance.targetTile;
        Tile assassinTile = SelectionManager.Instance.selectedTile;

        if (targetTile == null || assassinTile == null)
            return;

        if (!GetSpecialAttackArea(targetTile).Contains(targetTile))
        {
            Debug.LogWarning("[Assassin] Cible hors de portée spéciale.");
            return;
        }

        int damage = GetAttackDamage() * 3;
        Debug.Log($"{name} inflige {damage} dégâts à {target.name} avec attaque spéciale.");

        target.SetCurrentHealth(target.GetCurrentHealth() - damage);

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est éliminé.");
            Destroy(target.gameObject);
            targetTile.SetPiece(this);
            transform.position = targetTile.transform.position;
            assassinTile.SetPiece(null);
        }
        else
        {
            // Téléportation sur une case adjacente libre
            Tile[,] board = BoardGenerator.Instance.GetBoard();
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in directions)
            {
                Vector2Int pos = targetTile.coordinates + dir;
                if (pos.x < 0 || pos.x >= board.GetLength(0) || pos.y < 0 || pos.y >= board.GetLength(1))
                    continue;

                Tile adjacentTile = board[pos.x, pos.y];
                if (!adjacentTile.IsOccupied())
                {
                    transform.position = adjacentTile.transform.position;
                    adjacentTile.SetPiece(this);
                    assassinTile.SetPiece(null);
                    Debug.Log($"{name} se téléporte sur {pos} après l'attaque.");
                    break;
                }
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
