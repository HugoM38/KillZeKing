using UnityEngine;
using TMPro;

public class PieceInfoUI : MonoBehaviour
{
    public static PieceInfoUI instance;

    public TextMeshProUGUI infoText;
    public TextMeshProUGUI turnText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(true); // 🔒 Forcer actif (même si désactivé dans la scène)
        ShowNoSelection();
    }

    public void ShowInfo(BaseUnitScript piece)
    {
        if (piece == null)
        {
            ShowNoSelection();
            return;
        }

        infoText.text =
            $"{piece.name} ({piece.team})\n" +
            $"PV : {piece.currentHealth} / {piece.maxHealth}\n" +
            $"ATK : {piece.attackDamage}\n" +
            $"Énergie : {piece.currentEnergy} / {piece.maxEnergy}";
    }

    public void ShowNoSelection()
    {
        infoText.text = "Aucune pièce sélectionnée";
    }

    public void UpdateTurnDisplay(BaseUnitScript.Team currentPlayer, int pa, int maxPA, int pm, int maxPM)
    {
        if (turnText != null)
        {
            string teamName = currentPlayer == BaseUnitScript.Team.Player ? "Joueur" : "Ennemi";
            turnText.text = $"Tour : {teamName} | PA : {pa}/{maxPA} | PM : {pm}/{maxPM}";
        }
    }

    public TextMeshProUGUI targetInfoText;

    public void ShowTargetInfo(BaseUnitScript target)
    {
        if (target == null)
        {
            targetInfoText.text = "";
            return;
        }

        targetInfoText.text =
            $"<b>Ennemi : {target.name}</b>\n" +
            $"PV : {target.currentHealth} / {target.maxHealth}\n" +
            $"ATK : {target.attackDamage}\n" +
            $"Énergie : {target.currentEnergy} / {target.maxEnergy}";
    }

}
