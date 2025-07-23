using System.Collections.Generic;
using UnityEngine;

public class SniperScript : BaseUnitScript
{
    private Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Sniper] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        List<Tile> directionTiles = new List<Tile>();
        Tile[,] board = BoardGenerator.Instance.GetBoard();
        int maxRange = 6;

        foreach (var dir in directions)
        {
            for (int i = 1; i <= maxRange; i++)
            {
                Vector2Int pos = originTile.coordinates + dir * i;
                if (pos.x < 0 || pos.x >= board.GetLength(0) || pos.y < 0 || pos.y >= board.GetLength(1))
                    break;

                Tile t = board[pos.x, pos.y];
                directionTiles.Add(t);
                t.SetHighlight(Color.yellow);
            }
        }

        return directionTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || targetTile == null)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        Vector2Int direction = targetTile.coordinates - originTile.coordinates;

        if (direction == Vector2Int.zero)
            return area;

        // On convertit la direction en vecteur unitaire (haut, bas, gauche, droite uniquement)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = new Vector2Int((int)Mathf.Sign(direction.x), 0);
        else
            direction = new Vector2Int(0, (int)Mathf.Sign(direction.y));

        for (int i = 1; i <= 6; i++)
        {
            Vector2Int pos = originTile.coordinates + direction * i;
            if (pos.x < 0 || pos.x >= board.GetLength(0) || pos.y < 0 || pos.y >= board.GetLength(1))
                break;

            Tile tile = board[pos.x, pos.y];
            if (tile.IsOccupied() && tile.currentPiece.team != this.team)
                area.Add(tile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        Tile targetTile = SelectionManager.Instance.targetTile;
        List<Tile> affected = GetSpecialAttackArea(targetTile);

        if (affected.Count == 0)
        {
            Debug.Log("[Sniper] Aucune cible touchée par l’attaque spéciale.");
            return;
        }

        foreach (Tile tile in affected)
        {
            if (tile.currentPiece != null && tile.currentPiece.team != this.team)
            {
                Debug.Log($"{name} touche {tile.currentPiece.name} avec {GetAttackDamage()} dégâts !");
                tile.currentPiece.SetCurrentHealth(tile.currentPiece.GetCurrentHealth() - GetAttackDamage());

                if (tile.currentPiece.GetCurrentHealth() <= 0)
                {
                    Debug.Log($"{tile.currentPiece.name} est éliminé !");
                    Destroy(tile.currentPiece.gameObject);
                    tile.SetPiece(null);
                }
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
