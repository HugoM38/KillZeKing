using UnityEngine;
using System.Collections.Generic;


public class BaseUnitScript : MonoBehaviour
{
    public enum Team { Player, Enemy }
    public Team team;

    public enum TileFilter { Self, Empty, Ally, Enemy }


    [Header("Stats de Base")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attackDamage;

    [SerializeField] private int movementRange = 1;
    [SerializeField] private int attackRange = 1;

    [SerializeField] private int maxEnergy = 4;
    [SerializeField] private int currentEnergy = 1;

    private void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = 1;

        TextMesh text = GetComponentInChildren<TextMesh>();
        if (text != null)
        {
            text.color = team == Team.Player ? Color.white : Color.black;
            text.text = name;
        }
    }

    #region Getters / Setters

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    public void SetCurrentHealth(int value) => currentHealth = Mathf.Clamp(value, 0, maxHealth);

    public int GetAttackDamage() => attackDamage;
    public void SetAttackDamage(int value) => attackDamage = value;

    public int GetMovementRange() => movementRange;
    public void SetMovementRange(int value) => movementRange = value;

    public int GetAttackRange() => attackRange;
    public void SetAttackRange(int value) => attackRange = value;

    public int GetMaxEnergy() => maxEnergy;
    public int GetCurrentEnergy() => currentEnergy;
    public void SetCurrentEnergy(int value) => currentEnergy = Mathf.Clamp(value, 0, maxEnergy);

    #endregion

    #region Actions / Visualisations

    public List<Tile> ShowMoveOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[ShowMoveOptions] Aucun selectedTile défini dans SelectionManager.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();

        List<Tile> tilesInRange = GetTilesInRange(originTile, movementRange, board);
        List<Tile> emptyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Empty);
        foreach (Tile tile in emptyTiles)
        {
            tile.SetHighlight(Color.blue);
        }

        Debug.Log($"{name} a {emptyTiles.Count} cases vides accessibles pour le déplacement.");
        return emptyTiles;
    }


    public virtual void ShowAttackOptions()
    {
        Debug.Log($"{name} : Affiche les options d'attaque.");
        // Logique de highlight à implémenter plus tard
    }

    public virtual void ShowSpecialAttackOptions()
    {
        Debug.Log($"{name} : Affiche les options d'attaque spéciale.");
        // Logique de highlight à implémenter plus tard
    }

    public virtual void Attack(BaseUnitScript target)
    {
        Debug.Log($"{name} attaque {target.name}.");
        // Logique d'attaque à implémenter
    }

    public virtual void SpecialAttack(BaseUnitScript target)
    {
        Debug.Log($"{name} utilise une attaque spéciale sur {target.name}.");
        // Logique d'attaque spéciale à implémenter
    }

    public List<Tile> GetTilesInRange(Tile originTile, int range, Tile[,] board)
    {
        List<Tile> result = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<(Tile tile, int dist)> queue = new Queue<(Tile, int)>();

        visited.Add(originTile);
        queue.Enqueue((originTile, 0));

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        int boardWidth = board.GetLength(0);
        int boardHeight = board.GetLength(1);

        while (queue.Count > 0)
        {
            var (currentTile, dist) = queue.Dequeue();

            if (dist != 0)
                result.Add(currentTile);

            if (dist >= range)
                continue;

            Vector2Int currentPos = currentTile.coordinates;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int nextPos = currentPos + dir;

                if (nextPos.x < 0 || nextPos.x >= boardWidth || nextPos.y < 0 || nextPos.y >= boardHeight)
                    continue;

                Tile nextTile = board[nextPos.x, nextPos.y];

                if (visited.Contains(nextTile))
                    continue;

                visited.Add(nextTile);

                if (nextTile.IsOccupied())
                    continue; // obstacle

                queue.Enqueue((nextTile, dist + 1));
            }
        }

        return result;
    }

    public List<Tile> FilterTiles(List<Tile> tiles, Tile originTile, TileFilter filter)
    {
        List<Tile> filteredTiles = new List<Tile>();

        foreach (Tile tile in tiles)
        {
            switch (filter)
            {
                case TileFilter.Self:
                    if (tile == originTile)
                        filteredTiles.Add(tile);
                    break;

                case TileFilter.Empty:
                    if (!tile.IsOccupied())
                        filteredTiles.Add(tile);
                    break;

                case TileFilter.Ally:
                    if (tile.IsOccupied() && tile.currentPiece.team == this.team)
                        filteredTiles.Add(tile);
                    break;

                case TileFilter.Enemy:
                    if (tile.IsOccupied() && tile.currentPiece.team != this.team)
                        filteredTiles.Add(tile);
                    break;
            }
        }

        return filteredTiles;
    }
    #endregion
}
