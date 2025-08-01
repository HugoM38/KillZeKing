using System.Collections.Generic;
using UnityEngine;

public class VoleurScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Voleur] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);
        List<Tile> enemyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Enemy);

        foreach (Tile tile in enemyTiles)
            tile.SetHighlight(Color.yellow);

        return enemyTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        var area = new List<Tile>();
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || originTile.currentPiece == null)
            return area;

        if (targetTile == null || targetTile.currentPiece == null || targetTile.currentPiece.team == this.team)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        var inRangeTiles = GetTilesInRange(originTile, GetAttackRange(), board);

        if (inRangeTiles.Contains(targetTile))
            area.Add(targetTile);

        return area;
    }

    public override void SpecialAttack(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning("[Voleur] SpecialAttack échoué : aucune cible.");
            return;
        }

        Debug.Log($"{name} attaque {target.name} avec son attaque spéciale et tente de voler une carte !");
        target.SetCurrentHealth(target.GetCurrentHealth() - GetAttackDamage());

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est vaincu !");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
        }

        // ====== NOUVEAU : pioche d'une carte pour le joueur ======
        var playerDeck = FindObjectOfType<FullDeckGenerator>();
        if (playerDeck != null)
        {
            playerDeck.DrawOneCard();
            Debug.Log("[Voleur] Vous avez pioché 1 carte !");
        }
        else
        {
            Debug.LogWarning("[Voleur] Impossible de trouver le FullDeckGenerator pour piocher !");
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
