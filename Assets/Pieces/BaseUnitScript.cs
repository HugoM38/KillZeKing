using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Classe de base pour toutes les unités : gère stats, portée, déplacements, attaques et effets spéciaux.
/// </summary>
public class BaseUnitScript : MonoBehaviour
{
    public enum Team { Player, Enemy }
    public Team team;

    public enum TileFilter { Self, Empty, Ally, Enemy }

    // Effets de statut
    public enum StatusEffect
    {
        Stun,       // Entrave : plus de déplacement
        NoAttack,   // Bouclier anti-attaque : plus d'attaque
        Silence     // Sceau de silence : plus de capacité spéciale
    }
    private Dictionary<StatusEffect,int> statusDurations = new Dictionary<StatusEffect,int>();

    [Header("Stats de Base")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attackDamage;
    [SerializeField] private int movementRange = 1;
    [SerializeField] private int attackRange   = 1;
    [SerializeField] private int maxEnergy     = 4;
    [SerializeField] private int currentEnergy = 1;

    public GameObject statChangePopupPrefab;

    public bool CanMove    => !statusDurations.ContainsKey(StatusEffect.Stun);
    public bool CanAttack  => !statusDurations.ContainsKey(StatusEffect.NoAttack);
    public bool CanSpecial => !statusDurations.ContainsKey(StatusEffect.Silence);

    [Header("Identification de la famille")]
    public UnitFamily Family;


    void Awake() { }

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = 1;
        var text = GetComponentInChildren<TextMesh>();
        if (text != null)
        {
            text.color = team == Team.Player ? Color.white : Color.black;
            text.text  = name;
        }
    }

    void OnMouseDown()
    {
        // application d'un sort en attente
        if (SpellManager.Instance != null && SpellManager.Instance.pendingSpellValue > 0)
        {
            ApplyPendingSpell();
            return;
        }

        // sélection normale
        var myTile = GetComponentInParent<Tile>();
        if (myTile != null)
            SelectionManager.Instance.OnTileSelected(myTile);
    }

    private void ApplyPendingSpell()
    {
        string spell = SpellManager.Instance.pendingSpellName;
        int val      = SpellManager.Instance.pendingSpellValue;
        switch (spell)
        {
            case "fireball_0": SetCurrentHealth(currentHealth - val); break;
            case "heal":       SetCurrentHealth(currentHealth + val); break;
            case "shield":     SetMaxHealth(maxHealth + val);       break;
            default:
                Debug.LogWarning($"Sort inconnu : {spell}");
                break;
        }
        SpellManager.Instance.ClearSpell();
    }

    /// <summary>Applique un effet de statut pour un certain nombre de tours.</summary>
    public void ApplyStatus(StatusEffect effect, int turns)
    {
        statusDurations[effect] = turns;
        ShowStatChange($"«{effect}» appliqué {turns} tour(s)", Color.magenta);
    }

    /// <summary>En fin de tour, décrémente et retire les effets expirés.</summary>
    public void OnTurnEnd()
    {
        var keys = new List<StatusEffect>(statusDurations.Keys);
        foreach (var effect in keys)
        {
            statusDurations[effect]--;
            if (statusDurations[effect] <= 0)
            {
                statusDurations.Remove(effect);
                ShowStatChange($"«{effect}» expiré", Color.gray);
            }
        }
    }

    #region Getters / Setters

    public int GetMaxHealth()    => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    public int GetAttackDamage()  => attackDamage;
    public int GetMovementRange() => movementRange;
    public int GetAttackRange()   => attackRange;
    public int GetMaxEnergy()     => maxEnergy;
    public int GetCurrentEnergy() => currentEnergy;

    public void SetMaxHealth(int v)
    {
        int d = v - maxHealth;
        maxHealth = Mathf.Max(1, v);
        if (d != 0) ShowStatChange($"{(d>0?"+":"")}{d} PV Max", Color.cyan);
    }

    public void SetCurrentHealth(int v)
    {
        int d = v - currentHealth;
        currentHealth = Mathf.Clamp(v, 0, maxHealth);
        if (d != 0) ShowStatChange($"{(d>0?"+":"")}{d} PV", d<0?Color.red:Color.green);
    }

    public void SetAttackDamage(int v)
    {
        int d = v - attackDamage;
        attackDamage = Mathf.Max(0, v);
        if (d != 0) ShowStatChange($"{(d>0?"+":"")}{d} ATK", Color.yellow);
    }

    public void SetMovementRange(int v)
    {
        int d = v - movementRange;
        movementRange = Mathf.Max(0, v);
        if (d != 0) ShowStatChange($"{(d>0?"+":"")}{d} Portée Déplacement", Color.gray);
    }

    public void SetAttackRange(int v)
    {
        int d = v - attackRange;
        attackRange = Mathf.Max(0, v);
        if (d != 0) ShowStatChange($"{(d>0?"+":"")}{d} Portée Attaque", Color.gray);
    }

    public void SetCurrentEnergy(int v)
    {
        currentEnergy = Mathf.Clamp(v, 0, maxEnergy);
    }

    #endregion

    #region Actions / Visualisations

