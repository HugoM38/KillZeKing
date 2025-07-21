using UnityEngine;
using TMPro;

public class PieceInfoUI : MonoBehaviour
{
    public static PieceInfoUI instance;

    public TextMeshProUGUI infoText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI targetInfoText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(true);
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
            $"PV : {piece.GetCurrentHealth()} / {piece.GetMaxHealth()}\n" +
            $"ATK : {piece.GetAttackDamage()}\n" +
            $"Énergie : {piece.GetCurrentEnergy()} / {piece.GetMaxEnergy()}";
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

    public void ShowTargetInfo(BaseUnitScript target)
    {
        if (target == null)
        {
            targetInfoText.text = "";
            return;
        }

        targetInfoText.text =
            $"<b>Ennemi : {target.name}</b>\n" +
            $"PV : {target.GetCurrentHealth()} / {target.GetMaxHealth()}\n" +
            $"ATK : {target.GetAttackDamage()}\n" +
            $"Énergie : {target.GetCurrentEnergy()} / {target.GetMaxEnergy()}";
    }
}
