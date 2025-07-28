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

    public GameObject statChangePopupPrefab;


    void OnMouseDown()
    {
        // 1) Y a-t-il un sort en attente ?
        if (SpellManager.Instance != null
         && SpellManager.Instance.pendingSpellValue > 0)
        {
            string spell = SpellManager.Instance.pendingSpellName;
            int val = SpellManager.Instance.pendingSpellValue;

            switch (spell)
            {
                case "fireball_0":
                    // inflige val dégâts
                    SetCurrentHealth(GetCurrentHealth() - val);
                    Debug.Log($"{name} subit {val} dégâts de Boule de Feu");
                    break;

                case "heal":
                    // soigne val points
                    SetCurrentHealth(GetCurrentHealth() + val);
                    Debug.Log($"{name} est soigné de {val} PV");
                    break;

                case "shield":
                    // par exemple : ajoute val à maxHealth
                    SetMaxHealth(GetMaxHealth() + val);
                    Debug.Log($"{name} gagne {val} de Bouclier (PV max)");
                    break;

                default:
                    Debug.LogWarning($"Sort inconnu : {spell}");
                    break;
            }

            // 2) on vide le sort et on stoppe là
            SpellManager.Instance.ClearSpell();
            return;
        }

        // 3) sinon, ta logique normale de sélection
        var myTile = GetComponentInParent<Tile>();
        if (myTile != null)
            SelectionManager.Instance.OnTileSelected(myTile);
    }


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
    public void SetMaxHealth(int value)
    {
        int delta = value - maxHealth;
        maxHealth = Mathf.Max(1, value);

        if (delta != 0)
        {
            ShowStatChange($"{(delta > 0 ? "+" : "")}{delta} PV Max", Color.cyan);
        }
    }

    public int GetCurrentHealth() => currentHealth;
    public void SetCurrentHealth(int value)
    {
        int delta = value - currentHealth;
        currentHealth = Mathf.Clamp(value, 0, maxHealth);

        if (delta != 0)
        {
            Color color = delta < 0 ? Color.red : Color.green;
            ShowStatChange($"{(delta > 0 ? "+" : "")}{delta} PV", color);
        }
    }

    public int GetAttackDamage() => attackDamage;
    public void SetAttackDamage(int value)
    {
        int delta = value - attackDamage;
        attackDamage = Mathf.Max(0, value);

        if (delta != 0)
        {
            ShowStatChange($"{(delta > 0 ? "+" : "")}{delta} ATK", Color.yellow);
        }
    }

    public int GetMovementRange() => movementRange;
    public void SetMovementRange(int value)
    {
        int delta = value - movementRange;
        movementRange = Mathf.Max(0, value);

        if (delta != 0)
        {
            ShowStatChange($"{(delta > 0 ? "+" : "")}{delta} Portée Déplacement", Color.gray);
        }
    }

    public int GetAttackRange() => attackRange;
    public void SetAttackRange(int value)
    {
        int delta = value - attackRange;
        attackRange = Mathf.Max(0, value);

        if (delta != 0)
        {
            ShowStatChange($"{(delta > 0 ? "+" : "")}{delta} Portée Attaque", Color.gray);
        }
    }

    public int GetMaxEnergy() => maxEnergy;
    public int GetCurrentEnergy() => currentEnergy;
    public void SetCurrentEnergy(int value) => currentEnergy = Mathf.Clamp(value, 0, maxEnergy);

    #endregion

    #region Actions / Visualisations

    public virtual List<Tile> ShowMoveOptions()
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
        emptyTiles = emptyTiles.FindAll(tile => tile.IsWalkable());
        foreach (Tile tile in emptyTiles)
        {
            tile.SetHighlight(Color.blue);
        }

        Debug.Log($"{name} a {emptyTiles.Count} cases vides accessibles pour le déplacement.");
        return emptyTiles;
    }


    public virtual List<Tile> ShowAttackOptions()
    {
        Tile originTile = SelectionManager.Instance.selectedTile;
        if (originTile == null)
        {
            Debug.LogWarning("[ShowAttackOptions] Aucun selectedTile défini dans SelectionManager.");
            return new List<Tile>();
        }

        Tile[,] board = BoardGenerator.Instance.GetBoard();

        List<Tile> tilesInRange = GetTilesInRange(originTile, attackRange, board);
        List<Tile> enemyTiles = FilterTiles(tilesInRange, originTile, TileFilter.Enemy);

        foreach (Tile tile in enemyTiles)
        {
            tile.SetHighlight(Color.red);
        }

        Debug.Log($"{name} a {enemyTiles.Count} cibles ennemies possibles à portée.");
        return enemyTiles;
    }


    public virtual List<Tile> ShowSpecialAttackOptions()
    {
        Debug.Log($"{name} : Affiche les options d'attaque spéciale.");
        return new List<Tile>();

    }

    public virtual bool Attack(BaseUnitScript target)
    {
        if (target == null)
        {
            Debug.LogWarning($"{name} a tenté d'attaquer une cible invalide.");
            return false;
        }

        Debug.Log($"{name} attaque {target.name} pour {attackDamage} dégâts.");
        target.SetCurrentHealth(target.GetCurrentHealth() - attackDamage);
        Debug.Log($"{target.name} a maintenant {target.GetCurrentHealth()} PV.");

        if (target.GetCurrentHealth() <= 0)
        {
            Debug.Log($"{target.name} est vaincu !");
            Destroy(target.gameObject);
            return true;
        }

        return false;
    }

    public virtual List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        Debug.LogWarning($"{name} : GetSpecialAttackArea() n'est pas défini pour cette unité.");
        return new List<Tile>();
    }

    public virtual void SpecialAttack(BaseUnitScript target)
    {
        Debug.Log($"{name} utilise une attaque spéciale sur {target.name}.");
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

                if (!nextTile.IsWalkable())
                    continue;

                if (nextTile.IsOccupied() && nextTile.currentPiece.team != this.team)
                    continue;

                visited.Add(nextTile);
                queue.Enqueue((nextTile, dist + 1));
            }
        }

        return result;
    }


    public void TakeDamage(int amount)
    {
        SetCurrentHealth(GetCurrentHealth() - amount);
    }

    public List<Tile> FilterTiles(List<Tile> tiles, Tile originTile, TileFilter filter)
    {
        List<Tile> filteredTiles = new List<Tile>();
        Debug.Log("FILTER_TILES");
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
                    if (tile.IsOccupied())
                    {
                        if (tile.currentPiece != null && tile.currentPiece.team == this.team)
                            filteredTiles.Add(tile);
                    }
                    break;

                case TileFilter.Enemy:
                    Debug.Log("ENNEMI");
                    if (tile.IsOccupied())
                    {
                        Debug.Log($"ALED: {tile.currentPiece.team}");
                        if (tile.currentPiece != null && tile.currentPiece.team != this.team)
                            filteredTiles.Add(tile);
                    }
                    break;
            }
        }

        return filteredTiles;
    }

    private void ShowStatChange(string text, Color color)
    {
        if (statChangePopupPrefab == null)
        {
            Debug.LogWarning("statChangePopupPrefab non assigné sur " + name);
            return;
        }

        GameObject popup = Instantiate(statChangePopupPrefab, transform.position + Vector3.up, Quaternion.identity);
        StatChangePopup popupScript = popup.GetComponent<StatChangePopup>();
        if (popupScript != null)
        {
            popupScript.Initialize(text, color);
        }
    }

    #endregion
}
