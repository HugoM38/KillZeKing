using UnityEngine;
using UnityEngine.EventSystems;

public class HidePieceInfoOnEmptyClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ✅ Si on clique sur un élément UI (bouton, panel...), on ignore
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // 🔍 On convertit le clic souris en coordonnées du monde 2D
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 🎯 On vérifie s'il y a un collider à cet endroit
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // ❌ Si on ne clique ni une pièce ni une case → annuler la sélection
            bool clickedPiece = hit.collider != null && hit.collider.GetComponent<ChessPiece>() != null;
            bool clickedTile = hit.collider != null && hit.collider.GetComponent<Tile>() != null;

            if (!clickedPiece && !clickedTile)
            {
                Tile[,] board = FindFirstObjectByType<BoardGenerator>().GetBoard();
                SelectionManager.Instance.ClearSelection(board);
            }
        }
    }
}
