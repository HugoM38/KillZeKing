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
        if (piece != null)
        {
            Debug.Log($"[Click] {piece.name} sélectionné");
            PieceInfoUI.ShowInfo(piece);
        }
        else
        {
            Debug.LogWarning("Aucune pièce détectée sur cet objet.");
        }
    }
}
