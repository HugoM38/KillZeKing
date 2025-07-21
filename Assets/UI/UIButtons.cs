using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public static UIButtons Instance;

    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject specialAttackButton;
    public GameObject cancelButton;
    public GameObject endTurnButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        SetButtonsVisibility(
            showMove: false,
            showAttack: false,
            showSpecialAttack: false,
            showCancel: false,
            showEndTurn: true
        );
    }

    public void OnClickMove()
    {
        SelectionManager.Instance.SetAction(PlayerAction.Move);
    }

    public void OnClickAttack()
    {
        SelectionManager.Instance.SetAction(PlayerAction.Attack);
    }

    public void OnClickSpecialAttack()
    {
        SelectionManager.Instance.SetAction(PlayerAction.SpecialAttack);
    }

    public void OnClickCancel()
    {
        SelectionManager.Instance.ClearSelection();
        SelectionManager.Instance.SetAction(PlayerAction.None);
        SetButtonsVisibility(
            showMove: false,
            showAttack: false,
            showSpecialAttack: false,
            showCancel: false,
            showEndTurn: true
        );
    }

    public void OnClickEndTurn()
    {
        SelectionManager.Instance.ClearSelection();
        TurnManager.Instance.NextTurn();

        var tm = TurnManager.Instance;
        var stats = tm.CurrentStats;
        PieceInfoUI.instance.UpdateTurnDisplay(tm.currentPlayer, stats.pa, stats.maxPA, stats.pm, stats.maxPM);

        SetButtonsVisibility(
            showMove: false,
            showAttack: false,
            showSpecialAttack: false,
            showCancel: false,
            showEndTurn: true
        );
    }

    public void SetButtonsVisibility(bool showMove, bool showAttack, bool showSpecialAttack, bool showCancel, bool showEndTurn)
    {
        SetButtonActive(moveButton, showMove);
        SetButtonActive(attackButton, showAttack);
        SetButtonActive(specialAttackButton, showSpecialAttack);
        SetButtonActive(cancelButton, showCancel);
        SetButtonActive(endTurnButton, showEndTurn);
    }

    private void SetButtonActive(GameObject button, bool isActive)
    {
        if (button != null)
            button.SetActive(isActive);
    }
}
