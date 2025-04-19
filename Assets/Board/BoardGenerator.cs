using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 8;
    public int height = 8;
    public float tileSpacing = 1.1f;

    private Tile[,] tiles;

    void Start()
    {
        GenerateBoard();
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
}
