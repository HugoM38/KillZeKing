using UnityEngine;
using System.Collections.Generic;

public class UIButtons : MonoBehaviour
{
    public static UIButtons Instance;

    public GameObject actionButton;
    public GameObject cancelButton;
    public GameObject attackButton;
    public GameObject specialAttackButton;
    public GameObject endTurnButton;

    private void Awake()
    {
        Instance = this;
        RefreshButtons(showAction: false, showCancel: false, showAttackOptions: false);
        if (endTurnButton != null) endTurnButton.SetActive(true);
        if (actionButton != null) actionButton.SetActive(false);
        if (cancelButton != null) cancelButton.SetActive(false);
        if (attackButton != null) attackButton.SetActive(false);
        if (specialAttackButton != null) specialAttackButton.SetActive(false);
    }

    public void OnClickAction()
    {
        SelectionManager.Instance.PrepareActionSelection();
        RefreshButtons(showAction: false, showCancel: true, showAttackOptions: false);
    }

    public void OnClickCancel()
    {
        SelectionManager.Instance.ResetSelection();
        RefreshButtons(showAction: false, showCancel: false, showAttackOptions: false);
    }

    public void ShowAttackOptions()
    {
        RefreshButtons(showAction: false, showCancel: true, showAttackOptions: true);
    }

    public void OnClickAttack()
    {
        SelectionManager.Instance.PerformAttack();
        RefreshButtons(showAction: true, showCancel: false, showAttackOptions: false);
    }

    public void OnClickSpecialAttack()
    {
        SelectionManager.Instance.PerformSpecialAttack();
        RefreshButtons(showAction: true, showCancel: false, showAttackOptions: false);
    }

    public void OnClickEndTurn()
    {
        TurnManager.Instance.NextTurn();
        var tm = TurnManager.Instance;
        var stats = tm.CurrentStats;
        PieceInfoUI.instance.UpdateTurnDisplay(tm.currentPlayer, stats.pa, stats.maxPA, stats.pm, stats.maxPM);

        RefreshButtons(showAction: false, showCancel: false, showAttackOptions: false);
    }

    public void RefreshButtons(bool showAction, bool showCancel, bool showAttackOptions)
    {
        actionButton.SetActive(showAction);
        cancelButton.SetActive(showCancel);
        attackButton.SetActive(showAttackOptions);
        specialAttackButton.SetActive(showAttackOptions);
        Debug.Log("TEST");
    }
}