    /// <summary>Cases accessibles pour le déplacement.</summary>
    public virtual List<Tile> ShowMoveOptions()
    {
        if (!CanMove) { Debug.Log($"{name} est entravé."); return new List<Tile>(); }
        var origin = SelectionManager.Instance.selectedTile;
        if (origin == null) { Debug.LogWarning("Aucun selectedTile défini."); return new List<Tile>(); }
        var board = BoardGenerator.Instance.GetBoard();
        var inRange = GetTilesInRange(origin, movementRange, board);
        var empty = FilterTiles(inRange, origin, TileFilter.Empty).FindAll(t => t.IsWalkable());
        empty.ForEach(t => t.SetHighlight(Color.blue));
        return empty;
    }

    /// <summary>Cases cibles pour l’attaque normale.</summary>
    public virtual List<Tile> ShowAttackOptions()
    {
        if (!CanAttack) { Debug.Log($"{name} ne peut pas attaquer."); return new List<Tile>(); }
        var origin = SelectionManager.Instance.selectedTile;
        if (origin == null) { Debug.LogWarning("Aucun selectedTile défini."); return new List<Tile>(); }
        var board = BoardGenerator.Instance.GetBoard();
        var enemies = new List<Tile>();
        var src = origin.coordinates;
        int w = board.GetLength(0), h = board.GetLength(1);
        for (int dx = -attackRange; dx <= attackRange; dx++)
            for (int dy = -attackRange; dy <= attackRange; dy++)
                if (Mathf.Abs(dx) + Mathf.Abs(dy) <= attackRange)
                {
                    int x = src.x + dx, y = src.y + dy;
                    if (x < 0|| x>=w|| y<0|| y>=h) continue;
                    var t = board[x,y];
                    if (t.IsOccupied() && t.currentPiece.team != team)
                    {
                        enemies.Add(t);
                        t.SetHighlight(Color.red);
                    }
                }
        return enemies;
    }

    /// <summary>Cases cibles pour l’attaque spéciale.</summary>
    public virtual List<Tile> ShowSpecialAttackOptions()
    {
        if (!CanSpecial) { Debug.Log($"{name} est silencé."); return new List<Tile>(); }
        // Par défaut, aucune option : les classes enfants override
        return new List<Tile>();
    }

    /// <summary>Zone d’effet autour de la case choisie pour l’attaque spéciale.</summary>
    public virtual List<Tile> GetSpecialAttackArea(Tile targetTile)
    {
        // Par défaut, aucune zone : override dans enfants
        return new List<Tile>();
    }

    /// <summary>Exécute l’attaque spéciale sur la cible.</summary>
    public virtual void SpecialAttack(BaseUnitScript target)
    {
        // Par défaut, rien : override dans enfants
    }

    /// <summary>Attaque normale la cible et retourne vrai si elle meurt.</summary>
    public virtual bool Attack(BaseUnitScript target)
    {
        if (!CanAttack || target == null) return false;
        target.SetCurrentHealth(target.GetCurrentHealth() - attackDamage);
        if (target.GetCurrentHealth() <= 0)
        {
            Destroy(target.gameObject);
            return true;
        }
        return false;
    }

    #endregion

    #region Pathfinding

    /// <summary>Retourne toutes les tuiles à portée (Manhattan) à partir de origin.</summary>
    public List<Tile> GetTilesInRange(Tile origin, int range, Tile[,] board)
    {
        var result = new List<Tile>();
        var visited = new HashSet<Tile> { origin };
        var queue = new Queue<(Tile, int)>();
        queue.Enqueue((origin, 0));
        var dirs = new[]{Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        int w = board.GetLength(0), h = board.GetLength(1);

        while (queue.Count > 0)
        {
            var (tile, dist) = queue.Dequeue();
            if (dist > 0) result.Add(tile);
            if (dist >= range) continue;
            foreach (var d in dirs)
            {
                var np = tile.coordinates + d;
                if (np.x<0||np.x>=w||np.y<0||np.y>=h) continue;
                var next = board[np.x,np.y];
                if (visited.Contains(next)) continue;
                visited.Add(next);
                result.Add(next);
                if (next.IsOccupied() || !next.IsWalkable()) continue;
                queue.Enqueue((next, dist+1));
            }
        }
        return result;
    }

    /// <summary>Filtre les tuiles selon le critère donné.</summary>
    public List<Tile> FilterTiles(List<Tile> tiles, Tile origin, TileFilter filter)
    {
        var list = new List<Tile>();
        foreach (var t in tiles)
        {
            switch (filter)
            {
                case TileFilter.Self:
                    if (t == origin) list.Add(t);
                    break;
                case TileFilter.Empty:
                    if (!t.IsOccupied()) list.Add(t);
                    break;
                case TileFilter.Ally:
                    if (t.IsOccupied() && t.currentPiece.team == team) list.Add(t);
                    break;
                case TileFilter.Enemy:
                    if (t.IsOccupied() && t.currentPiece.team != team) list.Add(t);
                    break;
            }
        }
        return list;
    }

    #endregion

    /// <summary>Réduit la vie, affiche popup.</summary>
    public void TakeDamage(int amount)
    {
        SetCurrentHealth(currentHealth - amount);
    }

    private void ShowStatChange(string text, Color color)
    {
        if (statChangePopupPrefab == null) return;
        var popup = Instantiate(statChangePopupPrefab, transform.position + Vector3.up, Quaternion.identity);
        var script = popup.GetComponent<StatChangePopup>();
        if (script != null) script.Initialize(text, color);
    }
}
