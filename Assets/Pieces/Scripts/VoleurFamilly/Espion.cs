using System.Collections.Generic;
using UnityEngine;

public class EspionScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Espion] Aucun selectedTile défini.");
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
        List<Tile> area = new List<Tile>();

        if (targetTile == null || targetTile.currentPiece == null || targetTile.currentPiece.team == this.team)
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
        if (target == null)
        {
            Debug.LogWarning("[Espion] Aucune cible à attaquer.");
            return;
        }

        // Attaque normale
        Debug.Log($"{name} attaque {target.name} avec une frappe d'espionnage.");
        target.SetCurrentHealth(target.GetCurrentHealth() - GetAttackDamage());

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} a été éliminé par l'espion !");
            Destroy(target.gameObject);
            SelectionManager.Instance.targetTile.SetPiece(null);
        }

        // Effet de l'espion : défausser la main adverse et piocher 3 cartes
        // Détermination de la main adverse
        BaseUnitScript.Team opponent = (team == BaseUnitScript.Team.Player)
            ? BaseUnitScript.Team.Enemy
            : BaseUnitScript.Team.Player;
        FullDeckGenerator opponentHand = (opponent == BaseUnitScript.Team.Player)
            ? TurnManager.Instance.playerHand
            : TurnManager.Instance.enemyHand;

        opponentHand.DiscardHandToDeck();
        opponentHand.ShuffleDeck();
        opponentHand.DrawCards(3);

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
