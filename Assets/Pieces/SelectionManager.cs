using UnityEngine;
using System.Collections.Generic;

public enum PlayerAction
{
    None,
    Move,
    Attack,
    SpecialAttack
}

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public Tile selectedTile;
    public Tile targetTile;
    public PlayerAction currentAction = PlayerAction.None;
    public List<Tile> tileOptions; // Cases jaunes
    private List<Tile> temporaryHighlights = new List<Tile>(); // Cases violettes

    [HideInInspector]
    public Tile hoveredTile;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnTileSelected(Tile tile)
    {
        if (currentAction == PlayerAction.None)
        {
            targetTile = null;

            if (tile.currentPiece != null && tile.currentPiece.team == TurnManager.Instance.currentPlayer)
            {
                selectedTile = tile;
                PieceInfoUI.instance.ShowInfo(tile.currentPiece);

                var piece = tile.currentPiece;
                var stats = TurnManager.Instance.CurrentStats;

                bool canMove = stats.pm > 0;
                bool canAttack = stats.pa > 0 && piece.GetCurrentEnergy() > 0;
                bool canSpecialAttack = stats.pa > 0 && piece.GetCurrentEnergy() == piece.GetMaxEnergy();

                UIButtons.Instance.SetButtonsVisibility(
                    showMove: canMove,
                    showAttack: canAttack,
                    showSpecialAttack: canSpecialAttack,
                    showCancel: false,
                    showEndTurn: true
                );
            }
            else
            {
                UIButtons.Instance.SetButtonsVisibility(
                    showMove: false,
                    showAttack: false,
                    showSpecialAttack: false,
                    showCancel: false,
                    showEndTurn: true
                );
            }
        }
        else
        {
            targetTile = tile;
            ExecuteCurrentAction();
        }
    }

    public void SetAction(PlayerAction action)
    {
        // 1. Nettoyage des anciennes surbrillances
        ClearTemporaryHighlights();
        if (tileOptions != null)
        {
            foreach (var t in tileOptions)
                t.SetHighlightActive(false);
            tileOptions = null;
        }

        // 2. Mise à jour de l'action
        currentAction = action;
        targetTile = null;
        Debug.Log($"Action courante définie : {currentAction}");

        // 3. Remplissage et surbrillance selon l'action
        if (selectedTile != null && selectedTile.currentPiece != null)
        {
            var piece = selectedTile.currentPiece;
            switch (currentAction)
            {
                case PlayerAction.None:
                    ClearSelection();
                    break;

                case PlayerAction.Move:
                    tileOptions = piece.ShowMoveOptions();
                    ShowAbilityTargets();
                    UIButtons.Instance.SetButtonsVisibility(false, false, false, true, true);
                    break;

                case PlayerAction.Attack:
                    tileOptions = piece.ShowAttackOptions();
                    ShowAbilityTargets();
                    UIButtons.Instance.SetButtonsVisibility(false, false, false, true, true);
                    break;

                case PlayerAction.SpecialAttack:
                    tileOptions = piece.ShowSpecialAttackOptions();
                    ShowAbilityTargets();
                    UIButtons.Instance.SetButtonsVisibility(false, false, false, true, true);
                    break;
            }
        }
    }

    private void ExecuteCurrentAction()
    {
        if (selectedTile == null || targetTile == null)
        {
            Debug.LogWarning("Sélection ou cible invalide pour exécuter l'action.");
            return;
        }

        var piece = selectedTile.currentPiece;
        if (piece == null)
        {
            Debug.LogWarning("Aucune pièce sélectionnée pour exécuter l'action.");
            return;
        }

        switch (currentAction)
        {
            case PlayerAction.Move:
                MoveTo();
                break;
            case PlayerAction.Attack:
                AttackTarget();
                break;
            case PlayerAction.SpecialAttack:
                if (tileOptions == null || !tileOptions.Contains(targetTile))
                {
                    Debug.LogWarning("[SelectionManager] SpecialAttack échoué : la case ciblée n'est pas valide.");
                    return;
                }
                piece.SpecialAttack(targetTile.currentPiece);
                ClearTemporaryHighlights();
                ClearSelection();
                break;
        }
    }

    public void MoveTo()
    {
        if (selectedTile == null || selectedTile.currentPiece == null || targetTile == null)
        {
            Debug.LogWarning("[SelectionManager] MoveTo échoué : prérequis non remplis.");
            return;
        }

        if (tileOptions == null || !tileOptions.Contains(targetTile))
        {
            Debug.LogWarning("[SelectionManager] MoveTo échoué : la case ciblée n'est pas valide.");
            return;
        }

        BaseUnitScript pieceToMove = selectedTile.currentPiece;
        pieceToMove.transform.position = targetTile.transform.position;

        targetTile.SetPiece(pieceToMove);
        selectedTile.SetPiece(null);

        TurnManager.Instance.SpendPM();

        UIButtons.Instance.SetButtonsVisibility(false, false, false, false, true);
        PieceInfoUI.instance.ShowInfo(null);
        ClearSelection();
    }

    public void AttackTarget()
    {
        if (selectedTile == null || selectedTile.currentPiece == null || targetTile == null)
        {
            Debug.LogWarning("[SelectionManager] AttackTarget échoué : prérequis non remplis.");
            return;
        }

        if (tileOptions == null || !tileOptions.Contains(targetTile))
        {
            Debug.LogWarning("[SelectionManager] AttackTarget échoué : la case ciblée n'est pas valide.");
            return;
        }

        BaseUnitScript attacker = selectedTile.currentPiece;
        BaseUnitScript target = targetTile.currentPiece;

        if (target == null)
        {
            Debug.LogWarning("[SelectionManager] AttackTarget échoué : aucune pièce sur la cible.");
            return;
        }

        bool targetDied = attacker.Attack(target);

        if (targetDied && attacker.GetAttackRange() == 1)
        {
            Debug.Log($"{attacker.name} avance sur la case de la cible vaincue.");
            attacker.transform.position = targetTile.transform.position;
            targetTile.SetPiece(attacker);
            selectedTile.SetPiece(null);
        }

        attacker.SetCurrentEnergy(attacker.GetCurrentEnergy() - 1);
        TurnManager.Instance.SpendPA();

        UIButtons.Instance.SetButtonsVisibility(false, false, false, false, true);
        PieceInfoUI.instance.ShowInfo(null);
        ClearSelection();
    }

    public void OnTileHovered(Tile tile)
    {
        hoveredTile = tile;

        if (currentAction == PlayerAction.SpecialAttack)
        {
            if (tileOptions != null && tileOptions.Contains(tile))
            {
                var piece = selectedTile.currentPiece;
                if (piece != null)
                {
                    List<Tile> area = piece.GetSpecialAttackArea(tile);
                    ShowEffectArea(area);
                }
            }
            else
            {
                ShowAbilityTargets();
            }
        }
        else if (currentAction == PlayerAction.Attack)
        {
            ShowAbilityTargets();
        }

        if (tile.currentPiece != null)
            PieceInfoUI.instance.ShowTargetInfo(tile.currentPiece);
        else
            PieceInfoUI.instance.ShowTargetInfo(null);
    }

    public void OnTileUnhovered(Tile tile)
    {
        if (hoveredTile == tile) hoveredTile = null;
        if (currentAction == PlayerAction.SpecialAttack || currentAction == PlayerAction.Attack)
        {
            ShowAbilityTargets();
        }
    }

    private void ShowEffectArea(List<Tile> area)
    {
        ClearTemporaryHighlights();

        foreach (Tile tile in area)
        {
            tile.SetHighlight(Color.magenta);
            temporaryHighlights.Add(tile);
        }
    }

    private void ShowAbilityTargets()
    {
        ClearTemporaryHighlights();

        if (tileOptions != null)
        {
            foreach (Tile tile in tileOptions)
            {
                tile.SetHighlight(Color.yellow);
                tile.SetHighlightActive(true);
            }
        }
    }

    private void ClearTemporaryHighlights()
    {
        foreach (Tile tile in temporaryHighlights)
        {
            tile.SetHighlightActive(false);
        }
        temporaryHighlights.Clear();
    }

    public void ClearSelection()
    {
        selectedTile = null;
        targetTile = null;
        currentAction = PlayerAction.None;
        Debug.Log("Sélection et action réinitialisées.");

        ClearTemporaryHighlights();

        if (tileOptions != null)
        {
            foreach (Tile tile in tileOptions)
            {
                tile.SetHighlightActive(false);
            }
            tileOptions = null;
        }
    }
}
