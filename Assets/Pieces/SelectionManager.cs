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
    public List<Tile> tileOptions;

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
                PieceInfoUI.instance.ShowTargetInfo(tile.currentPiece);
                UIButtons.Instance.SetButtonsVisibility(
                    showMove: false,
                    showAttack: false,
                    showSpecialAttack: false,
                    showCancel: false,
                    showEndTurn: true
                );
            }
        }
    }

    public void SetAction(PlayerAction action)
    {
        currentAction = action;
        targetTile = null;
        Debug.Log($"Action courante définie : {currentAction}");

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
                    UIButtons.Instance.SetButtonsVisibility(
                    showMove: false,
                    showAttack: false,
                    showSpecialAttack: false,
                    showCancel: true,
                    showEndTurn: true
                );
                    break;
                case PlayerAction.Attack:
                    piece.ShowAttackOptions();
                    break;
                case PlayerAction.SpecialAttack:
                    piece.ShowSpecialAttackOptions();
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
                piece.Attack(targetTile.currentPiece);
                break;
            case PlayerAction.SpecialAttack:
                piece.SpecialAttack(targetTile.currentPiece);
                break;
        }
    }

    public void ClearSelection()
    {
        selectedTile = null;
        targetTile = null;
        currentAction = PlayerAction.None;
        Debug.Log("Sélection et action réinitialisées.");
        if (tileOptions != null)
        {
            foreach (Tile tile in tileOptions)
            {
                tile.SetHighlightActive(false);
            }
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
        pieceToMove.SetCurrentEnergy(pieceToMove.GetCurrentEnergy() - 1);
        TurnManager.Instance.SpendPM();

        UIButtons.Instance.SetButtonsVisibility(
            showMove: false, showAttack: false, showSpecialAttack: false, showCancel: false, showEndTurn: true
        );
        PieceInfoUI.instance.ShowInfo(null);
        
        ClearSelection();
    }


}
