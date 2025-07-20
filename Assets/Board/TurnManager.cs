using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public BaseUnitScript.Team currentPlayer = BaseUnitScript.Team.Player;

    [System.Serializable]
    public class PlayerStats
    {
        public int pa = 1;
        public int pm = 1;
        public int maxPA = 1;
        public int maxPM = 1;
    }

    public PlayerStats playerStats = new PlayerStats();
    public PlayerStats enemyStats = new PlayerStats();

    private int turnCountPlayer = 1;
    private int turnCountEnemy = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public PlayerStats CurrentStats =>
        currentPlayer == BaseUnitScript.Team.Player ? playerStats : enemyStats;

    public void NextTurn()
    {
        currentPlayer = currentPlayer == BaseUnitScript.Team.Player
            ? BaseUnitScript.Team.Enemy
            : BaseUnitScript.Team.Player;

        if (currentPlayer == BaseUnitScript.Team.Player)
            turnCountPlayer++;
        else
            turnCountEnemy++;

        int turnCount = currentPlayer == BaseUnitScript.Team.Player ? turnCountPlayer : turnCountEnemy;
        PlayerStats stats = CurrentStats;

        if (turnCount > 1)
        {
            stats.maxPA = Mathf.Min(stats.maxPA + 1, 5);
            stats.maxPM = Mathf.Min(stats.maxPM + 1, 5);
            RechargerEnergieDesUnites();
        }

        stats.pa = stats.maxPA;
        stats.pm = stats.maxPM;

        Debug.Log($"[TurnManager] Tours - Joueur : {turnCountPlayer}, Ennemi : {turnCountEnemy}");
        Debug.Log($"[TurnManager] {currentPlayer} joue son tour #{turnCount}, maxPA = {stats.maxPA}, maxPM = {stats.maxPM}");

        UpdateUI();
    }

    private void RechargerEnergieDesUnites()
    {
        var allUnits = Object.FindObjectsByType<BaseUnitScript>(FindObjectsSortMode.None);

        foreach (var unit in allUnits)
        {
            if (unit.team == currentPlayer)
                unit.RechargerEnergie();
        }
    }

    public bool HasEnoughPA() => CurrentStats.pa > 0;
    public bool HasEnoughPM() => CurrentStats.pm > 0;

    public void SpendPA()
    {
        CurrentStats.pa = Mathf.Max(0, CurrentStats.pa - 1);
        UpdateUI();
    }

    public void SpendPM()
    {
        CurrentStats.pm = Mathf.Max(0, CurrentStats.pm - 1);
        UpdateUI();
    }

    public void UpdateUI()
    {
        PlayerStats stats = CurrentStats;
        PieceInfoUI.instance?.UpdateTurnDisplay(currentPlayer, stats.pa, stats.maxPA, stats.pm, stats.maxPM);

        UIButtons.Instance?.RefreshButtons(showAction: false, showCancel: false, showAttackOptions: false);
    }
}
