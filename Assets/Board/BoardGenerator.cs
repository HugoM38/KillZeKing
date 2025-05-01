using UnityEngine;
using System.Collections;

public class BoardGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 8;
    public int height = 8;
    public float tileSpacing = 1.1f;

    private Tile[,] tiles;

    public ChessPiece PawnWhitePrefab;
    public ChessPiece PawnBlackPrefab;
    public ChessPiece RookWhitePrefab;
    public ChessPiece RookBlackPrefab;
    public ChessPiece KnightWhitePrefab;
    public ChessPiece KnightBlackPrefab;
    public ChessPiece BishopWhitePrefab;
    public ChessPiece BishopBlackPrefab;
    public ChessPiece QueenWhitePrefab;
    public ChessPiece QueenBlackPrefab;
    public ChessPiece KingWhitePrefab;
    public ChessPiece KingBlackPrefab;


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

        for (int x = 0; x < width; x++)
        {
            Tile whitePawnTile = tiles[x, 1];
            ChessPiece whitePawn = Instantiate(PawnWhitePrefab, whitePawnTile.transform.position, Quaternion.identity, whitePawnTile.transform);
            whitePawnTile.currentPiece = whitePawn;

            Tile blackPawnTile = tiles[x, height - 2];
            ChessPiece blackPawn = Instantiate(PawnBlackPrefab, blackPawnTile.transform.position, Quaternion.identity, blackPawnTile.transform);
            blackPawnTile.currentPiece = blackPawn;
        }

        PlaceMajorPieces(0, true);
        PlaceMajorPieces(height - 1, false);
    }

    void PlaceMajorPieces(int y, bool isWhite)
    {
        ChessPiece rookPrefab = isWhite ? RookWhitePrefab : RookBlackPrefab;
        ChessPiece knightPrefab = isWhite ? KnightWhitePrefab : KnightBlackPrefab;
        ChessPiece bishopPrefab = isWhite ? BishopWhitePrefab : BishopBlackPrefab;
        ChessPiece queenPrefab = isWhite ? QueenWhitePrefab : QueenBlackPrefab;
        ChessPiece kingPrefab = isWhite ? KingWhitePrefab : KingBlackPrefab;

        // Placement classique :
        int widthCenter = width / 2;

        // Tours
        SpawnPiece(rookPrefab, 0, y);
        SpawnPiece(rookPrefab, width - 1, y);

        // Cavaliers
        SpawnPiece(knightPrefab, 1, y);
        SpawnPiece(knightPrefab, width - 2, y);

        // Fous
        SpawnPiece(bishopPrefab, 2, y);
        SpawnPiece(bishopPrefab, width - 3, y);

        // Reine
        SpawnPiece(queenPrefab, 3, y);

        // Roi
        SpawnPiece(kingPrefab, 4, y);
        print("spawned pieces");
    }

    void SpawnPiece(ChessPiece prefab, int x, int y)
    {
        if (prefab == null)
        {
            Debug.LogError($"[BUG] Prefab manquant à la position {x},{y}");
            return;
        }

        if (tiles[x, y] == null)
        {
            Debug.LogError($"[BUG] Tile non générée à la position {x},{y} — SetupPieces() appelé trop tôt ?");
            return;
        }

        Tile tile = tiles[x, y];

        ChessPiece piece = Instantiate(prefab, tile.transform.position, Quaternion.identity, tile.transform);
        tile.currentPiece = piece;

        Debug.Log($"[OK] {piece.name} placé en ({x},{y})");
    }



}
