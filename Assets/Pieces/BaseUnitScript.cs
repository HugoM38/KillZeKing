using UnityEngine;
using System.Collections.Generic;

public class BaseUnitScript : MonoBehaviour
{
    public enum Team { Player, Enemy }
    public Team team;

    public int maxHealth;
    public int currentHealth;
    public int attackDamage;

    public int movementRange = 1;
    public int attackRange = 1;

    public int maxEnergy = 4;
    public int currentEnergy = 1;

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

    public virtual List<Vector2Int> GetAvailableMoves(Vector2Int origin, Tile[,] board)
    {
        return GetTilesInRange(origin, movementRange, board, isAttack: false);
    }

    public virtual List<Vector2Int> GetAttackableTiles(Vector2Int origin, Tile[,] board)
    {
        return GetTilesInRange(origin, attackRange, board, isAttack: true);
    }

    private List<Vector2Int> GetTilesInRange(Vector2Int origin, int range, Tile[,] board, bool isAttack)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<(Vector2Int pos, int dist)> queue = new Queue<(Vector2Int, int)>();

        visited.Add(origin);
        queue.Enqueue((origin, 0));

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
            var (current, dist) = queue.Dequeue();

            if (dist != 0)
                result.Add(current);

            if (dist >= range)
                continue;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (next.x < 0 || next.x >= boardWidth || next.y < 0 || next.y >= boardHeight)
                    continue;

                if (visited.Contains(next))
                    continue;

                visited.Add(next);
                Tile tile = board[next.x, next.y];

                if (tile.IsOccupied())
                {
                    if (isAttack && tile.currentPiece.team != this.team)
                    {
                        result.Add(next);
                    }
                    continue;
                }

                queue.Enqueue((next, dist + 1));
            }
        }

        return result;
    }

    public Vector2Int GetCurrentTilePosition()
    {
        if (SelectionManager.Instance.selectedPiece == this)
        {
            if (SelectionManager.Instance.selectedTile != null)
                return SelectionManager.Instance.selectedTile.coordinates;
            else
                Debug.LogWarning("[BaseUnitScript] Aucun tile sélectionné pour la pièce.");
        }

        Debug.LogWarning("[BaseUnitScript] GetCurrentTilePosition() appelée sur une pièce non sélectionnée.");
        return new Vector2Int(-1, -1);
    }


    public bool TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void GainEnergy()
    {
        currentEnergy = Mathf.Min(currentEnergy + 1, maxEnergy);
    }

    public bool HasEnoughEnergy()
    {
        return currentEnergy > 0;
    }

    public void UseEnergy(int amount)
    {
        currentEnergy = Mathf.Max(0, currentEnergy - amount);
    }

    public void RechargerEnergie()
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + 1);
        Debug.Log($"[Énergie] {name} : {currentEnergy}/{maxEnergy}");
    }

    public virtual void SpecialAbility(BaseUnitScript target)
    {
        Debug.Log($"{name} utilise sa compétence spéciale sur {target.name} (par défaut, aucun effet).");
    }

}
