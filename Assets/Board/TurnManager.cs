using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public ChessPiece.PieceColor currentPlayer = ChessPiece.PieceColor.White;

    [System.Serializable]
    public class PlayerStats
    {
        public int pa = 1;
        public int pm = 1;
        public int maxPA = 1;
        public int maxPM = 1;
    }

    public PlayerStats whiteStats = new PlayerStats();
    public PlayerStats blackStats = new PlayerStats();

    private int turnCountWhite = 1;
    private int turnCountBlack = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public PlayerStats CurrentStats =>
        currentPlayer == ChessPiece.PieceColor.White ? whiteStats : blackStats;

    public void NextTurn()
    {
        currentPlayer = currentPlayer == ChessPiece.PieceColor.White
            ? ChessPiece.PieceColor.Black
            : ChessPiece.PieceColor.White;

        if (currentPlayer == ChessPiece.PieceColor.White)
            turnCountWhite++;
        else
            turnCountBlack++;

        PlayerStats stats = CurrentStats;

        int turnCount = currentPlayer == ChessPiece.PieceColor.White ? turnCountWhite : turnCountBlack;

        if (turnCount > 1)
        {
            stats.maxPA = Mathf.Min(stats.maxPA + 1, 5);
            stats.maxPM = Mathf.Min(stats.maxPM + 1, 5);
        }

        stats.pa = stats.maxPA;
        stats.pm = stats.maxPM;
        Debug.Log($"[TurnManager] Tours - Blanc : {turnCountWhite}, Noir : {turnCountBlack}");
        Debug.Log($"[TurnManager] joue son tour #{turnCount}, maxPA = {stats.maxPA}, maxPM = {stats.maxPM}");
        UpdateUI();
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

        UIButtons.Instance?.ShowActionButtons(false);
    }
}
