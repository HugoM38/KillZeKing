using UnityEngine;

[RequireComponent(typeof(ChessPiece))]
public class PieceSelector : MonoBehaviour
{
    private ChessPiece piece;

    private void Awake()
    {
        piece = GetComponent<ChessPiece>();
    }
    private void OnMouseDown()
    {
        var sm = SelectionManager.Instance;
        if (sm == null || piece == null) return;

        // ✅ S'il y a une action en cours
        if (sm.currentState != PlayerActionState.None)
        {
            Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
            Vector2Int pos = piece.GetCurrentTilePosition(board);
            Tile myTile = board[pos.x, pos.y];

            // ⚠️ Si je suis sur une case valide → laisser le Tile gérer l’action
            if (sm.validMoves.Contains(myTile))
            {
                Debug.Log("[Info] Clic sur une pièce ennemie valide (traité par Tile)");
                myTile.ExecuteAttack();
                return;
            }

            // ❌ Sinon, on ignore pour ne pas casser l'action
            Debug.Log("[Ignoré] Clic sur une pièce pendant une action (non ciblée)");
            return;
        }

        // ✅ Aucune action en cours : sélectionner la pièce
        Tile[,] boardFull = FindFirstObjectByType<BoardGenerator>().GetBoard();
        sm.SelectPiece(piece, boardFull);

        Debug.Log($"[Sélection] {piece.type} ({piece.color})");
    }


}
