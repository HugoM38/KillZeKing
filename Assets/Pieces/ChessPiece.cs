using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class ChessPiece : MonoBehaviour
{
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
    public enum PieceColor { White, Black }

    public PieceType type;
    public PieceColor color;

    // ➡️ Ajout des statistiques
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;

    private void Start()
    {
        currentHealth = maxHealth;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 1; // ➡️ Plus haut que les tiles
        }
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null && sr.sprite != null)
        {
            collider.size = sr.sprite.bounds.size;
            collider.offset = sr.sprite.bounds.center;
        }
    }
}
