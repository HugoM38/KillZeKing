using UnityEngine;
using System.Collections;

public class BoardGenerator : MonoBehaviour
{
    public TextAsset mapJson;
    public GameObject tilePrefab;
    public int width = 16;
    public int height = 16;
    public float tileSpacing = 1.1f;

    private Tile[,] tiles;

    [Header("Guerriers")]
    public GuerrierScript guerrierPrefab;
    public BaseUnitScript chevalierPrefab;
    public BaseUnitScript berserkerPrefab;
    public BaseUnitScript sentinellePrefab;

    [Header("Mages")]
    public MageScript magePrefab;
    public SorcierScript sorcierPrefab;
    public OccultisteScript occultistePrefab;
    public EnchanteurScript enchanteurPrefab;
    public OmbreScript ombrePrefab;

    [Header("Archers")]
    public ArcherScript archerPrefab;
    public SniperScript sniperPrefab;
    public RangerScript rangerPrefab;
    public PiegeurScript piegeurPrefab;

    [Header("Voleurs")]
    public VoleurScript voleurPrefab;
    public AssassinScript assassinPrefab;
    public EspionScript espionPrefab;
    public SaboteurScript saboteurPrefab;

    [Header("Ingénieurs")]
    public IngenieurScript ingenieurPrefab;
    public TourelleScript tourellePrefab;
    public BarricadeScript barricadePrefab;
    public ShieldScript bouclierPrefab;

    [Header("Héros")]
    public DominusScript dominusPrefab;
    public InspirisScript inspirisPrefab;
    public SanguinaScript sanguinaPrefab;
    public MachinaScript machinaPrefab;
    public VeyraScript VeyraPrefab;

    public static BoardGenerator Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


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
        MapData mapData = JsonUtility.FromJson<MapData>(mapJson.text);
        width = mapData.width;
        height = mapData.height;
        tiles = new Tile[width, height];

        Vector2 boardCenterOffset = new Vector2((width - 1) * tileSpacing / 2f, (height - 1) * tileSpacing / 2f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                string typeString = mapData.tiles[index];
                if (!System.Enum.TryParse(typeString, out TileType type))
                {
                    Debug.LogError($"Type de tuile inconnu : {typeString} à ({x},{y})");
                    type = TileType.Grass;
                }

                Vector2 position = new Vector2(x * tileSpacing, y * tileSpacing) - boardCenterOffset;
                GameObject tileGO = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileGO.name = $"Tile_{x}_{y}";

                Tile tile = tileGO.GetComponent<Tile>();
                tile.Init(new Vector2Int(x, y), type);
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

        GameObject playerHeroGO = GameManager.Instance.heroSelected;
        GameObject enemyHeroGO = GameManager.Instance.enemySelected;

        if (playerHeroGO != null)
        {
            BaseUnitScript playerHeroPrefab = playerHeroGO.GetComponent<BaseUnitScript>();
            SpawnUnit(playerHeroPrefab, new Vector2Int(width / 2, playerY), BaseUnitScript.Team.Player);
        }

        if (enemyHeroGO != null)
        {
            BaseUnitScript enemyHeroPrefab = enemyHeroGO.GetComponent<BaseUnitScript>();
            SpawnUnit(enemyHeroPrefab, new Vector2Int(width / 2, enemyY), BaseUnitScript.Team.Enemy);
        }

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
