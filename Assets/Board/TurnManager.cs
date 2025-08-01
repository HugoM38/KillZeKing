using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Hands")]
    public FullDeckGenerator playerHand;
    public FullDeckGenerator enemyHand;

    [Header("Tour actuel")]
    public BaseUnitScript.Team currentPlayer = BaseUnitScript.Team.Player;

    [System.Serializable]
    public class PlayerStats { public int pa=1, pm=1, maxPA=1, maxPM=1; }
    public PlayerStats playerStats = new PlayerStats();
    public PlayerStats enemyStats  = new PlayerStats();

    private int turnCountPlayer = 1;
    private int turnCountEnemy  = 0;

    void Awake()
    {
        if (Instance==null) Instance=this; else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public PlayerStats CurrentStats =>
        currentPlayer==BaseUnitScript.Team.Player ? playerStats : enemyStats;

    public void NextTurn()
    {
        // 1) Changement de joueur
        currentPlayer = (currentPlayer==BaseUnitScript.Team.Player)
            ? BaseUnitScript.Team.Enemy
            : BaseUnitScript.Team.Player;

        if (currentPlayer==BaseUnitScript.Team.Player) turnCountPlayer++;
        else                                           turnCountEnemy++;

        PlayerStats stats = CurrentStats;
        int turnCount = (currentPlayer==BaseUnitScript.Team.Player)
            ? turnCountPlayer : turnCountEnemy;

        // 2) Gain de PA/PM si >1
        if (turnCount>1)
        {
            stats.maxPA = Mathf.Min(stats.maxPA+1,5);
            stats.maxPM = Mathf.Min(stats.maxPM+1,5);
            RechargerEnergieDesUnites();
        }
        stats.pa = stats.maxPA;
        stats.pm = stats.maxPM;

        // 3) Pioche
        if (currentPlayer==BaseUnitScript.Team.Player)
            playerHand.DrawOneCard();
        else
            enemyHand.DrawOneCard();

        UpdateUI();
    }

    private void RechargerEnergieDesUnites()
    {
        var allUnits = Object.FindObjectsByType<BaseUnitScript>(FindObjectsSortMode.None);
        foreach (var u in allUnits)
            if (u.team == currentPlayer)
                u.SetCurrentEnergy(u.GetCurrentEnergy()+1);
    }

    public void SpendPA() { CurrentStats.pa = Mathf.Max(0,CurrentStats.pa-1); UpdateUI(); }
    public void SpendPM() { CurrentStats.pm = Mathf.Max(0,CurrentStats.pm-1); UpdateUI(); }

    private void UpdateUI()
    {
        var s = CurrentStats;
        PieceInfoUI.instance?.UpdateTurnDisplay(currentPlayer,s.pa,s.maxPA,s.pm,s.maxPM);
        UIButtons.Instance?.SetButtonsVisibility(
            showMove:false, showAttack:false, showSpecialAttack:false,
            showCancel:false, showEndTurn:true
        );
    }
}
