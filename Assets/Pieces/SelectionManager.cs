using System.Collections.Generic;
using UnityEngine;

public enum PlayerActionState
{
    None,
    ActionSelected
}

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public BaseUnitScript selectedPiece;
    public Tile selectedTile;
    public List<Tile> validMoves = new List<Tile>();
    public PlayerActionState currentState = PlayerActionState.None;
    public bool targetSelected = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectPiece(BaseUnitScript piece, Tile[,] board)
    {
        selectedPiece = piece;
        selectedTile = null;
        currentState = PlayerActionState.None;
        validMoves.Clear();

        foreach (Tile tile in board)
        {
            if (tile.currentPiece == piece)
            {
                selectedTile = tile;
                break;
            }
        }

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.instance.ShowInfo(piece);

        bool isOwnPiece = piece.team == TurnManager.Instance.currentPlayer;
        if (isOwnPiece)
            UIButtons.Instance.RefreshButtons(showAction: true, showCancel: false, showAttackOptions: false);
    }

    public void ResetSelection()
    {
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
        targetSelected = false;
        selectedPiece = null;
        selectedTile = null;
        currentState = PlayerActionState.None;
        validMoves.Clear();

        foreach (Tile tile in board)
            tile.ClearHighlight();

        PieceInfoUI.instance.ShowNoSelection();
        PieceInfoUI.instance.ShowTargetInfo(null);

        UIButtons.Instance.RefreshButtons(showAction: false, showCancel: false, showAttackOptions: false);
    }

    public void PerformAttack()
    {
        if (selectedPiece == null || selectedTile == null)
        {
            Debug.LogWarning("Aucune pièce ou case sélectionnée pour l'attaque.");
            return;
        }

        BaseUnitScript targetPiece = selectedTile.currentPiece;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        if (targetPiece != null && targetPiece.team != selectedPiece.team)
        {
            int previousHP = targetPiece.currentHealth;

            targetPiece.TakeDamage(selectedPiece.attackDamage);
            selectedPiece.UseEnergy(1);
            TurnManager.Instance.SpendPA();

            Debug.Log($"{selectedPiece.name} attaque {targetPiece.name} et inflige {selectedPiece.attackDamage} dégâts.");

            if (targetPiece.currentHealth <= 0 && previousHP > 0)
            {
                Debug.Log($"{selectedPiece.name} a tué {targetPiece.name} !");
                KillTargetAndMove(board);
            }

            PieceInfoUI.instance.ShowTargetInfo(null);
            ResetSelection();
        }
    }

    public void PerformSpecialAttack()
    {
        if (selectedPiece == null || selectedTile == null)
        {
            Debug.LogWarning("Aucune pièce ou case sélectionnée pour l'attaque spéciale.");
            return;
        }

        BaseUnitScript targetPiece = selectedTile.currentPiece;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();

        if (targetPiece != null && targetPiece.team != selectedPiece.team)
        {
            int previousHP = targetPiece.currentHealth;

            selectedPiece.UseEnergy(selectedPiece.maxEnergy);
            selectedPiece.SpecialAbility(targetPiece);
            TurnManager.Instance.SpendPA();

            if (targetPiece.currentHealth <= 0 && previousHP > 0)
            {
                Debug.Log($"{selectedPiece.name} a tué {targetPiece.name} avec une attaque spéciale !");
                KillTargetAndMove(board);
            }

            Debug.Log($"Attaque spéciale sur {targetPiece.name}");
            PieceInfoUI.instance.ShowTargetInfo(null);
            ResetSelection();
        }
    }

    private void KillTargetAndMove(Tile[,] board)
    {
        Tile attackerTile = null;
        Tile targetTile = selectedTile;

        foreach (Tile tile in board)
        {
            if (tile.currentPiece == selectedPiece)
            {
                attackerTile = tile;
                break;
            }
        }

        if (attackerTile == null)
        {
            Debug.LogError("Attacker tile introuvable !");
            return;
        }

        BaseUnitScript targetPiece = targetTile.currentPiece;
        if (targetPiece != null)
        {
            Destroy(targetPiece.gameObject);
        }

        targetTile.MovePieceFrom(attackerTile);

        Debug.Log($"{selectedPiece.name} s'est déplacé de {attackerTile.coordinates} à {targetTile.coordinates}.");
    }


    public void PrepareActionSelection()
    {
        if (selectedPiece == null)
        {
            Debug.LogWarning("[SelectionManager] Aucun pion sélectionné.");
            return;
        }

        currentState = PlayerActionState.ActionSelected;
        Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
        Vector2Int origin = selectedPiece.GetCurrentTilePosition();

        validMoves.Clear();

        foreach (Tile tile in board)
            tile.ClearHighlight();

        Tile selfTile = board[origin.x, origin.y];
        selfTile.SetHighlight(Color.yellow);

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacentPos = origin + dir;
            if (adjacentPos.x >= 0 && adjacentPos.x < board.GetLength(0) &&
                adjacentPos.y >= 0 && adjacentPos.y < board.GetLength(1))
            {
                Tile adjacentTile = board[adjacentPos.x, adjacentPos.y];
                if (adjacentTile.IsOccupied() && adjacentTile.currentPiece.team == selectedPiece.team)
                {
                    adjacentTile.SetHighlight(Color.green);
                    validMoves.Add(adjacentTile);
                }
            }
        }

        if (TurnManager.Instance.HasEnoughPM())
        {
            var movePositions = selectedPiece.GetAvailableMoves(origin, board);

            foreach (var pos in movePositions)
            {
                Tile tile = board[pos.x, pos.y];
                if (!tile.IsOccupied())
                {
                    tile.SetHighlight(Color.blue);
                    validMoves.Add(tile);
                }
            }
        }

        if (TurnManager.Instance.HasEnoughPA() && selectedPiece.currentEnergy > 0)
        {
            var attackPositions = selectedPiece.GetAttackableTiles(origin, board);

            foreach (var pos in attackPositions)
            {
                Tile tile = board[pos.x, pos.y];
                if (tile.IsOccupied() && tile.currentPiece.team != selectedPiece.team)
                {
                    tile.SetHighlight(Color.red);
                    validMoves.Add(tile);
                }
            }
        }
    }
}
