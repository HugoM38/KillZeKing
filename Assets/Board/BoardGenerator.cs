using UnityEngine;
using System.Collections;

public class BoardGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 16;
    public int height = 16;
    public float tileSpacing = 1.1f;

    private Tile[,] tiles;

    [Header("Guerriers")]
    public BaseUnitScript guerrierPrefab;
    public BaseUnitScript chevalierPrefab;
    public BaseUnitScript berserkerPrefab;
    public BaseUnitScript sentinellePrefab;

    [Header("Mages")]
    public BaseUnitScript magePrefab;
    public BaseUnitScript sorcierPrefab;
    public BaseUnitScript occultistePrefab;
    public BaseUnitScript enchanteurPrefab;

    [Header("Archers")]
    public BaseUnitScript archerPrefab;
    public BaseUnitScript sniperPrefab;
    public BaseUnitScript rangerPrefab;
    public BaseUnitScript piegeurPrefab;

    [Header("Voleurs")]
    public BaseUnitScript voleurPrefab;
    public BaseUnitScript assassinPrefab;
    public BaseUnitScript espionPrefab;
    public BaseUnitScript saboteurPrefab;

    [Header("Moines")]
    public BaseUnitScript moinePrefab;
    public BaseUnitScript moineGuerisseurPrefab;
    public BaseUnitScript maitreKiPrefab;
    public BaseUnitScript exorcistePrefab;

    [Header("Ingénieurs")]
    public BaseUnitScript ingenieurPrefab;
    public BaseUnitScript tourellePrefab;
    public BaseUnitScript barricadePrefab;
    public BaseUnitScript bouclierPrefab;

    void Start()
    {
        GenerateBoard();
        StartCoroutine(SetupAfterBoard());
    }

    IEnumerator SetupAfterBoard()
    {
        yield return new WaitForEndOfFrame();
        SetupPieces();
    }

    public Tile[,] GetBoard()
    {
        return tiles;
    }

    void GenerateBoard()
    {
        tiles = new Tile[width, height];

        Vector2 boardCenterOffset = new Vector2((width - 1) * tileSpacing / 2f, (height - 1) * tileSpacing / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x * tileSpacing, y * tileSpacing);
                position -= boardCenterOffset;

                GameObject tileGO = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileGO.name = $"Tile_{x}_{y}";

                Tile tile = tileGO.GetComponent<Tile>();
                Color tileColor = (x + y) % 2 == 0 ? Color.white : Color.black;
                tile.Init(new Vector2Int(x, y), tileColor);

                tiles[x, y] = tile;
            }
        }

        AdjustCamera();
    }

    void AdjustCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            float boardWidth = width * tileSpacing;
            float boardHeight = height * tileSpacing;

            float aspectRatio = (float)Screen.width / Screen.height;

            float sizeBasedOnWidth = boardWidth / (2f * aspectRatio);
            float sizeBasedOnHeight = boardHeight / 2f;

            mainCamera.orthographicSize = Mathf.Max(sizeBasedOnWidth, sizeBasedOnHeight) + 1f;
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        }
    }

    void SetupPieces()
    {
        if (tiles == null)
        {
            Debug.LogError("Board not generated yet!");
            return;
        }

        int playerY = 1;
        int enemyY = height - 2;

        // Guerriers
        SpawnUnit(guerrierPrefab, new Vector2Int(0, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(chevalierPrefab, new Vector2Int(1, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(berserkerPrefab, new Vector2Int(2, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(sentinellePrefab, new Vector2Int(3, playerY), BaseUnitScript.Team.Player);

        SpawnUnit(guerrierPrefab, new Vector2Int(0, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(chevalierPrefab, new Vector2Int(1, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(berserkerPrefab, new Vector2Int(2, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(sentinellePrefab, new Vector2Int(3, enemyY), BaseUnitScript.Team.Enemy);

        // Mages
        SpawnUnit(magePrefab, new Vector2Int(4, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(sorcierPrefab, new Vector2Int(5, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(occultistePrefab, new Vector2Int(6, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(enchanteurPrefab, new Vector2Int(7, playerY), BaseUnitScript.Team.Player);

        SpawnUnit(magePrefab, new Vector2Int(4, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(sorcierPrefab, new Vector2Int(5, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(occultistePrefab, new Vector2Int(6, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(enchanteurPrefab, new Vector2Int(7, enemyY), BaseUnitScript.Team.Enemy);

        // Archers
        SpawnUnit(archerPrefab, new Vector2Int(8, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(sniperPrefab, new Vector2Int(9, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(rangerPrefab, new Vector2Int(10, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(piegeurPrefab, new Vector2Int(11, playerY), BaseUnitScript.Team.Player);

        SpawnUnit(archerPrefab, new Vector2Int(8, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(sniperPrefab, new Vector2Int(9, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(rangerPrefab, new Vector2Int(10, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(piegeurPrefab, new Vector2Int(11, enemyY), BaseUnitScript.Team.Enemy);

        // Voleurs
        SpawnUnit(voleurPrefab, new Vector2Int(12, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(assassinPrefab, new Vector2Int(13, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(espionPrefab, new Vector2Int(14, playerY), BaseUnitScript.Team.Player);
        SpawnUnit(saboteurPrefab, new Vector2Int(15, playerY), BaseUnitScript.Team.Player);

        SpawnUnit(voleurPrefab, new Vector2Int(12, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(assassinPrefab, new Vector2Int(13, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(espionPrefab, new Vector2Int(14, enemyY), BaseUnitScript.Team.Enemy);
        SpawnUnit(saboteurPrefab, new Vector2Int(15, enemyY), BaseUnitScript.Team.Enemy);

        // Moines
        SpawnUnit(moinePrefab, new Vector2Int(0, 0), BaseUnitScript.Team.Player);
        SpawnUnit(moineGuerisseurPrefab, new Vector2Int(1, 0), BaseUnitScript.Team.Player);
        SpawnUnit(maitreKiPrefab, new Vector2Int(2, 0), BaseUnitScript.Team.Player);
        SpawnUnit(exorcistePrefab, new Vector2Int(3, 0), BaseUnitScript.Team.Player);

        SpawnUnit(moinePrefab, new Vector2Int(0, height - 1), BaseUnitScript.Team.Enemy);
        SpawnUnit(moineGuerisseurPrefab, new Vector2Int(1, height - 1), BaseUnitScript.Team.Enemy);
        SpawnUnit(maitreKiPrefab, new Vector2Int(2, height - 1), BaseUnitScript.Team.Enemy);
        SpawnUnit(exorcistePrefab, new Vector2Int(3, height - 1), BaseUnitScript.Team.Enemy);

        // Ingénieurs
        SpawnUnit(ingenieurPrefab, new Vector2Int(4, 0), BaseUnitScript.Team.Player);
        SpawnUnit(tourellePrefab, new Vector2Int(5, 0), BaseUnitScript.Team.Player);
        SpawnUnit(barricadePrefab, new Vector2Int(6, 0), BaseUnitScript.Team.Player);
        SpawnUnit(bouclierPrefab, new Vector2Int(7, 0), BaseUnitScript.Team.Player);

        SpawnUnit(ingenieurPrefab, new Vector2Int(4, height - 1), BaseUnitScript.Team.Enemy);
        SpawnUnit(tourellePrefab, new Vector2Int(5, height - 1), BaseUnitScript.Team.Enemy);
        SpawnUnit(barricadePrefab, new Vector2Int(6, height - 1), BaseUnitScript.Team.Enemy);
        SpawnUnit(bouclierPrefab, new Vector2Int(7, height - 1), BaseUnitScript.Team.Enemy);
    }

    void SpawnUnit(BaseUnitScript prefab, Vector2Int position, BaseUnitScript.Team team)
    {
        if (prefab == null)
        {
            Debug.LogError($"[BUG] Prefab manquant à la position {position.x},{position.y}");
            return;
        }

        if (tiles[position.x, position.y] == null)
        {
            Debug.LogError($"[BUG] Tile non générée à la position {position.x},{position.y}");
            return;
        }

        Tile tile = tiles[position.x, position.y];

        BaseUnitScript unit = Instantiate(prefab, tile.transform.position, Quaternion.identity, tile.transform);
        unit.team = team;
        tile.currentPiece = unit;

        Debug.Log($"[OK] {unit.name} placé en ({position.x},{position.y}) pour l'équipe {team}");
    }
}
