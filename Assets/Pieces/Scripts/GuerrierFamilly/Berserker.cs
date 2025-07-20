using UnityEngine;
using System.Collections.Generic;

public class BerserkerScript : BaseUnitScript, ISpecialActionCommand
{
    private Tile selectedTileForSpecial;

    public void PrepareAction()
    {
        if (currentEnergy < maxEnergy)
        {
            Debug.Log($"{unitName} n'a pas assez d'énergie pour préparer l'attaque spéciale.");
            return;
        }

        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
        Vector2Int origin = GetCurrentTilePosition();
        selectedTileForSpecial = board[origin.x, origin.y];

        selectedTileForSpecial.SetHighlight(Color.yellow);
        SelectionManager.Instance.validMoves.Clear();
        SelectionManager.Instance.validMoves.Add(selectedTileForSpecial);

        Debug.Log($"{unitName} prépare son attaque spéciale.");
    }

    public void ExecuteAction()
    {
        if (currentEnergy < maxEnergy)
        {
            Debug.Log($"{unitName} n'a pas assez d'énergie pour lancer l'attaque spéciale.");
            return;
        }

        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
        Vector2Int origin = GetCurrentTilePosition();
        List<Vector2Int> adjacentPositions = GetTilesInRange(origin, 1, board, true);

        int enemiesHit = 0;
        foreach (Vector2Int pos in adjacentPositions)
        {
            Tile tile = board[pos.x, pos.y];
            if (tile.IsOccupied() && tile.currentPiece.team != this.team)
            {
                tile.currentPiece.TakeDamage(attackPower);
                enemiesHit++;
                Debug.Log($"{unitName} inflige {attackPower} dégâts à {tile.currentPiece.unitName}");
            }
        }

        Heal(enemiesHit);
        UseEnergy(maxEnergy);
        Debug.Log($"{unitName} a frappé {enemiesHit} ennemis avec son attaque spéciale.");
    }

    private void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"{unitName} récupère {amount} PV. Santé actuelle : {currentHealth}/{maxHealth}");
    }
}
