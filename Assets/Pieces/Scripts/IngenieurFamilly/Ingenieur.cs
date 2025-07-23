using System.Collections.Generic;
using UnityEngine;

public class IngenieurScript : BaseUnitScript
{
    [SerializeField] private GameObject barricadePrefab;
    [SerializeField] private GameObject tourellePrefab;
    [SerializeField] private GameObject shieldPrefab;

    public override List<Tile> ShowSpecialAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[Ingénieur] Aucun selectedTile défini.");
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

        if (!targetTile.IsOccupied())
        {
            Tile originTile = SelectionManager.Instance.selectedTile;
            Tile[,] board = BoardGenerator.Instance.GetBoard();
            List<Tile> inRange = GetTilesInRange(originTile, GetAttackRange(), board);

            if (inRange.Contains(targetTile))
            {
                area.Add(targetTile);
            }
        }

        return area;
    }

    public override void SpecialAttack(BaseUnitScript _)
    {
        Tile targetTile = SelectionManager.Instance.targetTile;

        if (targetTile == null || targetTile.IsOccupied())
        {
            Debug.LogWarning("[Ingénieur] La case est déjà occupée ou invalide.");
            return;
        }

        GameObject prefabToSummon = GetRandomStructurePrefab();
        if (prefabToSummon == null)
        {
            Debug.LogWarning("[Ingénieur] Aucun prefab d'invocation défini.");
            return;
        }

        GameObject newStructure = Instantiate(prefabToSummon, targetTile.transform.position, Quaternion.identity);
        BaseUnitScript structureScript = newStructure.GetComponent<BaseUnitScript>();
        if (structureScript != null)
        {
            structureScript.team = this.team;
            targetTile.SetPiece(structureScript);
            Debug.Log($"{name} invoque {structureScript.name} sur la case {targetTile.coordinates}");
        }
        else
        {
            Debug.LogError("[Ingénieur] Le prefab ne contient pas de script de type BaseUnitScript !");
            Destroy(newStructure);
            return;
        }

        TurnManager.Instance.SpendPA();
        SetCurrentEnergy(0);
    }

    private GameObject GetRandomStructurePrefab()
    {
        GameObject[] options = new GameObject[] { barricadePrefab, tourellePrefab, shieldPrefab };
        int index = Random.Range(0, options.Length);
        return options[index];
    }
}
