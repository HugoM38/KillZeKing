using System.Collections.Generic;
using UnityEngine;

public class IngenieurScript : BaseUnitScript
{
    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Ingenieur] Aucun selectedTile défini.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);
        List<Tile> emptyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Empty);

        foreach (Tile tile in emptyTiles)
        {
            tile.SetHighlight(Color.yellow);
        }

        return emptyTiles;
    }

    public override List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        List<Tile> area = new List<Tile>();

        if (targetTile == null || targetTile.IsOccupied())
            return area;

        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
            return area;

        Tile[,] board = BoardGenerator.Instance.GetBoard();
        List<Tile> tilesInRange = GetTilesInRange(originTile, GetAttackRange(), board);

        if (tilesInRange.Contains(targetTile))
        {
            area.Add(targetTile);
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript _)
    {
        Tile tile = SelectionManager.Instance.targetTile;
        if (tile == null || tile.IsOccupied())
        {
            Debug.LogWarning("[Ingenieur] Impossible d'invoquer sur cette case.");
            return;
        }

        if (PrefabFactory.Instance == null)
        {
            Debug.LogError("[Ingenieur] PrefabFactory.Instance est null !");
            return;
        }

        GameObject[] pool = new GameObject[]
        {
            PrefabFactory.Instance.BarricadePrefab,
            PrefabFactory.Instance.ShieldPrefab,
            PrefabFactory.Instance.TurretPrefab
        };

        GameObject prefab = pool[Random.Range(0, pool.Length)];
        GameObject clone = Instantiate(prefab, tile.transform.position, Quaternion.identity);
        BaseUnitScript unit = clone.GetComponent<BaseUnitScript>();

        if (unit == null)
        {
            Debug.LogError("[Ingenieur] Le prefab invoqué ne contient pas BaseUnitScript.");
            Destroy(clone);
            return;
        }

        unit.team = this.team;

        // ✅ Ajout important : placer l’unité sur la case
        tile.SetPiece(unit);

        // ✅ Appliquer le buff si Machina est dans l’équipe
        MachinaScript[] machinas = GameObject.FindObjectsByType<MachinaScript>(FindObjectsSortMode.None);
        foreach (MachinaScript machina in machinas)
        {
            if (machina.team == this.team)
            {
                machina.EnhanceUnit(unit);  // Appliquer le buff
                break;
            }
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    private void TryEnhanceUnit(BaseUnitScript unit)
    {
        #if UNITY_2023_1_OR_NEWER
                MachinaScript[] machinas = Object.FindObjectsByType<MachinaScript>(FindObjectsSortMode.None);
        #else
                MachinaScript[] machinas = Object.FindObjectsOfType<MachinaScript>();
        #endif

        foreach (var machina in machinas)
        {
            if (machina != null && machina.team == this.team && machina.IsBoostActive())
            {
                machina.EnhanceUnit(unit);
                break;
            }
        }
    }
}
