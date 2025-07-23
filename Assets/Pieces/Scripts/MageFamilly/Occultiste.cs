using System.Collections.Generic;
using UnityEngine;

public class OccultisteScript : BaseUnitScript
{
    public GameObject ombrePrefab;

    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Occultiste] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> inRange = GetTilesInRange(originTile, GetAttackRange(), board);
        List<Tile> emptyTiles = FilterTiles(inRange, originTile, TileFilter.Empty);

        foreach (Tile tile in emptyTiles)
        {
            tile.SetHighlight(Color.yellow);
        }

        return emptyTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null || targetTile == null || targetTile.IsOccupied())
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
        Tile summonTile = SelectionManager.Instance.targetTile;
        if (summonTile == null || summonTile.IsOccupied())
        {
            Debug.LogWarning("[Occultiste] La case d'invocation est invalide.");
            return;
        }

        if (ombrePrefab == null)
        {
            Debug.LogError("[Occultiste] Prefab Guerrier non assigné !");
            return;
        }

        // Création de l’unité invoquée
        GameObject newUnit = Instantiate(ombrePrefab, summonTile.transform.position, Quaternion.identity);
        BaseUnitScript unitScript = newUnit.GetComponent<BaseUnitScript>();

        if (unitScript != null)
        {
            unitScript.team = this.team;
            summonTile.SetPiece(unitScript);
        }
        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }
}
